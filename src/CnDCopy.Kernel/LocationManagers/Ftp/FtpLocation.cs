using System;

namespace CnDCopy.Kernel.LocationManagers.Ftp
{
    public class FtpLocation : ILocation
    {
        public Uri ItemUri { get; set; }
        public bool IsFolder { get; private set; }
        public Credentials Credentials { get; set; }
    }
}
