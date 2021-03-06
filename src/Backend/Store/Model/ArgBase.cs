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
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using ZeroInstall.Store.Model.Design;

namespace ZeroInstall.Store.Model
{
    /// <summary>
    /// Common base class for <see cref="Arg"/> and <see cref="ForEachArgs"/>.
    /// </summary>
    [TypeConverter(typeof(ArgBaseConverter))]
    [XmlType("arg-base", Namespace = Feed.XmlNamespace)]
    public abstract class ArgBase : FeedElement, ICloneable
    {
        /// <summary>
        /// Performs sanity checks.
        /// </summary>
        /// <exception cref="InvalidDataException">One or more required fields are not set.</exception>
        /// <remarks>This method should be called to prepare a <see cref="Feed"/> for solver processing. Do not call it if you plan on serializing the feed again since it may loose some of its structure.</remarks>
        public abstract void Normalize();

        /// <summary>
        /// Creates a deep copy of this <see cref="ArgBase"/> instance.
        /// </summary>
        /// <returns>The new copy of the <see cref="ArgBase"/>.</returns>
        public abstract ArgBase Clone();

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Convenience cast for turning strings into plain <see cref="Arg"/>s.
        /// </summary>
        public static implicit operator ArgBase(string value) => new Arg {Value = value};
    }
}
