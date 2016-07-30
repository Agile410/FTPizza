using NUnit.Framework;
using System.Collections.Generic;

namespace FTPizza.Tests
{
    [TestFixture]
    public class FTPTests
    {
        [Test]
        public void LoginToFtpWithBadLogin()
        {
            //Needs a more specific exception
            Assert.That(() => new Ftp(null, null, null), Throws.Exception);
        }

        [Test]
        public void SuccessfulLoginIntoFtpServer()
        {
            var client = new Ftp("", "", "speedtest.tele2.net/");

            Assert.That(client, Is.Not.Null);
        }

    }
}
