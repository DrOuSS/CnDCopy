using System;

namespace CnDCopy.Kernel
{
	public interface ILocation
	{
		Uri ItemUri { get; set; }
		bool IsFolder { get; }
	}
}