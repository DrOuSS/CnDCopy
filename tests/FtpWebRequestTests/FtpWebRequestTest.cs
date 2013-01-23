using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using CnDCopy.Kernel;
using CnDCopy.Kernel.LocationManagers.Ftp;
using NUnit.Framework;

namespace FtpWebRequestTests
{
	[TestFixture]
	public class FtpWebRequestTest
	{
		private class FtpManagerTest : FtpManager
		{
			public FtpManagerTest () : base(null)
			{
			}

			public new FtpWebRequest CreateRequest (Uri uri, string methodRequest, Credentials credentials)
			{
				return base.CreateRequest (uri, methodRequest, credentials);
			}
		}

		private ManualResetEventSlim _mre;

		[Test]
		public void GetFileLength ()
		{
			var request = (FtpWebRequest)WebRequest.Create (new Uri ("ftp://127.0.0.1/masseffect3.pdf"));
			request.ConnectionGroupName = Guid.NewGuid ().ToString ();
			request.Credentials = new NetworkCredential ("test", (string)null);
			request.Method = WebRequestMethods.Ftp.GetFileSize;
			request.UseBinary = true;
			request.EnableSsl = false;
			request.KeepAlive = true;
			request.UsePassive = false;
			var response = request.GetResponse ();
			var fileLength = response.ContentLength;

			Assert.AreEqual (129781, fileLength, "File length shall be 129 781 bytes.");
		}

		[Test]
		public void GetFileLength2 ()
		{
			var locationManager = new FtpManagerTest ();
			var request = locationManager.CreateRequest (new Uri ("ftp://127.0.0.1/masseffect3.pdf"),
                                                        WebRequestMethods.Ftp.GetFileSize,
                                                        new Credentials { Username = "test", Password = null });
			var response = request.GetResponse ();
			var fileLength = response.ContentLength;

			Assert.AreEqual (129781, fileLength, "File length shall be 129 781 bytes.");
		}

		[Test]
		public void GetFileDownload ()
		{
			_mre = new ManualResetEventSlim (false);
			var location = new FtpLocation
                               {
                                   ItemUri = new Uri("ftp://127.0.0.1/masseffect3.pdf"),
                               };
			var locationManager = new FtpManagerTest ();
			int bytesRead = 0;
			using (var sw = new BinaryWriter(File.Create(@"c:\test.pdf"))) {
				locationManager.BeginRetreive (location, buffer =>
				{
					Debug.Assert (sw != null, "sw != null");
					sw.Write (buffer);
					bytesRead += buffer.Length;
				}, () => _mre.Set ());
				_mre.Wait ();
				sw.Flush ();
				sw.Close ();

			}
			Assert.AreEqual (129781, bytesRead);
		}
	}
}
