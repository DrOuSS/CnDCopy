
using CnDCopy.Kernel.LocationManagers;

namespace CnDCopy.Kernel
{
	public interface ILocationFactory
	{
		ILocationManagerBase GetSourceManager (ILocation location);
        ILocationManagerBase GetDestinationManager(ILocation location);
	}
}