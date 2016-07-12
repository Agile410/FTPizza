using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace FTPizza
{
    public class Ftp
    {
        public string _userName { get; set; }
        public string _userPass { get; set; }
        public string _userUrl { get; set; }
        public List<string> currentDirFiles;

        public Ftp(string userName, string userPass, string userUrl)
        {
            _userName = userName;
            _userPass = userPass;
            _userUrl = userUrl;
            currentDirFiles = new List<string>();

            // TODO: Refractor into new function that updates when we change DIR
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl);
            request.Credentials = new NetworkCredential(_userName, _userPass);
            try
            {
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.KeepAlive = true;
                var response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);

                string line = reader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    currentDirFiles.Add(line);
                    line = reader.ReadLine();
                }
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
            int listLength;

            Console.WriteLine("To download files, enter one filename per line." +
                "\nWhen you are done, press 'CTRL+z, and then 'Enter'.");

            var downloadList = new List<string>();

            while ((item = Console.ReadLine()) != null)
            {
                item = ParseItem(item);
                if (item != null)
                {
                    downloadList.Add(item);
                }
            }

            listLength = downloadList.Count;

            //test print statements
            for (int i = 0; i < listLength; i++)
            {
                Console.WriteLine("DL: " + downloadList[i]);
            }

            // TODO: Error handling for non-existent file

            for (int i = 0; i < listLength; i++)
            {
                try
                {
                    var request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl + "/"
                                                                          + downloadList[i]);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = false;

                    request.Credentials = new NetworkCredential(_userName, _userPass);

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream writer = new FileStream(downloadList[i], FileMode.Create))
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
                catch (Exception)
                {

                    throw;
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

        private string ParseItem(string item)
        {
            string line;
            bool found = false;

            try
            {
                if (!item.Contains("."))
                {
                    throw new Exception("Malformatted file: " + item);
                }

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(item))
                    {
                        Console.WriteLine("FOUND IT!!!");
                        found = true;
                        return item;
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

            return null;
        }
    }
}