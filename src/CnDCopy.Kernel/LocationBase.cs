using System;

namespace CnDCopy.Kernel
{
    public abstract class LocationBase : ILocation
    {
        public Uri ItemUri { get; set; }
        public bool IsFolder { get; protected set; }
        public Credentials Credentials { get; set; }
    }
}
