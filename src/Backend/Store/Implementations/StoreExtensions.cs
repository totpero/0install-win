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
using System.IO;
using JetBrains.Annotations;
using NanoByte.Common.Tasks;

namespace ZeroInstall.Store.Implementations
{
    /// <summary>
    /// Contains extension methods for <see cref="IStore"/>s.
    /// </summary>
    public static class StoreExtensions
    {
        /// <summary>
        /// Removes all implementations from a store.
        /// </summary>
        /// <param name="store">The store to be purged.</param>
        /// <param name="handler">A callback object used when the the user is to be informed about progress.</param>
        /// <exception cref="OperationCanceledException">The user canceled the task.</exception>
        /// <exception cref="IOException">An implementation could not be deleted.</exception>
        /// <exception cref="UnauthorizedAccessException">Write access to the store is not permitted.</exception>
        public static void Purge([NotNull] this IStore store, [NotNull] ITaskHandler handler)
        {
            #region Sanity checks
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            #endregion

            foreach (var manifestDigest in store.ListAll())
                store.Remove(manifestDigest, handler);
        }
    }
}