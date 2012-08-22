using System;
using System.IO;
using System.Reflection;
using CnDCopy.Kernel;
using CnDCopy.Kernel.LocationManagers.UNC;
using NUnit.Framework;

namespace CopierTests
{
    [TestFixture]
    public class Unc2Unc
    {
        [Test]
        public void Local2LocalWithAbsoluteUri()
        {
            var copier = new Copier { LocationFactory = new LocationFactory() };

            var source = new UncLocation { ItemUri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), @"TestSet\github.jpg")) };
            var destination = new UncLocation { ItemUri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), @"TestSet\github-copy.jpg")) };

            var sourceUncManager = new UncManager(source, ReplaceMode.Ignore);
            var destinationUncManager = new UncManager(destination, ReplaceMode.Ignore);

            var done = copier.Copy(source, destination, ReplaceMode.Ignore);

            if (done)
            {
                Assert.IsTrue(File.Exists(destination.ItemUri.LocalPath),
                              "File destination.ItemUri.LocalPath shall exist.");

                var sourceSize = sourceUncManager.GetSize();
                var destinationSize = destinationUncManager.GetSize();
                Assert.AreEqual(sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");

                destinationUncManager.Delete();
            }
        }

        [Test]
        public void Local2LocalWithRelativeUri()
        {
            var copier = new Copier { LocationFactory = new LocationFactory() };

            var source = new UncLocation { ItemUri = new Uri(@"TestSet\github.jpg", UriKind.Relative) };
            var destination = new UncLocation { ItemUri = new Uri(@"TestSet\github-copy.jpg", UriKind.Relative) };

            var sourceUncManager = new UncManager(source, ReplaceMode.Ignore);
            var destinationUncManager = new UncManager(destination, ReplaceMode.Ignore);

            var done = copier.Copy(source, destination, ReplaceMode.Ignore);

            if (done)
            {
                Assert.IsTrue(File.Exists(destination.ItemUri.LocalPath),
                              "File destination.ItemUri.LocalPath shall exist.");

                var sourceSize = sourceUncManager.GetSize();
                var destinationSize = destinationUncManager.GetSize();
                Assert.AreEqual(sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");

                destinationUncManager.Delete();
            }
        }
    }
}
