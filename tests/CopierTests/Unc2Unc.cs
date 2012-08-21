using System;
using System.IO;
using CnDCopy.Kernel;
using CnDCopy.Kernel.LocationManagers.UNC;
using NUnit.Framework;

namespace CopierTests
{
    [TestFixture]
    public class Unc2Unc
    {
        [Test]
        public void Local2Local()
        {
            var copier = new Copier { LocationFactory = new LocationFactory() };

            var source = new UncLocation { ItemUri = new Uri(@"E:\Temp\FTP\masseffect3.pdf") };
            var destination = new UncLocation { ItemUri = new Uri(@"E:\Temp\FTP\masseffect3-2.pdf") };

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
