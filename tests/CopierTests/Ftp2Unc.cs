using System;
using System.IO;
using CnDCopy.Kernel;
using CnDCopy.Kernel.LocationManagers.Ftp;
using CnDCopy.Kernel.LocationManagers.UNC;
using NUnit.Framework;

namespace CopierTests
{
	[TestFixture]
	public class Ftp2Unc
	{
		[Test]
		public void Ftp2Local ()
		{/*
            var copier = new Copier { LocationFactory = new LocationFactory() };

            var source = new FtpLocation
                             {
                                 ItemUri = new Uri(@"ftp://127.0.0.1/masseffect3.pdf"),
                                 Credentials = new Credentials {Username = "test"}
                             };
            var sourceFtpManager = new FtpManager(source, ReplaceMode.Ignore);

            var destination = new UncLocation { ItemUri = new Uri(@"E:\Temp\FTP\masseffect3-2.pdf") };
            var destinationUncManager = new UncManager(destination, ReplaceMode.Ignore);

            var done = copier.Copy(source, destination, ReplaceMode.Ignore);

            if (done)
            {
                Assert.IsTrue(File.Exists(destination.ItemUri.LocalPath),
                              "File destination.ItemUri.LocalPath shall exist.");

                var sourceSize = sourceFtpManager.GetSize();
                Assert.AreEqual(sourceSize, destinationUncManager.GetSize(),
                                "File length shall be " + sourceSize + " bytes.");

                destinationUncManager.Delete();
            }*/
		}

		[Test]
		public void Ftp2Ftp ()
		{/*
            var copier = new Copier { LocationFactory = new LocationFactory() };

            var source = new FtpLocation
            {
                ItemUri = new Uri(@"ftp://127.0.0.1/masseffect3.pdf"),
                Credentials = new Credentials { Username = "test" }
            };
            var destination = new FtpLocation
            {
                ItemUri = new Uri(@"ftp://127.0.0.1/masseffect3-2.pdf"),
                Credentials = new Credentials { Username = "test" }
            };
            var sourceFtpManager = new FtpManager(source, ReplaceMode.Ignore);
            var destinationFtpManager = new FtpManager(destination, ReplaceMode.Ignore);

            var done = copier.Copy(source, destination, ReplaceMode.Ignore);

            if (done)
            {
                var sourceSize = sourceFtpManager.GetSize();
                var destinationSize = destinationFtpManager.GetSize();
                Assert.AreEqual(sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");
                destinationFtpManager.Delete();
            }*/
		}

	}
}
