﻿/*
 * Copyright 2010-2013 Bastian Eicher
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common.Collections;
using Common.Storage;
using ZeroInstall.DesktopIntegration.AccessPoints;
using ZeroInstall.DesktopIntegration.Properties;
using ZeroInstall.Model;

namespace ZeroInstall.DesktopIntegration
{
    /// <summary>
    /// Stores a list of applications and the kind of desktop integration the user chose for them.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "C5 collections don't need to be disposed.")]
    [XmlRoot("app-list", Namespace = XmlNamespace), XmlType("app-list", Namespace = XmlNamespace)]
    [XmlNamespace("xsi", XmlStorage.XsiNamespace)]
    //[XmlNamespace("caps", CapabilityList.XmlNamespace)]
    //[XmlNamespace("feed", Feed.XmlNamespace)]
    public sealed class AppList : XmlUnknown, ICloneable, IEquatable<AppList>
    {
        #region Constants
        /// <summary>
        /// The XML namespace used for storing application list data.
        /// </summary>
        public const string XmlNamespace = "http://0install.de/schema/desktop-integration/app-list";

        /// <summary>
        /// The URI to retrieve an XSD containing the XML Schema information for this class in serialized form.
        /// </summary>
        public const string XsdLocation = XmlNamespace + "/app-list.xsd";

        /// <summary>
        /// Provides XML Editors with location hints for XSD files.
        /// </summary>
        [XmlAttribute("schemaLocation", Namespace = XmlStorage.XsiNamespace)]
        public string XsiSchemaLocation = XmlNamespace + " " + XsdLocation;
        #endregion

        #region Properties
        private readonly List<AppEntry> _entries = new List<AppEntry>();

        /// <summary>
        /// A list of <see cref="AppEntry"/>s.
        /// </summary>
        [Description("A list of application entries.")]
        [XmlElement("app")]
        public List<AppEntry> Entries { get { return _entries; } }
        #endregion

        //--------------------//

        #region Access
        /// <summary>
        /// Checks whether an <see cref="AppEntry"/> for a specific interface ID exists.
        /// </summary>
        /// <param name="interfaceID">The <see cref="AppEntry.InterfaceID"/> to look for.</param>
        /// <returns><see langword="true"/> if a matching entry was found; <see langword="false"/> otherwise.</returns>
        public bool Contains(string interfaceID)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(interfaceID)) throw new ArgumentNullException("interfaceID");
            #endregion

            return Entries.Any(entry => ModelUtils.IDEquals(entry.InterfaceID, interfaceID));
        }

        /// <summary>
        /// Tries to find an <see cref="AppEntry"/> for a specific interface ID.
        /// </summary>
        /// <param name="interfaceID">The <see cref="AppEntry.InterfaceID"/> to look for.</param>
        /// <returns>The first matching <see cref="AppEntry"/> ; <see langword="null"/> if no match was found.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no entry matching the interface ID was found.</exception>
        public AppEntry this[string interfaceID]
        {
            get
            {
                #region Sanity checks
                if (string.IsNullOrEmpty(interfaceID)) throw new ArgumentNullException("interfaceID");
                #endregion

                return Entries.First(entry => ModelUtils.IDEquals(entry.InterfaceID, interfaceID),
                    noneException: () => new KeyNotFoundException(string.Format(Resources.AppNotInList, interfaceID)));
            }
        }
        #endregion

        #region Conflict IDs
        /// <summary>
        /// Returns a list of all conflict IDs and the <see cref="AccessPoint"/>s belong to.
        /// </summary>
        /// <seealso cref="AccessPoint.GetConflictIDs"/>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Performs some potentially slow computations.")]
        public IDictionary<string, ConflictData> GetConflictIDs()
        {
            var conflictIDs = new Dictionary<string, ConflictData>();
            foreach (var appEntry in Entries)
            {
                if (appEntry.AccessPoints == null) continue;
                foreach (var accessPoint in appEntry.AccessPoints.Entries)
                {
                    foreach (string conflictID in accessPoint.GetConflictIDs(appEntry).Where(conflictID => !conflictIDs.ContainsKey(conflictID)))
                        conflictIDs.Add(conflictID, new ConflictData(appEntry, accessPoint));
                }
            }
            return conflictIDs;
        }
        #endregion

        #region Storage
        /// <summary>
        /// Returns the default file path used to store the main <see cref="AppList"/> on this system.
        /// </summary>
        /// <param name="machineWide">Store the <see cref="AppList"/> machine-wide instead of just for the current user.</param>
        public static string GetDefaultPath(bool machineWide = false)
        {
            return Path.Combine(
                // Machine-wide storage cannot be portable, per-user storage can be portable
                machineWide ? Locations.GetIntegrationDirPath("0install.net", true, "desktop-integration") : Locations.GetSaveConfigPath("0install.net", false, "desktop-integration"),
                "app-list.xml");
        }
        #endregion

        //--------------------//

        #region Clone
        /// <summary>
        /// Creates a deep copy of this <see cref="AppList"/> instance.
        /// </summary>
        /// <returns>The new copy of the <see cref="AppList"/>.</returns>
        public AppList Clone()
        {
            var appList = new AppList {UnknownAttributes = UnknownAttributes, UnknownElements = UnknownElements};
            foreach (var entry in Entries) appList.Entries.Add(entry.Clone());

            return appList;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(AppList other)
        {
            if (other == null) return false;
            return base.Equals(other) && Entries.UnsequencedEquals(other.Entries);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is AppList && Equals((AppList)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ Entries.GetUnsequencedHashCode();
            }
        }
        #endregion
    }
}
