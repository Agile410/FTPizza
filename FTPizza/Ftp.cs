using System;
using System.ComponentModel;
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
        public string _userUrl { get; set; }

        public Ftp(string userName, string userPass, string userUrl)
        {
            _userName = userName;
            _userPass = userPass;
            _userUrl = userUrl;

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


        public void list()
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

        public void get()
        {
            string item;
            string line;
            bool found = false;
            
            // TODO: Refactor for multiple files.
            Console.WriteLine("Enter the name of file you wish to retrieve:");
            item = Console.ReadLine();

            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if(line.Contains(item)) 
                    {
                        Console.WriteLine("FOUND IT!!!");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new Exception("Unable to locate file: " + item);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            // TODO: Error handling for non-existent file

            // TODO: Parse for multiple files
            //string item = ParseItem();

            var request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl + "/" + item);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.UseBinary = false;

            request.Credentials = new NetworkCredential(_userName, _userPass);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                using (FileStream writer = new FileStream(item, FileMode.Create))
                {
                    byte[] buffer = new byte[2048];
                    int bytesRead = responseStream.Read(buffer, 0, buffer.Length);

                    while (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                        bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        public void put()
        {
            throw new NotImplementedException();
        }

        public void quit()
        {
            throw new NotImplementedException();
        }

        public void Local()
        {
            throw new NotImplementedException();
        }

        private string ParseItem()
        {
            throw new NotImplementedException();
        }
    }
}