﻿/*
 * Copyright 2010 Bastian Eicher, Roland Leopold Walkling
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
using Common.Utils;
using NUnit.Framework;
using System.IO;
using Common.Storage;

namespace ZeroInstall.Store.Implementation.Archive
{
    [TestFixture]
    public class TarExtractorTestBasicFunctionality
    {
        private TemporaryDirectory _sandbox;
        private string _oldWorkingDirectory;

        [SetUp]
        public void SetUp()
        {
            _sandbox = new TemporaryDirectory("0install-unit-tests");
            _oldWorkingDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = _sandbox.Path;
        }

        [TearDown]
        public void TearDown()
        {
            Environment.CurrentDirectory = _oldWorkingDirectory;
            _sandbox.Dispose();
        }

        [Test]
        public void TestPlain()
        {
            using (var archive = TestData.GetTestTarArchiveStream())
                TestExtract("application/x-tar", archive);
        }

        [Test]
        public void TestGzCompressed()
        {
            using (var archive = TestData.GetTestTarGzArchiveStream())
                TestExtract("application/x-compressed-tar", archive);
        }

        [Test]
        public void TestBz2Compressed()
        {
            using (var archive = TestData.GetTestTarBz2ArchiveStream())
                TestExtract("application/x-bzip-compressed-tar", archive);
        }

        [Test]
        public void TestLzmaCompressed()
        {
            using (var archive = TestData.GetTestTarLzmaArchiveStream())
                TestExtract("application/x-lzma-compressed-tar", archive);
        }

        [Test]
        public void TestRubyGem()
        {
            using (var archive = TestData.GetTestGemArchiveStream())
                TestExtract("application/x-ruby-gem", archive);
        }

        private void TestExtract(string mimeType, Stream archive)
        {
            using (var extractor = Extractor.CreateExtractor(mimeType, archive, _sandbox.Path))
                extractor.RunSync();

            Assert.IsTrue(File.Exists("subdir1/regular"), "Should extract file 'regular'");
            Assert.AreEqual(new DateTime(2000, 1, 1, 12, 0, 0), File.GetLastWriteTimeUtc("subdir1/regular"), "Correct last write time for file 'regular' should be set");

            Assert.IsTrue(File.Exists("subdir2/executable"), "Should extract file 'executable'");
            Assert.AreEqual(new DateTime(2000, 1, 1, 12, 0, 0), File.GetLastWriteTimeUtc("subdir2/executable"), "Correct last write time for file 'executable' should be set");
        }
    }

    [TestFixture]
    internal class TarExtractorTestCornerCases
    {
        private TemporaryDirectory _sandbox;

        [SetUp]
        public void SetUp()
        {
            _sandbox = new TemporaryDirectory("0install-unit-tests");
        }

        [TearDown]
        public void TearDown()
        {
            _sandbox.Dispose();
        }

        /// <summary>
        /// Tests whether the extractor generates a correct .xbit file for a sample TAR archive containing an executable file.
        /// </summary>
        [Test]
        public void TestExtractUnixArchiveWithExecutable()
        {
            using (var archive = TestData.GetTestTarArchiveStream())
            using (var extractor = new TarExtractor(archive, _sandbox.Path))
                extractor.RunSync();

            if (MonoUtils.IsUnix)
                Assert.IsTrue(FileUtils.IsExecutable(Path.Combine(_sandbox.Path, "subdir2/executable")), "File 'executable' should be marked as exectuable");
            else
            {
                string xbitFileContent = File.ReadAllText(Path.Combine(_sandbox.Path, ".xbit")).Trim();
                Assert.AreEqual("/subdir2/executable", xbitFileContent);
            }
        }

        /// <summary>
        /// Tests whether the extractor generates a correct .symlink file for a sample TAR archive containing a symbolic link.
        /// </summary>
        [Test]
        public void TestExtractUnixArchiveWithSymlink()
        {
            using (var archive = TestData.GetTestTarArchiveStream())
            using (var extractor = new TarExtractor(archive, _sandbox.Path))
                extractor.RunSync();

            string target;
            if (MonoUtils.IsUnix)
                Assert.IsTrue(FileUtils.IsSymlink(Path.Combine(_sandbox.Path, "symlink"), out target));
            else
            {
                string symlinkFileContent = File.ReadAllText(Path.Combine(_sandbox.Path, ".symlink")).Trim();
                Assert.AreEqual("/symlink", symlinkFileContent);
                target = File.ReadAllText(Path.Combine(_sandbox.Path, "symlink"));
            }
            Assert.AreEqual("subdir1/regular", target, "Symlink should point to 'regular'");
        }
    }
}
