
namespace CnDCopy.Kernel.LocationManagers
{
	public interface IDirectLocationManager : ILocationManagerBase
	{
		void DirectCopy (ILocation sourceLocation, ILocation destinationLocation, ReplaceMode replaceMode);
	}
}

