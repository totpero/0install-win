﻿/*
 * Copyright 2010-2016 Bastian Eicher
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser Public License for more details.
 *
 * You should have received a copy of the GNU Lesser Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using NanoByte.Common;
using NanoByte.Common.Collections;
using NanoByte.Common.Storage;
using ZeroInstall.Services.Feeds;
using ZeroInstall.Services.PackageManagers;
using ZeroInstall.Store;
using ZeroInstall.Store.Implementations;
using ZeroInstall.Store.Model;
using ZeroInstall.Store.Model.Preferences;
using ZeroInstall.Store.Model.Selection;

namespace ZeroInstall.Services.Solvers
{
    /// <summary>
    /// Generates <see cref="SelectionCandidate"/>s for <see cref="ISolver"/>s to choose among.
    /// </summary>
    /// <remarks>Caches loaded <see cref="Feed"/>s, preferences, etc..</remarks>
    public class SelectionCandidateProvider
    {
        #region Depdendencies
        private readonly Config _config;
        private readonly IFeedManager _feedManager;
        private readonly IPackageManager _packageManager;

        /// <summary>
        /// Creates a new <see cref="SelectionCandidate"/> provider.
        /// </summary>
        /// <param name="config">User settings controlling network behaviour, solving, etc.</param>
        /// <param name="feedManager">Provides access to remote and local <see cref="Feed"/>s. Handles downloading, signature verification and caching.</param>
        /// <param name="store">Used to check which <see cref="Implementation"/>s are already cached.</param>
        /// <param name="packageManager">An external package manager that can install <see cref="PackageImplementation"/>s.</param>
        /// <param name="languages">The preferred languages for the implementation.</param>
        public SelectionCandidateProvider([NotNull] Config config, [NotNull] IFeedManager feedManager, [NotNull] IStore store, [NotNull] IPackageManager packageManager, [NotNull] LanguageSet languages)
        {
            #region Sanity checks
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (feedManager == null) throw new ArgumentNullException(nameof(feedManager));
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (packageManager == null) throw new ArgumentNullException(nameof(packageManager));
            if (languages == null) throw new ArgumentNullException(nameof(languages));
            #endregion

            _config = config;
            _feedManager = feedManager;
            _isCached = BuildCacheChecker(store);
            _packageManager = packageManager;
            _comparer = new TransparentCache<FeedUri, SelectionCandidateComparer>(id => new SelectionCandidateComparer(config, _isCached, _interfacePreferences[id].StabilityPolicy, languages));
        }

        /// <summary>
        /// Returns a predicate that checks whether a given <see cref="Implementation"/> is cached in the <paramref name="store"/>
        /// (or has an <see cref="ImplementationBase.LocalPath"/> or <see cref="ExternalImplementation.IsInstalled"/>).
        /// </summary>
        private static Predicate<Implementation> BuildCacheChecker(IStore store)
        {
            var storeContainsCache = new TransparentCache<ManifestDigest, bool>(store.Contains);

            return implementation =>
            {
                if (!string.IsNullOrEmpty(implementation.LocalPath)) return true;

                var externalImplementation = implementation as ExternalImplementation;
                if (externalImplementation != null) return externalImplementation.IsInstalled;

                return storeContainsCache[implementation.ManifestDigest];
            };
        }
        #endregion

        #region Caches
        /// <summary>Maps interface URIs to <see cref="InterfacePreferences"/>.</summary>
        private readonly TransparentCache<FeedUri, InterfacePreferences> _interfacePreferences = new TransparentCache<FeedUri, InterfacePreferences>(InterfacePreferences.LoadForSafe);

        /// <summary>Maps interface URIs to <see cref="SelectionCandidateComparer"/>s.</summary>
        private readonly TransparentCache<FeedUri, SelectionCandidateComparer> _comparer;

        /// <summary>
        /// Maps <see cref="ImplementationBase.ID"/>s to <see cref="Implementation"/>s provided by <see cref="IPackageManager.Query"/>.
        /// </summary>
        private readonly Dictionary<string, ExternalImplementation> _externalImplementations = new Dictionary<string, ExternalImplementation>();

        /// <summary>Indicates which implementations are already cached in the <see cref="IStore"/>.</summary>
        private readonly Predicate<Implementation> _isCached;
        #endregion

        /// <summary>
        /// Gets all <see cref="SelectionCandidate"/>s for a specific set of <see cref="Requirements"/> sorted from best to worst.
        /// </summary>
        public IList<SelectionCandidate> GetSortedCandidates([NotNull] Requirements requirements)
        {
            var candidates = GetFeeds(requirements)
                .SelectMany(x => GetCandidates(x.Key, x.Value, requirements))
                .ToList();
            candidates.Sort(_comparer[requirements.InterfaceUri]);
            return candidates;
        }

        /// <summary>
        /// Loads the main feed for the specified <paramref name="requirements"/>, additional feeds added by local configuration and <see cref="Feed.Feeds"/> references.
        /// </summary>
        /// <returns>A dictionary mapping <see cref="FeedUri"/>s to the actual <see cref="Feed"/>s loaded from there.</returns>
        private IDictionary<FeedUri, Feed> GetFeeds(Requirements requirements)
        {
            var dictionary = new Dictionary<FeedUri, Feed>();

            AddFeedToDict(dictionary, requirements.InterfaceUri, requirements);

            foreach (var uri in GetNativeFeedPaths(requirements.InterfaceUri))
                AddFeedToDict(dictionary, uri, requirements);

            foreach (var uri in GetSitePackagePaths(requirements.InterfaceUri))
                AddFeedToDict(dictionary, uri, requirements);

            foreach (var reference in _interfacePreferences[requirements.InterfaceUri].Feeds)
            {
                try
                {
                    AddFeedToDict(dictionary, reference.Source, requirements);
                }
                    #region Error handling
                catch (IOException ex)
                {
                    // Wrap exception to add context information
                    throw new IOException(
                        string.Format("Failed to load feed {1} manually registered for interface {0}. Try '0install remove-feed {0} {1}'.", requirements.InterfaceUri.ToStringRfc(), reference.Source.ToStringRfc()),
                        ex);
                }
                #endregion
            }

            return dictionary;
        }

        private static IEnumerable<FeedUri> GetNativeFeedPaths(FeedUri interfaceUri)
        {
            return Locations.GetLoadDataPaths("0install.net", true, "native_feeds", interfaceUri.PrettyEscape())
                .Select(x => new FeedUri(x));
        }

        private static IEnumerable<FeedUri> GetSitePackagePaths(FeedUri interfaceUri)
        {
            var sitePackageDirs = Locations.GetLoadDataPaths("0install.net", isFile: false, resource: interfaceUri.EscapeComponent().Prepend("site-packages"));
            var subDirectories = sitePackageDirs.SelectMany(x => new DirectoryInfo(x).GetDirectories());
            return
                from dir in subDirectories
                let path = Path.Combine(dir.FullName, "0install" + Path.DirectorySeparatorChar + "feed.xml")
                where File.Exists(path)
                select new FeedUri(path);
        }

        /// <summary>
        /// Loads a feed and adds it to a dictionary if it is not already in it. Recursivley adds <see cref="Feed.Feeds"/> references.
        /// </summary>
        /// <param name="dictionary">The dictionary to add the feed to.</param>
        /// <param name="feedUri">The URI to load the feed from</param>
        /// <param name="requirements">Requirements to apply as a filter to <see cref="Feed.Feeds"/> references before following them.</param>
        private void AddFeedToDict([NotNull] IDictionary<FeedUri, Feed> dictionary, [CanBeNull] FeedUri feedUri, [NotNull] Requirements requirements)
        {
            if (feedUri == null || dictionary.ContainsKey(feedUri)) return;

            var feed = _feedManager[feedUri];
            if (feed.MinInjectorVersion != null && FeedElement.ZeroInstallVersion < feed.MinInjectorVersion)
            {
                Log.Warn($"The solver version is too old. The feed '{feedUri}' requires at least version {feed.MinInjectorVersion} but the installed version is {FeedElement.ZeroInstallVersion}. Try updating Zero Install.");
                return;
            }

            dictionary.Add(feedUri, feed);
            foreach (var reference in feed.Feeds)
            {
                if (reference.Architecture.IsCompatible(requirements.Architecture) &&
                    (reference.Languages.Count == 0 || reference.Languages.ContainsAny(requirements.Languages, ignoreCountry: true)))
                    AddFeedToDict(dictionary, reference.Source, requirements);
            }
        }

        private IEnumerable<SelectionCandidate> GetCandidates(FeedUri feedUri, Feed feed, Requirements requirements)
        {
            var feedPreferences = FeedPreferences.LoadForSafe(feedUri);

            foreach (var element in feed.Elements)
            {
                var packageImplementation = element as PackageImplementation;
                if (packageImplementation != null)
                { // Each <package-implementation> provides 0..n selection candidates
                    var externalImplementations = _packageManager.Query(packageImplementation, requirements.Distributions.ToArray());
                    foreach (var externalImplementation in externalImplementations)
                    {
                        _externalImplementations[externalImplementation.ID] = externalImplementation;
                        yield return new SelectionCandidate(new FeedUri(FeedUri.FromDistributionPrefix + feedUri), feedPreferences, externalImplementation, requirements);
                    }
                }
                else if (requirements.Distributions.ContainsOrEmpty(Restriction.DistributionZeroInstall))
                {
                    var implementation = element as Implementation;
                    if (implementation != null)
                    { // Each <implementation> provides 1 selection candidate
                        yield return new SelectionCandidate(feedUri, feedPreferences, implementation, requirements,
                            offlineUncached: (_config.NetworkUse == NetworkLevel.Offline) && !_isCached(implementation));
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the original <see cref="Implementation"/> an <see cref="ImplementationSelection"/> was based ofF.
        /// </summary>
        public Implementation LookupOriginalImplementation(ImplementationSelection implemenationSelection)
        {
            #region Sanity checks
            if (implemenationSelection == null) throw new ArgumentNullException(nameof(implemenationSelection));
            #endregion

            return implemenationSelection.ID.StartsWith(ExternalImplementation.PackagePrefix)
                ? _externalImplementations[implemenationSelection.ID]
                : _feedManager[implemenationSelection.FromFeed ?? implemenationSelection.InterfaceUri][implemenationSelection.ID];
        }
    }
}
