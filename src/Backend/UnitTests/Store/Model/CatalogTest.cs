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

using System.Collections.Generic;
using FluentAssertions;
using NanoByte.Common.Storage;
using NUnit.Framework;

namespace ZeroInstall.Store.Model
{
    /// <summary>
    /// Contains test methods for <see cref="Catalog"/>.
    /// </summary>
    [TestFixture]
    public class CatalogTest
    {
        #region Helpers
        /// <summary>
        /// Creates a fictive test <see cref="Catalog"/>.
        /// </summary>
        public static Catalog CreateTestCatalog()
        {
            return new Catalog {Feeds = {FeedTest.CreateTestFeed()}};
        }
        #endregion

        /// <summary>
        /// Ensures that <see cref="Catalog.GetFeed"/> and <see cref="Catalog.this"/> correctly find contained <see cref="Feed"/>s.
        /// </summary>
        [Test]
        public void TestGetFeed()
        {
            var catalog = CreateTestCatalog();

            catalog.GetFeed(FeedTest.Test1Uri).Should().Be(FeedTest.CreateTestFeed());
            catalog[FeedTest.Test1Uri].Should().Be(FeedTest.CreateTestFeed());

            catalog.GetFeed(new FeedUri("http://invalid/")).Should().BeNull();
            // ReSharper disable once UnusedVariable
            catalog.Invoking(x => { var _ = x[new FeedUri("http://invalid/")]; }).ShouldThrow<KeyNotFoundException>();
        }

        /// <summary>
        /// Ensures that the class is correctly serialized and deserialized.
        /// </summary>
        [Test]
        public void TestSaveLoad()
        {
            Catalog catalog1 = CreateTestCatalog(), catalog2;
            Assert.That(catalog1, Is.XmlSerializable);
            using (var tempFile = new TemporaryFile("0install-unit-tests"))
            {
                // Write and read file
                catalog1.SaveXml(tempFile);
                catalog2 = XmlStorage.LoadXml<Catalog>(tempFile);
            }

            // Ensure data stayed the same
            catalog2.Should().Be(catalog1, because: "Serialized objects should be equal.");
            catalog2.GetHashCode().Should().Be(catalog1.GetHashCode(), because: "Serialized objects' hashes should be equal.");
            catalog2.Should().NotBeSameAs(catalog1, because: "Serialized objects should not return the same reference.");
        }

        /// <summary>
        /// Ensures that the class can be correctly cloned.
        /// </summary>
        [Test]
        public void TestClone()
        {
            var catalog1 = CreateTestCatalog();
            var catalog2 = catalog1.Clone();

            // Ensure data stayed the same
            catalog2.Should().Be(catalog1, because: "Cloned objects should be equal.");
            catalog2.GetHashCode().Should().Be(catalog1.GetHashCode(), because: "Cloned objects' hashes should be equal.");
            catalog2.Should().NotBeSameAs(catalog1, because: "Cloning should not return the same reference.");
        }

        /// <summary>
        /// Ensures that <see cref="Catalog.FindByShortName"/> works correctly.
        /// </summary>
        [Test]
        public void TestFindByShortName()
        {
            var appA = new Feed
            {
                Uri = FeedTest.Test1Uri, Name = "AppA",
                EntryPoints = {new EntryPoint {Command = Command.NameRun, BinaryName = "BinaryA"}}
            };
            var appB = new Feed
            {
                Uri = FeedTest.Test2Uri, Name = "AppB",
                EntryPoints = {new EntryPoint {Command = Command.NameRun, BinaryName = "BinaryB"}}
            };
            var catalog = new Catalog {Feeds = {appA, appA.Clone(), appB, appB.Clone()}};

            catalog.FindByShortName("").Should().BeNull();
            catalog.FindByShortName("AppA").Should().BeSameAs(appA);
            catalog.FindByShortName("BinaryA").Should().BeSameAs(appA);
            catalog.FindByShortName("AppB").Should().BeSameAs(appB);
            catalog.FindByShortName("BinaryB").Should().BeSameAs(appB);
        }

        /// <summary>
        /// Ensures that <see cref="Catalog.Search"/> works correctly.
        /// </summary>
        [Test]
        public void TestSearch()
        {
            var appA = new Feed {Uri = FeedTest.Test1Uri, Name = "AppA"};
            var appB = new Feed {Uri = FeedTest.Test2Uri, Name = "AppB"};
            var lib = new Feed {Uri = FeedTest.Test3Uri, Name = "Lib"};
            var catalog = new Catalog {Feeds = {appA, appB, lib}};

            catalog.Search("").Should().Equal(appA, appB, lib);
            catalog.Search("App").Should().Equal(appA, appB);
            catalog.Search("AppA").Should().Equal(appA);
            catalog.Search("AppB").Should().Equal(appB);
        }
    }
}
