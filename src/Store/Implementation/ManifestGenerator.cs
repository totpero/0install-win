﻿/*
 * Copyright 2010-2012 Bastian Eicher, Roland Leopold Walkling
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
using Common.Tasks;
using Common.Utils;
using ZeroInstall.Store.Properties;

namespace ZeroInstall.Store.Implementation
{
    /// <summary>
    /// Generates a <see cref="Manifest"/> for a directory in the filesystem as a background task.
    /// </summary>
    public class ManifestGenerator : ThreadTask
    {
        #region Properties
        /// <inheritdoc />
        public override string Name { get { return string.Format(Resources.GeneratingManifest, Format, Path.GetFileName(TargetDir)); } }

        /// <summary>
        /// The path of the directory to analyze.
        /// </summary>
        public string TargetDir { get; private set; }

        /// <summary>
        /// The format of the manifest to generate.
        /// </summary>
        public ManifestFormat Format { get; private set; }

        /// <summary>
        /// If <see cref="ThreadTask.State"/> is <see cref="TaskState.Complete"/> this property contains the generated <see cref="Manifest"/>; otherwise it's <see langword="null"/>.
        /// </summary>
        public Manifest Result { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Prepares to generate a manifest for a directory in the filesystem.
        /// </summary>
        /// <param name="path">The path of the directory to analyze.</param>
        /// <param name="format">The format of the manifest to generate.</param>
        public ManifestGenerator(string path, ManifestFormat format)
        {
            #region Sanity checks
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (format == null) throw new ArgumentNullException("format");
            #endregion

            TargetDir = path;
            Format = format;
        }
        #endregion

        //--------------------//

        #region Thread code
        /// <inheritdoc />
        protected override void RunTask()
        {
            try
            {
                if (CancelRequest.WaitOne(0, false)) throw new OperationCanceledException();
                lock (StateLock) State = TaskState.Started;

                // Get the complete (recursive) content of the directory sorted according to the format specification
                var entries = Format.GetSortedDirectoryEntries(TargetDir);
                BytesTotal = TotalFileSize(entries);

                var externalXbits = FlagUtils.GetExternalFlags(".xbit", TargetDir);
                var externalSymlinks = FlagUtils.GetExternalFlags(".symlink", TargetDir);

                if (CancelRequest.WaitOne(0, false)) throw new OperationCanceledException();
                lock (StateLock) State = TaskState.Data;

                // Iterate through the directory listing to build a list of manifets entries
                var nodes = new C5.ArrayList<ManifestNode>(entries.Length);
                foreach (var entry in entries)
                {
                    var file = entry as FileInfo;
                    if (file != null)
                    {
                        // Don't include manifest management files in manifest
                        if (file.Name == ".manifest" || file.Name == ".xbit" || file.Name == ".symlink") continue;

                        nodes.Add(GetFileNode(file, Format, externalXbits, externalSymlinks));
                        BytesProcessed += file.Length;
                    }
                    else
                    {
                        var directory = entry as DirectoryInfo;
                        if (directory != null) nodes.Add(GetDirectoryNode(directory, Path.GetFullPath(TargetDir)));
                    }

                    if (CancelRequest.WaitOne(0, false)) throw new OperationCanceledException();
                }

                Result = new Manifest(nodes, Format);
            }
                #region Error handling
            catch (UnauthorizedAccessException ex)
            {
                // Wrap exception since only certain exception types are allowed
                throw new IOException(ex.Message, ex);
            }
            #endregion

            if (CancelRequest.WaitOne(0, false)) throw new OperationCanceledException();
            lock (StateLock) State = TaskState.Complete;
        }

        /// <summary>
        /// Determines the combined size of all files listed in a filesystem-element collection.
        /// </summary>
        /// <returns>The size of all files summed up in bytes.</returns>
        private static long TotalFileSize(IEnumerable<FileSystemInfo> entries)
        {
            long size = 0;
            foreach (var entry in entries)
            {
                var file = entry as FileInfo;
                if (file != null) size += file.Length;
            }
            return size;
        }

        /// <summary>
        /// Creates a manifest node for a file.
        /// </summary>
        /// <param name="file">The file object to create a node for.</param>
        /// <param name="format">The manifest format containing digest rules.</param>
        /// <param name="externalXbits">A list of fully qualified paths of files that are named in the <code>.xbit</code> file.</param>
        /// <param name="externalSymlinks">A list of fully qualified paths of files that are named in the <code>.symlink</code> file.</param>
        /// <returns>The node for the list.</returns>
        /// <exception cref="NotSupportedException">Thrown if the <paramref name="file"/> has illegal properties (e.g. is a device file, has line breaks in the filename, etc.).</exception>
        /// <exception cref="IOException">Thrown if there was an error reading the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if you have insufficient rights to read the file.</exception>
        private static ManifestNode GetFileNode(FileInfo file, ManifestFormat format, ICollection<string> externalXbits, ICollection<string> externalSymlinks)
        {
            // Real symlinks
            string symlinkContents;
            if (FileUtils.IsSymlink(file.FullName, out symlinkContents))
                return new ManifestSymlink(format.DigestString(symlinkContents), symlinkContents.Length, file.Name);

            // External symlinks
            if (externalSymlinks.Contains(file.FullName))
                return new ManifestSymlink(format.DigestFile(file.FullName), file.Length, file.Name);

            // Invalid file typest
            if (!FileUtils.IsRegularFile(file.FullName))
                throw new NotSupportedException(string.Format(Resources.IllegalFileType, file.FullName));

            // Executable files
            if (externalXbits.Contains(file.FullName) || FileUtils.IsExecutable(file.FullName))
                return new ManifestExecutableFile(format.DigestFile(file.FullName), FileUtils.ToUnixTime(file.LastWriteTimeUtc), file.Length, file.Name);

            // Regular files
            return new ManifestNormalFile(format.DigestFile(file.FullName), FileUtils.ToUnixTime(file.LastWriteTimeUtc), file.Length, file.Name);
        }

        /// <summary>
        /// Creates a manifest node for a directory.
        /// </summary>
        /// <param name="directory">The directory object to create a node for.</param>
        /// <param name="rootPath">The fully qualified path of the root directory the manifest is being generated for.</param>
        /// <returns>The node for the list.</returns>
        /// <exception cref="IOException">Thrown if there was an error reading the directory.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown if you have insufficient rights to read the directory.</exception>
        private static ManifestDirectory GetDirectoryNode(DirectoryInfo directory, string rootPath)
        {
            return new ManifestDirectory(
                FileUtils.ToUnixTime(directory.LastWriteTime),
                // Remove leading portion of path and use Unix slashes
                directory.FullName.Substring(rootPath.Length).Replace(Path.DirectorySeparatorChar, '/'));
        }
        #endregion
    }
}
