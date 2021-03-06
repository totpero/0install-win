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
using System.ComponentModel;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NanoByte.Common.Collections;
using ZeroInstall.Store.Model.Design;

namespace ZeroInstall.Store.Model
{
    /// <summary>
    /// Associates a <see cref="Command"/> with a user-friendly name and description.
    /// </summary>
    /// <seealso cref="Feed.EntryPoints"/>
    [Description("Associates a command with a user-friendly name and description.")]
    [Serializable, XmlRoot("entry-point", Namespace = Feed.XmlNamespace), XmlType("entry-point", Namespace = Feed.XmlNamespace)]
    public sealed class EntryPoint : FeedElement, IIconContainer, ISummaryContainer, ICloneable, IEquatable<EntryPoint>
    {
        /// <summary>
        /// The name of the <see cref="Command"/> this entry point represents.
        /// </summary>
        [Description("The name of the command this entry point represents.")]
        [TypeConverter(typeof(CommandNameConverter))]
        [XmlAttribute("command")]
        public string Command { get; set; }

        /// <summary>
        /// The canonical name of the binary supplying the command (without file extensions). This is used to suggest suitable alias names.
        /// </summary>
        /// <remarks>Will default to <see cref="Command"/> when left <c>null</c>.</remarks>
        [Description("The canonical name of the binary supplying the command (without file extensions). This is used to suggest suitable alias names.")]
        [XmlAttribute("binary-name"), DefaultValue(""), CanBeNull]
        public string BinaryName { get; set; }

        /// <summary>
        /// If <c>true</c>, indicates that the <see cref="Command"/> represented by this entry point requires a terminal in order to run.
        /// </summary>
        [Description("If true, indicates that the Command represented by this entry point requires a terminal in order to run.")]
        [XmlIgnore, DefaultValue(false)]
        public bool NeedsTerminal { get; set; }

        /// <summary>
        /// If <c>true</c>, indicates that this entry point should be offered as an auto-start candidate to the user.
        /// </summary>
        [Description("If true, indicates that this entry point should be offered as an auto-start candidate to the user.")]
        [XmlIgnore, DefaultValue(false)]
        public bool SuggestAutoStart { get; set; }

        /// <summary>
        /// If <c>true</c>, indicates that this entry point should be offered as a candidate for the "Send To" context menu to the user.
        /// </summary>
        [Description("If true, indicates that this entry point should be offered as a candidate for the \"Send To\" context menu to the user.")]
        [XmlIgnore, DefaultValue(false)]
        public bool SuggestSendTo { get; set; }

        #region XML serialization
        /// <summary>Used for XML serialization.</summary>
        /// <seealso cref="NeedsTerminal"/>
        [XmlElement("needs-terminal"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public string NeedsTerminalString { get { return (NeedsTerminal ? "" : null); } set { NeedsTerminal = (value != null); } }

        /// <summary>Used for XML serialization.</summary>
        /// <seealso cref="SuggestAutoStart"/>
        [XmlElement("suggest-auto-start"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public string SuggestAutoStartString { get { return (SuggestAutoStart ? "" : null); } set { SuggestAutoStart = (value != null); } }

        /// <summary>Used for XML serialization.</summary>
        /// <seealso cref="SuggestSendTo"/>
        [XmlElement("suggest-send-to"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public string SuggestSendToString { get { return (SuggestSendTo ? "" : null); } set { SuggestSendTo = (value != null); } }
        #endregion

        /// <summary>
        /// User-friendly names for the command. If not present, <see cref="Command"/> is used instead.
        /// </summary>
        [Browsable(false)]
        [XmlElement("name"), NotNull]
        public LocalizableStringCollection Names { get; } = new LocalizableStringCollection();

        /// <inheritdoc/>
        [Browsable(false)]
        [XmlElement("summary")]
        public LocalizableStringCollection Summaries { get; } = new LocalizableStringCollection();

        /// <inheritdoc/>
        [Browsable(false)]
        [XmlElement("description")]
        public LocalizableStringCollection Descriptions { get; } = new LocalizableStringCollection();

        /// <summary>
        /// Zero or more icons representing the command. Used for desktop icons, menu entries, etc..
        /// </summary>
        [Browsable(false)]
        [XmlElement("icon"), NotNull]
        public List<Icon> Icons { get; } = new List<Icon>();

        #region Conversion
        /// <summary>
        /// Returns the EntryPoint in the form "Command (BinaryName)". Not safe for parsing!
        /// </summary>
        public override string ToString() => string.IsNullOrEmpty(BinaryName)
            ? Command
            : Command + " (" + BinaryName + ")";
        #endregion

        #region Clone
        /// <summary>
        /// Creates a deep copy of this <see cref="EntryPoint"/> instance.
        /// </summary>
        /// <returns>The new copy of the <see cref="EntryPoint"/>.</returns>
        public EntryPoint Clone()
        {
            var newEntryPoint = new EntryPoint {UnknownAttributes = UnknownAttributes, UnknownElements = UnknownElements, IfZeroInstallVersion = IfZeroInstallVersion, Command = Command, BinaryName = BinaryName, NeedsTerminal = NeedsTerminal};
            newEntryPoint.Names.AddRange(Names.CloneElements());
            newEntryPoint.Summaries.AddRange(Summaries.CloneElements());
            newEntryPoint.Descriptions.AddRange(Descriptions.CloneElements());
            newEntryPoint.Icons.AddRange(Icons);
            return newEntryPoint;
        }

        object ICloneable.Clone() => Clone();
        #endregion

        #region Equality
        /// <inheritdoc/>
        public bool Equals(EntryPoint other)
        {
            if (other == null) return false;
            return base.Equals(other) &&
                   Command == other.Command && BinaryName == other.BinaryName && NeedsTerminal == other.NeedsTerminal &&
                   Names.UnsequencedEquals(other.Names) && Summaries.UnsequencedEquals(other.Summaries) && Descriptions.UnsequencedEquals(other.Descriptions) && Icons.UnsequencedEquals(other.Icons);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            return obj is EntryPoint && Equals((EntryPoint)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result * 397) ^ Command?.GetHashCode() ?? 0;
                result = (result * 397) ^ BinaryName?.GetHashCode() ?? 0;
                result = (result * 397) ^ NeedsTerminal.GetHashCode();
                result = (result * 397) ^ Names.GetUnsequencedHashCode();
                result = (result * 397) ^ Summaries.GetUnsequencedHashCode();
                result = (result * 397) ^ Descriptions.GetUnsequencedHashCode();
                result = (result * 397) ^ Icons.GetUnsequencedHashCode();
                return result;
            }
        }
        #endregion
    }
}
