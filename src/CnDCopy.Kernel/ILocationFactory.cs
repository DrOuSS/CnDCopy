using CnDCopy.Kernel.LocationManagers;

namespace CnDCopy.Kernel
{
	public interface ILocationFactory
	{
		LocationManagerBase GetSourceManager ();
		LocationManagerBase GetDestinationManager ();
	}
}