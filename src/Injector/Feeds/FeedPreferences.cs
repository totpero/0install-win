﻿/*
 * Copyright 2010-2012 Bastian Eicher
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;
using Common;
using Common.Collections;
using Common.Storage;
using Common.Utils;
using ZeroInstall.Model;

namespace ZeroInstall.Injector.Feeds
{
    /// <summary>
    /// Stores user-specific preferences for a <see cref="Model.Feed"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "C5 collections don't need to be disposed.")]
    [XmlRoot("feed-preferences", Namespace = Feed.XmlNamespace)]
    [XmlType("feed-preferences", Namespace = Feed.XmlNamespace)]
    public sealed class FeedPreferences : XmlUnknown, ICloneable, IEquatable<FeedPreferences>
    {
        #region Properties
        /// <summary>
        /// The point in time this feed was last checked for updates.
        /// </summary>
        [Description("The point in time this feed was last checked for updates.")]
        [XmlIgnore]
        public DateTime LastChecked { get; set; }

        /// <summary>Used for XML serialization.</summary>
        /// <seealso cref="LastChecked"/>
        [XmlAttribute("last-checked"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long LastCheckedUnix { get { return FileUtils.ToUnixTime(LastChecked); } set { LastChecked = FileUtils.FromUnixTime(value); } }

        // Preserve order
        private readonly C5.LinkedList<ImplementationPreferences> _implementations = new C5.LinkedList<ImplementationPreferences>();

        /// <summary>
        /// A list of implementation-specific user-overrides.
        /// </summary>
        [Description("A list of implementation-specific user-overrides.")]
        [XmlElement("implementation")]
        // Note: Can not use ICollection<T> interface with XML Serialization
            public C5.LinkedList<ImplementationPreferences> Implementations
        {
            get { return _implementations; }
        }
        #endregion

        //--------------------//

        #region Access
        /// <summary>
        /// Retrieves an existing entry from <see cref="Implementations"/> by ID or creates a new one if no appropriate one exists.
        /// </summary>
        /// <param name="id">The <see cref="ImplementationPreferences.ID"/> to search for.</param>
        /// <returns>The found or newly created <see cref="ImplementationPreferences"/>.</returns>
        public ImplementationPreferences this[string id]
        {
            get
            {
                ImplementationPreferences result;
                if (!Implementations.Find(implementation => implementation.ID == id, out result))
                {
                    result = new ImplementationPreferences {ID = id};
                    Implementations.Add(result);
                }
                return result;
            }
        }
        #endregion

        #region Normalize
        /// <summary>
        /// Removes superflous entries from <see cref="Implementations"/>.
        /// </summary>
        public void Normalize()
        {
            _implementations.RemoveAll(_implementations.FindAll(implementation => implementation.IsSuperflous));
        }
        #endregion

        #region Storage
        /// <summary>
        /// Loads <see cref="FeedPreferences"/> from an XML file.
        /// </summary>
        /// <param name="path">The file to load from.</param>
        /// <returns>The loaded <see cref="FeedPreferences"/>.</returns>
        /// <exception cref="IOException">Thrown if a problem occurs while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if read access to the file is not permitted.</exception>
        /// <exception cref="InvalidDataException">Thrown if a problem occurs while deserializing the XML data.</exception>
        public static FeedPreferences Load(string path)
        {
            return XmlStorage.Load<FeedPreferences>(path);
        }

        /// <summary>
        /// Loads <see cref="FeedPreferences"/> for a specific feed.
        /// </summary>
        /// <param name="feedID">The feed to load the preferences for.</param>
        /// <returns>The loaded <see cref="FeedPreferences"/>.</returns>
        /// <exception cref="IOException">Thrown if a problem occurs while reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if read access to the file is not permitted.</exception>
        /// <exception cref="InvalidDataException">Thrown if a problem occurs while deserializing the XML data.</exception>
        public static FeedPreferences LoadFor(string feedID)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(feedID)) throw new ArgumentNullException("feedID");
            #endregion

            var path = EnumerableUtils.First(Locations.GetLoadConfigPaths("0install.net", true, "injector", "feeds", ModelUtils.PrettyEscape(feedID)));
            if (string.IsNullOrEmpty(path)) return new FeedPreferences();

            return XmlStorage.Load<FeedPreferences>(path);
        }

        /// <summary>
        /// Tries to load <see cref="FeedPreferences"/> for a specific feed. Automatically falls back to defaults on errors.
        /// </summary>
        /// <param name="feedID">The feed to load the preferences for.</param>
        /// <returns>The loaded <see cref="FeedPreferences"/> or default value if there was a problem.</returns>
        public static FeedPreferences LoadForSafe(string feedID)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(feedID)) throw new ArgumentNullException("feedID");
            #endregion

            try
            {
                return LoadFor(feedID);
            }
                #region Error handling
            catch (FileNotFoundException)
            {
                Log.Info("Creating new feed preferences file for '" + feedID + "'.");
                return new FeedPreferences();
            }
            catch (IOException ex)
            {
                Log.Warn("Error loading feed preferences for '" + feedID + "'. Reverting to default values.");
                Log.Error(ex);
                return new FeedPreferences();
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Warn("Error loading feed preferences for '" + feedID + "'. Reverting to default values.");
                Log.Error(ex);
                return new FeedPreferences();
            }
            catch (InvalidDataException ex)
            {
                Log.Warn("Error loading feed preferences for '" + feedID + "'. Reverting to default values.");
                Log.Error(ex);
                return new FeedPreferences();
            }
            #endregion
        }

        /// <summary>
        /// Saves these <see cref="FeedPreferences"/> to an XML file.
        /// </summary>
        /// <param name="path">The file to save in.</param>
        /// <exception cref="IOException">Thrown if a problem occurs while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if write access to the file is not permitted.</exception>
        public void Save(string path)
        {
            Normalize();
            XmlStorage.Save(path, this);
        }

        /// <summary>
        /// Saves these <see cref="FeedPreferences"/> for a specific feed.
        /// </summary>
        /// <param name="feedID">The feed to save the preferences for.</param>
        /// <exception cref="IOException">Thrown if a problem occurs while writing the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if write access to the file is not permitted.</exception>
        public void SaveFor(string feedID)
        {
            Normalize();
            var path = Locations.GetSaveConfigPath("0install.net", true, "injector", "feeds", ModelUtils.PrettyEscape(feedID));
            XmlStorage.Save(path, this);
        }
        #endregion

        //--------------------//

        #region Clone
        /// <summary>
        /// Creates a deep copy of this <see cref="FeedPreferences"/> instance.
        /// </summary>
        /// <returns>The new copy of the <see cref="FeedPreferences"/>.</returns>
        public FeedPreferences Clone()
        {
            var feedPreferences = new FeedPreferences {UnknownAttributes = UnknownAttributes, UnknownElements = UnknownElements, LastChecked = LastChecked};
            foreach (var implementation in Implementations) feedPreferences.Implementations.Add(implementation.Clone());

            return feedPreferences;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Returns the preferences in the form "FeedPreferences: LastChecked". Not safe for parsing!
        /// </summary>
        public override string ToString()
        {
            return string.Format("FeedPreferences: {0}", LastChecked);
        }
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(FeedPreferences other)
        {
            if (other == null) return false;
            return base.Equals(other) && LastChecked == other.LastChecked && Implementations.SequencedEquals(other.Implementations);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is FeedPreferences && Equals((FeedPreferences)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ LastChecked.GetHashCode();
                result = (result * 397) ^ Implementations.GetSequencedHashCode();
                return result;
            }
        }
        #endregion
    }
}
