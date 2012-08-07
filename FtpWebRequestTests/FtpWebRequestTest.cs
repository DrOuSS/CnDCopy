using System;
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
        private ManualResetEventSlim _mre;

        [Test]
        public void GetFileLength()
        {
            var request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://127.0.0.1/masseffect3.pdf"));
            request.ConnectionGroupName = Guid.NewGuid().ToString();
            request.Credentials = new NetworkCredential("test", (string)null);
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.UseBinary = true;
            request.EnableSsl = false;
            request.KeepAlive = true;
            request.UsePassive = false;
            var response = request.GetResponse();
            var fileLength = response.ContentLength;

            Assert.AreEqual(129781, fileLength, "File length shall be 129 781 bytes.");
        }

        [Test]
        public void GetFileLength2()
        {
            var locationManager = new FtpLocationManager();
            var request = locationManager.CreateRequest(new Uri("ftp://127.0.0.1/masseffect3.pdf"),
                                                        WebRequestMethods.Ftp.GetFileSize,
                                                        new Credentials { Username = "test", Password = null });
            var response = request.GetResponse();
            var fileLength = response.ContentLength;

            Assert.AreEqual(129781, fileLength, "File length shall be 129 781 bytes.");
        }

        [Test]
        public void GetFileDownload()
        {
            _mre = new ManualResetEventSlim(false);
            var locationManager = new FtpLocationManager();
            var location = new FtpLocation
                               {
                                   Credentials = new Credentials{Username = "test"},
                                   ItemUri = new Uri("ftp://127.0.0.1/masseffect3.pdf"),
                               };
            int bytesRead = 0;
            using (var sw = new BinaryWriter(File.Create(@"c:\test.pdf")))
            {
                locationManager.BeginRetreiveFile(location, buffer =>
                                                                {
                                                                    sw.Write(buffer);
                                                                    bytesRead += buffer.Length;
                                                                }, () => _mre.Set());
                _mre.Wait();
                sw.Flush();
                sw.Close();

            }
            Assert.AreEqual(129781, bytesRead);
        }
    }
}
