
using CnDCopy.Kernel.LocationManagers;

namespace CnDCopy.Kernel
{
	public interface ILocationFactory
	{
		ILocationManagerBase GetSourceManager ();
		ILocationManagerBase GetDestinationManager ();
	}
}