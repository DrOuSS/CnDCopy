using CnDCopy.Kernel.LocationManagers;

namespace CnDCopy.Kernel
{
    public interface ILocationFactory
    {
        LocationManagerBase GetManager(ILocation location, ReplaceMode replaceMode);
    }
}