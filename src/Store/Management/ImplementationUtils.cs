﻿/*
 * Copyright 2010-2011 Bastian Eicher
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
using ZeroInstall.Model;

namespace ZeroInstall.Store.Management
{
    /// <summary>
    /// Provides utiltity methods for managing <see cref="Implementation"/>s.
    /// </summary>
    public static class ImplementationUtils
    {
        /// <summary>
        /// Tries to find an <see cref="Implementation"/> with a specific <see cref="ManifestDigest"/> in a list of <see cref="Feed"/>s.
        /// </summary>
        /// <param name="digest">The digest to search for.</param>
        /// <param name="feeds">The list of <see cref="Feed"/>s to search in.</param>
        /// <param name="feed">Returns the <see cref="Feed"/> a match was found in; <see langword="null"/> if no match found.</param>
        /// <returns>The matching <see cref="Implementation"/>; <see langword="null"/> if no match found.</returns>
        public static Model.Implementation GetImplementation(ManifestDigest digest, IEnumerable<Feed> feeds, out Feed feed)
        {
            #region Sanity checks
            if (feeds == null) throw new ArgumentNullException("feeds");
            #endregion

            foreach (var curFeed in feeds)
            {
                var implementation = curFeed.GetImplementation(digest);
                if (implementation != null)
                {
                    feed = curFeed;
                    return implementation;
                }
            }

            feed = null;
            return null;
        }
    }
}
