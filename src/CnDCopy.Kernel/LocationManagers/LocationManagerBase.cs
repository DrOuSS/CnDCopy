using System;

namespace CnDCopy.Kernel.LocationManagers
{
    public abstract class LocationManagerBase
    {
        internal LocationManagerBase(ILocation location, ReplaceMode replaceMode)
        {
            Location = location;
            ReplaceMode = replaceMode;
        }

        public ReplaceMode ReplaceMode { get; set; }
        public ILocation Location { get; private set; }
        
        public abstract long GetSize();

        public abstract void BeginRetreiveFile(Action<byte[]> bufferCallback, Action copyDone);

        public abstract PushRequest BeginPushFile();

        public abstract void Delete();

        public abstract bool Exists();
    }

}
