using System;

namespace FTPizza
{
    public class Ftp
    {

        public string _userName { get; set; }
        public string _userPass { get; set; }
        public string _userUrl  { get; set; }

        public Ftp(string userName, string userPass, string userUrl)
        {
            _userName = userName;
            _userPass = userPass;
            _userUrl = userUrl;
        }

        internal void list()
        {
            throw new NotImplementedException();
        }

        internal void get()
        {
            throw new NotImplementedException();
        }

        internal void put()
        {
            throw new NotImplementedException();
        }

        internal void quit()
        {
            throw new NotImplementedException();
        }
    }
}