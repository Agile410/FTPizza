using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System;

namespace FTPizza.Tests
{
    [TestFixture]
    public class FTPTests
    {
        public string[] connectionFile = File.ReadAllLines(@"connection.txt");
        public string ftpUrl;
        public string ftpUser;
        public string ftpPassword;
        public Ftp client;

        [SetUp]
        public void Init()
        {
            ftpUrl = connectionFile[0].Replace("url=", "");
            ftpUser = connectionFile[1].Replace("user=", "");
            ftpPassword = connectionFile[2].Replace("pass=", "");
            client = new Ftp(ftpUser, ftpPassword, ftpUrl);
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
            List<string> uploadList = new List<string>();

            // Make sure file UnitTestFile.txt exists locally in FTPizza\FTPizza.Tests\bin\Debug before executing tests
            uploadList.Add("UnitTestFile.txt");    
            client.Put(uploadList);
            client.FetchCurrentRemoteDirectoryItems();

            Assert.That(client.currentRemDirFiles, Has.Member("UnitTestFile.txt"));
        }

        [Test]
        public void SuccessfulDeleteDirectoryOnFtpServer()
        {
            client.CreateDirectory("UnitTestDirectory");

            string toDelete = "UnitTestDirectory";

            client.DeleteDirectory(toDelete);
            client.FetchCurrentRemoteDirectoryItems();

            Assert.That(client.currentRemDirFiles, Has.No.Member("UnitTestDirectory"));
        }

    }
}
