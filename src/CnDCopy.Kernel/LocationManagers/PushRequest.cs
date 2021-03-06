﻿using System;
using System.Threading;

namespace CnDCopy.Kernel.LocationManagers
{
	public abstract class PushRequest : IDisposable
	{
		private volatile bool _isDisposing;

		protected PushRequest ()
		{
			Done = new AutoResetEvent (false);
		}

		public AutoResetEvent Done { get; private set; }

		public abstract void BufferWriteCallback (byte[] buffer);

		public void CopyDone ()
		{
			Done.Set ();
		}

		public void Dispose ()
		{
			lock (this) {
				if (_isDisposing)
					return;

				_isDisposing = true;
			}
			try {
				OnDispose ();
			} finally {
				Done.Dispose ();
			}
		}

		protected virtual void OnDispose ()
		{
		}
	}
}
