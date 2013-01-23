using System;
using CnDCopy.Kernel.LocationManagers;

namespace CnDCopy.Kernel
{
	public interface IStreamableLocationManager : ILocationManagerBase
	{
		void BeginRetreive (ILocation sourceLocation, Action<byte[]> bufferCallback, Action copyDone);	
		PushRequest BeginPush (ILocation destinationLocation, ReplaceMode replaceMode);
	}
}

