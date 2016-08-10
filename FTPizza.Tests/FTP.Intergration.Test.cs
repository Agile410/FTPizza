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

        [Test]
        public void LoginToFtpWithBadLogin()
        {
            // TODO: Needs a more specific exception
            Assert.That(() => new Ftp(null, null, null), Throws.Exception);
        }

        [Test]
        public void SuccessfulLoginIntoFtpServer()
        {
            var client = new Ftp("", "", "speedtest.tele2.net/");

            Assert.That(client, Is.Not.Null);
        }

        [Test]
        public void SuccessfulPutFileInToFtpServer()
        {
            var upload_client = new Ftp("", "", "speedtest.tele2.net/upload");
            List<string> uploadList = new List<string> { "UnitTestFile.txt" };
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\UnitTestFile.txt";
            System.IO.StreamWriter sw = System.IO.File.AppendText(path);
            sw.Close();

            upload_client.Put(uploadList);

            Assert.That(upload_client.currentRemDirFiles, Has.Member("UnitTestFile.txt"));
        }

        [Test]
        public void UnsuccessfulPutFileInToFtpServer()
        {
            List<string> uploadList = null;

            Assert.That(() => client.Put(uploadList), Throws.Exception);
        }

        [Test]
        public void SuccessfulDeleteFileOnFtpServer()
        {
            var upload_client = new Ftp("", "", "speedtest.tele2.net/upload");
            List<string> uploadList = new List<string> { "UnitTestFileToDelete.txt" };
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\UnitTestFileToDelete.txt";
            System.IO.StreamWriter sw = System.IO.File.AppendText(path);
            sw.Close();

            upload_client.Put(uploadList);

            Assert.That(upload_client.currentRemDirFiles, Has.Member("UnitTestFileToDelete.txt"));

            var deleteList = new List<string> { "UnitTestFileToDelete.txt" };

            client.Delete(deleteList);

            Assert.That(client.currentRemDirFiles, Has.No.Member("UnitTestFileToDelete.txt"));
        }

        [Test]
        public void SuccessfulDeleteDirectoryOnFtpServer()
        {
            client.CreateDirectory("UnitTestDirectory");
            string toDelete = "UnitTestDirectory";

            client.DeleteDirectory(toDelete);

            Assert.That(client.currentRemDirFiles, Has.No.Member("UnitTestDirectory"));
        }

        [Test]
        public void UnsuccessfulDeleteDirectoryOnFtpServer()
        {
            string toDelete = null;

            Assert.That(() => client.DeleteDirectory(toDelete), Throws.Exception);
        }

        [TearDown]
        public void TearDown()
        {
            System.IO.File.Delete("512KB.zip");
            System.IO.File.Delete("UnitTestFile.txt");
        }
    }
}
