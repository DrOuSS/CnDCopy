namespace CnDCopy.Kernel.LocationManagers
{
	public abstract class LocationManagerBase : ILocationManagerBase
	{
		private volatile bool _isDisposed;

		protected LocationManagerBase (Credentials credentials)
		{
			Credentials = credentials;
		}

		public void Dispose ()
		{
			lock (this) {
				if (_isDisposed)
					return;

				_isDisposed = true;
			}

			Dispose (true);
		}

		protected abstract void Dispose (bool disposing);

		protected Credentials Credentials { get; private set; }
        
		public abstract long GetSize (ILocation location);

		public abstract void Delete (ILocation location);

		public abstract bool Exists (ILocation location);
	}

}
