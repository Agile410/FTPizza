using NUnit.Framework;
using System.Collections.Generic;
namespace FTPizza.Tests
{
    [TestFixture]
    public class FTPIntegration
    {
        Ftp client;

        [SetUp]
        public void Setup()
        {
            var dir = System.IO.Path.GetDirectoryName(typeof(FTPIntegration).Assembly.Location);
            System.Environment.CurrentDirectory = dir;
            client = new Ftp("", "", "speedtest.tele2.net/");
        }

        [Test]
        public void ShouldDownloadAFIle()
        {
            //Arrange
            var testRequestedFiles = new List<string> { "512KB.zip" };

            //Act
            client.DownloadUsingFtp(testRequestedFiles);

            //Assert
            Assert.IsTrue(System.IO.File.Exists("512KB.zip"));
        }

        [TearDown]
        public void TearDown()
        {
            System.IO.File.Delete("512KB.zip");
        }
    }
}
