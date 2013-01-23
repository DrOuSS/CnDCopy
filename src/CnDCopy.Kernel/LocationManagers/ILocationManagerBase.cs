
using System;

namespace CnDCopy.Kernel
{
	public interface ILocationManagerBase : IDisposable
	{
		long GetSize (ILocation location);		
		void Delete (ILocation location);		
		bool Exists (ILocation location);
	}
}

