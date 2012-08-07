using System;
namespace CnDCopy.Kernel.LocationManagers
{
    public interface ILocationManager
    {
        void PushFile(ILocation destination);
        void BeginRetreiveFile(ILocation source, Action<byte[]> bufferCallback, Action copyDone);
    }
}
