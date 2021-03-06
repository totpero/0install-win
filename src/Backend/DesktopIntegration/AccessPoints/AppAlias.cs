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
using System.Xml.Serialization;
using NanoByte.Common.Native;
using NanoByte.Common.Tasks;
using ZeroInstall.Store;
using ZeroInstall.Store.Model;

namespace ZeroInstall.DesktopIntegration.AccessPoints
{
    /// <summary>
    /// Makes an application discoverable via the system's search PATH.
    /// </summary>
    [XmlType("alias", Namespace = AppList.XmlNamespace)]
    public class AppAlias : CommandAccessPoint, IEquatable<AppAlias>
    {
        #region Constants
        /// <summary>
        /// The name of this category of <see cref="AccessPoint"/>s as used by command-line interfaces.
        /// </summary>
        public const string CategoryName = "aliases";
        #endregion

        /// <inheritdoc/>
        public override IEnumerable<string> GetConflictIDs(AppEntry appEntry) => new[] {"alias:" + Name};

        /// <inheritdoc/>
        public override void Apply(AppEntry appEntry, Feed feed, ITaskHandler handler, bool machineWide)
        {
            #region Sanity checks
            if (appEntry == null) throw new ArgumentNullException(nameof(appEntry));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            #endregion

            var target = new FeedTarget(appEntry.InterfaceUri, feed);
            if (WindowsUtils.IsWindows) Windows.AppAlias.Create(target, Command, Name, machineWide, handler);
            else if (UnixUtils.IsUnix) Unix.AppAlias.Create(target, Command, Name, handler, machineWide);
        }

        /// <inheritdoc/>
        public override void Unapply(AppEntry appEntry, bool machineWide)
        {
            #region Sanity checks
            if (appEntry == null) throw new ArgumentNullException(nameof(appEntry));
            #endregion

            if (WindowsUtils.IsWindows) Windows.AppAlias.Remove(Name, machineWide);
            else if (UnixUtils.IsUnix) Unix.AppAlias.Remove(Name, machineWide);
        }

        #region Clone
        /// <inheritdoc/>
        public override AccessPoint Clone() => new AppAlias {UnknownAttributes = UnknownAttributes, UnknownElements = UnknownElements, Name = Name, Command = Command};
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(AppAlias other)
        {
            return base.Equals(other);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            return obj.GetType() == typeof(AppAlias) && Equals((AppAlias)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
