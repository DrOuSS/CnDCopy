using System;
using System.IO;
using CnDCopy.Kernel;
using CnDCopy.Kernel.LocationManagers;
using NUnit.Framework;
using CnDCopy.Kernel.LocationManagers.UNC;

namespace CopierTests
{
	[TestFixture]
	public class Unc2Unc
	{
		private class LocationFactoryUnc2unc : ILocationFactory
		{
			LocationManagerBase _manager = new UncManager (null, ReplaceMode.Ignore);

			#region ILocationFactory implementation
			public LocationManagerBase GetSourceManager ()
			{
				return _manager;
			}

			public LocationManagerBase GetDestinationManager ()
			{
				return _manager;
			}
			#endregion
		}

		[Test]
		public void Local2LocalWithAbsoluteUri ()
		{
			var locationFactory = new LocationFactoryUnc2unc ();
			var copier = new Copier { LocationFactory = locationFactory };

			var source = new UncLocation { ItemUri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), @"TestSet\github.jpg")) };
			var destination = new UncLocation { ItemUri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), @"TestSet\github-copy.jpg")) };


			var done = copier.Copy (source, destination, ReplaceMode.Ignore);

			if (done) {
				Assert.IsTrue (File.Exists (destination.ItemUri.LocalPath),
                              "File destination.ItemUri.LocalPath shall exist.");

				var sourceSize = locationFactory.GetSourceManager ().GetSize (source);
				var destinationSize = locationFactory.GetDestinationManager ().GetSize (destination);
				Assert.AreEqual (sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");

				locationFactory.GetDestinationManager ().Delete (destination);
			}
		}

		[Test]
		public void Local2LocalWithRelativeUri ()
		{
			var locationFactory = new LocationFactoryUnc2unc ();
			var copier = new Copier { LocationFactory = locationFactory };

			var source = new UncLocation { ItemUri = new Uri(@"TestSet\github.jpg", UriKind.Relative) };
			var destination = new UncLocation { ItemUri = new Uri(@"TestSet\github-copy.jpg", UriKind.Relative) };

			var done = copier.Copy (source, destination, ReplaceMode.Ignore);

            Assert.IsTrue(done, "Copy must return true.");
			if (done) {
                var path = destination.ItemUri.IsAbsoluteUri ? destination.ItemUri.LocalPath : destination.ItemUri.ToString();
                Assert.IsTrue(File.Exists(path),
                              "File " + path + " shall exist.");

				var sourceSize = locationFactory.GetSourceManager ().GetSize (source);
				var destinationSize = locationFactory.GetDestinationManager ().GetSize (destination);
				Assert.AreEqual (sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");

				locationFactory.GetDestinationManager ().Delete (destination);
			}
		}
	}
}
