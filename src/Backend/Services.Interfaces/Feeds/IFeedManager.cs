/*
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
using System.IO;
using System.Net;
using JetBrains.Annotations;
using ZeroInstall.Store;
using ZeroInstall.Store.Feeds;
using ZeroInstall.Store.Model;
using ZeroInstall.Store.Trust;

namespace ZeroInstall.Services.Feeds
{
    /// <summary>
    /// Provides access to remote and local <see cref="Feed"/>s. Handles downloading, signature verification and caching.
    /// </summary>
    public interface IFeedManager
    {
        /// <summary>
        /// Set to <c>true</c> to re-download <see cref="Feed"/>s even if they are already in the <see cref="IFeedCache"/>.
        /// </summary>
        bool Refresh { get; set; }

        /// <summary>
        /// Is set to <c>true</c> if any <see cref="Feed"/> returned by <see cref="this"/> is getting stale and should be updated by setting <see cref="Refresh"/> to <c>true</c>.
        /// </summary>
        /// <remarks><see cref="Config.Freshness"/> controls the time span after which a feed is considered stale.</remarks>
        bool Stale { get; set; }

        /// <summary>
        /// Indicates whether <see cref="Stale"/> is <c>true</c> and <see cref="Config.NetworkUse"/> is <see cref="NetworkLevel.Full"/>.
        /// </summary>
        bool ShouldRefresh { get; }

        /// <summary>
        /// Returns a specific <see cref="Feed"/>. Automatically handles downloading, calling <see cref="Feed.Normalize"/> and caching. Updates the <see cref="Stale"/> indicator.
        /// </summary>
        /// <param name="feedUri">The canonical ID used to identify the feed.</param>
        /// <returns>The parsed <see cref="Feed"/> object.</returns>
        /// <remarks><see cref="Feed"/>s are always served from the <see cref="IFeedCache"/> if possible, unless <see cref="Refresh"/> is set to <c>true</c>.</remarks>
        /// <exception cref="OperationCanceledException">The user canceled the task.</exception>
        /// <exception cref="WebException">A problem occured while fetching the feed file.</exception>
        /// <exception cref="IOException">A problem occured while reading the feed file.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the cache is not permitted.</exception>
        /// <exception cref="SignatureException">The signature data of a remote feed file could not be verified.</exception>
        /// <exception cref="InvalidDataException"><see cref="Feed.Uri"/> is missing or does not match <paramref name="feedUri"/>.</exception>
        [NotNull]
        Feed this[[NotNull] FeedUri feedUri] { get; }

        /// <summary>
        /// Determines whether there is a stale cached copy of a particular feed.
        /// </summary>
        /// <param name="feedUri">The ID used to identify the feed. Must be an HTTP(S) URL.</param>
        /// <returns><c>true</c> if there is a stale copy in the cache or no copy at all; <c>false</c> if there is a fresh copy in the cache.</returns>
        bool IsStale([NotNull] FeedUri feedUri);

        /// <summary>
        /// Imports a remote <see cref="Feed"/> into the <see cref="IFeedCache"/> after verifying its signature.
        /// </summary>
        /// <param name="path">The path of a local copy of the feed.</param>
        /// <exception cref="IOException">A problem occured while reading the feed file.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the feed file or the cache is not permitted.</exception>
        /// <exception cref="SignatureException">The signature data of the feed file could not be handled or no signatures were trusted.</exception>
        /// <exception cref="InvalidDataException"><see cref="Feed.Uri"/> is missing.</exception>
        void ImportFeed([NotNull] string path);
    }
}
