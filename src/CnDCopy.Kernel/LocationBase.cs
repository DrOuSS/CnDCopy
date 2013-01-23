using System;

namespace CnDCopy.Kernel
{
    public abstract class LocationBase : ILocation
    {
        public Uri ItemUri { get; set; }
        public bool IsFolder { get; protected set; }
    }
}
