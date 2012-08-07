using System;

namespace CnDCopy.Kernel.LocationManagers.UNC
{
    public class UncLocation : ILocation
    {
        public Uri ItemUri { get; set; }
        public bool IsFolder { get; private set; }
        public Credentials Credentials { get; set; }
    }
}
