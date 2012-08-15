using CnDCopy.Kernel.LocationManagers;
using CnDCopy.Kernel.LocationManagers.Ftp;
using CnDCopy.Kernel.LocationManagers.UNC;

namespace CnDCopy.Kernel
{
    public class LocationFactory : ILocationFactory
    {
        public LocationManagerBase GetManager(ILocation location, ReplaceMode replaceMode)
        {
            if (location.ItemUri.Scheme.StartsWith("ftp"))
                return new FtpManager((FtpLocation)location, replaceMode);

            return new UncManager((UncLocation)location, replaceMode);
        }
    }
}