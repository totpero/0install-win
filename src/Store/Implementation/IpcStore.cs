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
using System.IO;
using System.Runtime.Remoting;
using Common;
using Common.Tasks;
using ZeroInstall.Model;
using ZeroInstall.Store.Implementation.Archive;

namespace ZeroInstall.Store.Implementation
{
    /// <summary>
    /// Provides transparent access to an <see cref="IStore"/> running in another process (the Store Service).
    /// </summary>
    public partial class IpcStore : IStore
    {
        #region List all
        /// <inheritdoc/>
        public IEnumerable<ManifestDigest> ListAll()
        {
            try
            {
                return GetServiceProxy().ListAll();
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
                return new ManifestDigest[0];
            }
            #endregion
        }

        /// <inheritdoc/>
        public IEnumerable<string> ListAllTemp()
        {
            try
            {
                return GetServiceProxy().ListAllTemp();
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
                return new string[0];
            }
            #endregion
        }
        #endregion

        #region Contains
        /// <inheritdoc/>
        public bool Contains(ManifestDigest manifestDigest)
        {
            try
            {
                return GetServiceProxy().Contains(manifestDigest);
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
                return false;
            }
            #endregion
        }

        /// <inheritdoc/>
        public bool Contains(string directory)
        {
            try
            {
                return GetServiceProxy().Contains(directory);
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
                return false;
            }
            #endregion
        }
        #endregion

        #region Get path
        /// <inheritdoc/>
        public string GetPath(ManifestDigest manifestDigest)
        {
            try
            {
                return GetServiceProxy().GetPath(manifestDigest);
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
                return null;
            }
            #endregion
        }
        #endregion

        #region Add
        /// <inheritdoc/>
        public void AddDirectory(string path, ManifestDigest manifestDigest, ITaskHandler handler)
        {
            try
            {
                GetServiceProxy().AddDirectory(path, manifestDigest, handler);
            }
                #region Error handling
            catch (RemotingException ex)
            {
                throw new IOException(ex.Message, ex);
            }
            #endregion
        }

        /// <inheritdoc/>
        public void AddArchives(IEnumerable<ArchiveFileInfo> archiveInfos, ManifestDigest manifestDigest, ITaskHandler handler)
        {
            try
            {
                GetServiceProxy().AddArchives(archiveInfos, manifestDigest, handler);
            }
                #region Error handling
            catch (RemotingException ex)
            {
                throw new IOException(ex.Message, ex);
            }
            #endregion
        }
        #endregion

        #region Remove
        /// <inheritdoc/>
        public void Remove(ManifestDigest manifestDigest)
        {
            try
            {
                GetServiceProxy().Remove(manifestDigest);
            }
                #region Error handling
            catch (RemotingException ex)
            {
                throw new ImplementationNotFoundException(ex.Message, ex);
            }
            #endregion
        }
        #endregion

        #region Optimise
        /// <inheritdoc/>
        public void Optimise(ITaskHandler handler)
        {
            try
            {
                GetServiceProxy().Optimise(handler);
            }
                #region Sanity checks
            catch (IOException ex)
            {
                Log.Error(ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error(ex);
            }
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
            }
            #endregion
        }
        #endregion

        #region Verify
        /// <inheritdoc/>
        public void Verify(ManifestDigest manifestDigest, ITaskHandler handler)
        {
            try
            {
                GetServiceProxy().Verify(manifestDigest, handler);
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
            }
            #endregion
        }

        /// <inheritdoc/>
        public IEnumerable<DigestMismatchException> Audit(ITaskHandler handler)
        {
            try
            {
                return GetServiceProxy().Audit(handler);
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
                return new DigestMismatchException[0];
            }
            #endregion
        }
        #endregion

        #region Caches
        /// <inheritdoc/>
        public void Flush()
        {
            try
            {
                GetServiceProxy().Flush();
            }
                #region Error handling
            catch (RemotingException)
            {
                // Ignore remoting errors in case service is offline
            }
            #endregion
        }
        #endregion
    }
}