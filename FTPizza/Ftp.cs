using System;
using System.IO;
using System.Net;

namespace FTPizza
{
    public class Ftp
    {
        private FtpWebResponse response;
        private StreamReader reader;
        public string _userName { get; set; }
        public string _userPass { get; set; }
        public string _userUrl  { get; set; }

        public Ftp(string userName, string userPass, string userUrl)
        {
            _userName = userName;
            _userPass = userPass;
            _userUrl  = userUrl;

            // TODO: Refractor into List
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl);
            request.Credentials = new NetworkCredential(_userName, _userPass);
            try
            {
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.KeepAlive = true;
                response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal void list()
        {
            string line;

            // Print list of directories
            try
            {
                Console.WriteLine(response.StatusCode);
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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