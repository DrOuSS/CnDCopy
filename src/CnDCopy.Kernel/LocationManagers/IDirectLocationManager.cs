
namespace CnDCopy.Kernel
{
	public interface IDirectLocationManager : ILocationManagerBase
	{
		void DirectCopy (ILocation sourceLocation, ILocation destinationLocation, ReplaceMode replaceMode);
	}
}

