using System;
using System.IO;
using CnDCopy.Kernel.LocationManagers;

namespace CnDCopy.Kernel
{
	public class Copier
	{
		public ILocationFactory LocationFactory { get; set; }


		public bool Copy (ILocation sourceLocation, ILocation destinationLocation, ReplaceMode replaceMode)
		{
			if (replaceMode == ReplaceMode.UserAsking)
				throw new Exception ("Use Copy(ILocation,ILocation,Func<ReplaceMode>) call.");

			return Copy (sourceLocation, destinationLocation, replaceMode, null);
		}

		public bool Copy (ILocation sourceLocation, ILocation destinationLocation, Func<ReplaceMode> userAskHandler)
		{
			if (userAskHandler == null)
				throw new Exception ("Use Copy(ILocation,ILocation, ReplaceMode) call.");

			return Copy (sourceLocation, destinationLocation, ReplaceMode.UserAsking, userAskHandler);
		}

		private bool Copy (ILocation sourceLocation, ILocation destinationLocation, ReplaceMode replaceMode, Func<ReplaceMode> userAskHandler = null)
		{
			//
			// Check location manager inheritance contract conflict
			//
            var sourceManager = LocationFactory.GetSourceManager(sourceLocation);
			if (sourceManager is IStreamableLocationManager && sourceManager is IDirectLocationManager)
				throw new Exception (sourceManager.GetType () + " cannot inherit from IStreamableLocationManager and IDirectLocationManager");

            var destinationManager = LocationFactory.GetDestinationManager(destinationLocation);
			if (destinationManager is IStreamableLocationManager && destinationManager is IDirectLocationManager)
				throw new Exception (destinationManager.GetType () + " cannot inherit from IStreamableLocationManager and IDirectLocationManager");

			//
			// Check destination existance
			//
			var canCopy = false;
			var destinationExists = destinationManager.Exists (destinationLocation);
			if (!destinationExists)
				canCopy = true;

			//
			// Check if destination must be replaced
			//

			if (destinationExists && replaceMode == ReplaceMode.UserAsking) {
				if (userAskHandler == null)
					throw new Exception ("If the ReplaceMode is ReplaceMode.UserAsking, the UserAskHandler delegate shall be set.");
				
				replaceMode = userAskHandler ();
			}

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


			if (canCopy) {

				if (sourceManager is IStreamableLocationManager && destinationManager is IStreamableLocationManager) {
					var streamableSourceManager = (IStreamableLocationManager)sourceManager;
					var streamableDestinationManager = (IStreamableLocationManager)destinationManager;

					using (var pushFile = streamableDestinationManager.BeginPush(destinationLocation, replaceMode)) {
						streamableSourceManager.BeginRetreive (sourceLocation, pushFile.BufferWriteCallback, pushFile.CopyDone);

						pushFile.Done.WaitOne ();
					}
				} else if (destinationManager is IDirectLocationManager) {
					var directDestinationManager = (IDirectLocationManager)destinationManager;
					directDestinationManager.DirectCopy (sourceLocation, destinationLocation, replaceMode);
				} else
					throw new NotImplementedException ();
			}

			return canCopy;
		}
	}
}
