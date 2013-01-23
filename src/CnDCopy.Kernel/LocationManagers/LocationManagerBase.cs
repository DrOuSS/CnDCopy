using System;

namespace CnDCopy.Kernel.LocationManagers
{
	public abstract class LocationManagerBase
	{
		internal LocationManagerBase (Credentials credentials, ReplaceMode defaultReplaceMode)
		{
			Credentials = credentials;
			DefaultReplaceMode = defaultReplaceMode;
		}

		protected ReplaceMode DefaultReplaceMode { get; set; }
		protected Credentials Credentials { get; private set; }
        
		public abstract long GetSize (ILocation location);

		public abstract void BeginRetreive (ILocation sourceLocation, Action<byte[]> bufferCallback, Action copyDone);

		public PushRequest BeginPush (ILocation destinationLocation)
		{
			return BeginPush (destinationLocation, DefaultReplaceMode); 		
		}
	
		public abstract PushRequest BeginPush (ILocation destinationLocation, ReplaceMode replaceMode);

		public abstract void Delete (ILocation location);

		public abstract bool Exists (ILocation location);
	}

}
