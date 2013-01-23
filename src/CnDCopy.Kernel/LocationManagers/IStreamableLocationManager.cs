using System;

namespace CnDCopy.Kernel.LocationManagers
{
	public interface IStreamableLocationManager : ILocationManagerBase
	{
		void BeginRetreive (ILocation sourceLocation, Action<byte[]> bufferCallback, Action copyDone);	
		PushRequest BeginPush (ILocation destinationLocation, ReplaceMode replaceMode);
	}
}

