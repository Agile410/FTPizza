﻿using NUnit.Framework;
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
            List<string> uploadList = new List<string>();

            // Make sure file UnitTestFile.txt exists locally in FTPizza\FTPizza.Tests\bin\Debug before executing tests
            uploadList.Add("UnitTestFile.txt");
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
        }
    }
}
