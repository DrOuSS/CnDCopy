using System;
using System.IO;

namespace CnDCopy.Kernel
{
	public class Copier
	{
		public ILocationFactory LocationFactory { get; set; }
		public Func<ReplaceMode> UserAskHandler { private get; set; } 

		public bool Copy (ILocation sourceLocation, ILocation destinationLocation, ReplaceMode replaceMode)
		{
			var sourceManager = LocationFactory.GetSourceManager ();
			var destinationManager = LocationFactory.GetDestinationManager ();

			var canCopy = false;
			var destinationExists = destinationManager.Exists (destinationLocation);
			if (!destinationExists)
				canCopy = true;

			if (destinationExists && (replaceMode & ReplaceMode.ReplaceIfDifferentSize) == ReplaceMode.ReplaceIfDifferentSize) {
				var sourceSize = sourceManager.GetSize (sourceLocation);
				var destinationSize = destinationManager.GetSize (destinationLocation);

				if (sourceSize != destinationSize)
					canCopy = true;
			}

			if (destinationExists && replaceMode == ReplaceMode.Replace)
				canCopy = true;

			if (destinationExists && replaceMode == ReplaceMode.Rename) {
				destinationLocation.ItemUri = new Uri (Path.Combine (destinationLocation.ItemUri.AbsolutePath,
                    Path.GetFileNameWithoutExtension (destinationLocation.ItemUri.AbsoluteUri) + "_" +
					DateTime.Now.TimeOfDay.ToString ("HHmmss") +
					Path.GetExtension (destinationLocation.ItemUri.AbsoluteUri)));

				canCopy = true;
			}

			if (destinationExists && replaceMode == ReplaceMode.UserAsking) {
				if (UserAskHandler == null)
					throw new Exception ("If the ReplaceMode is ReplaceMode.UserAsking, the UserAskHandler delegate shall be set.");

				replaceMode = UserAskHandler ();
			}


			if (canCopy) {
				using (var pushFile = destinationManager.BeginPush(destinationLocation, replaceMode)) {
					sourceManager.BeginRetreive (sourceLocation, pushFile.BufferWriteCallback, pushFile.CopyDone);

					pushFile.Done.WaitOne ();
				}
			}

			return canCopy;
		}
	}
}
