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
			LocationManagerBase _manager = new UncManager (null);

			#region ILocationFactory implementation
            public ILocationManagerBase GetSourceManager(ILocation location)
			{
				return _manager;
			}

            public ILocationManagerBase GetDestinationManager(ILocation location)
			{
				return _manager;
			}
			#endregion
		}

		private class LocationFactoryUnc2uncWithImpersonation : ILocationFactory
		{
			#region ILocationFactory implementation
            public ILocationManagerBase GetSourceManager(ILocation location)
			{
				return new UncManager (new Credentials{Username = @"frmitch-ditd999\test1", Password = "a1b2c3D"});
			}

            public ILocationManagerBase GetDestinationManager(ILocation location)
			{
                return new UncManager(new Credentials { Username = @"frmitch-ditd999\test2", Password = "a1b2c3D" });
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

                var sourceSize = locationFactory.GetSourceManager(source).GetSize(source);
                var destinationSize = locationFactory.GetDestinationManager(destination).GetSize(destination);
				Assert.AreEqual (sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");

                locationFactory.GetDestinationManager(destination).Delete(destination);
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

			Assert.IsTrue (done, "Copy must return true.");
			if (done) {
				var path = destination.ItemUri.IsAbsoluteUri ? destination.ItemUri.LocalPath : destination.ItemUri.ToString ();
				Assert.IsTrue (File.Exists (path),
                              "File " + path + " shall exist.");

                var sourceSize = locationFactory.GetSourceManager(source).GetSize(source);
                var destinationSize = locationFactory.GetDestinationManager(destination).GetSize(destination);
				Assert.AreEqual (sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");

                locationFactory.GetDestinationManager(destination).Delete(destination);
			}
		}

		[Test]
		public void Local2LocalWithImpersonification ()
		{
			var locationFactory = new LocationFactoryUnc2uncWithImpersonation();
			var copier = new Copier { LocationFactory = locationFactory };
			
			var source = new UncLocation { ItemUri = new Uri(@"TestSet\github.jpg", UriKind.Relative) };
			var destination = new UncLocation { ItemUri = new Uri(@"TestSet\github-copy.jpg", UriKind.Relative) };
			
			var done = copier.Copy (source, destination, ReplaceMode.Ignore);
			
			Assert.IsTrue (done, "Copy must return true.");
			if (done) {
				var path = destination.ItemUri.IsAbsoluteUri ? destination.ItemUri.LocalPath : destination.ItemUri.ToString ();
				Assert.IsTrue (File.Exists (path),
				               "File " + path + " shall exist.");

                var sourceSize = locationFactory.GetSourceManager(source).GetSize(source);
                var destinationSize = locationFactory.GetDestinationManager(destination).GetSize(destination);
				Assert.AreEqual (sourceSize, destinationSize, "File length shall be " + sourceSize + " bytes.");

                locationFactory.GetDestinationManager(destination).Delete(destination);
			}
		}

	}
}
