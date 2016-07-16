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

            fetchCurrentDirectoryItems();
        }

        /// <summary>
        /// Validate user info trying to get responce from server
        /// </summary>
        public bool ValidateUserDestination()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl);
            request.Credentials = new NetworkCredential(_userName, _userPass);

            try
            {
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.KeepAlive = true;
                var response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
                  Console.WriteLine("Fail to connect to ftp server check credentials and try again...");
                  return false;
            }
        }

        public void fetchCurrentDirectoryItems()
        {
            // Connect to ftp server
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl);
            request.Credentials = new NetworkCredential(_userName, _userPass);
            currentDirFiles = new List<string>();

            // Send request for directory files
            try
            {
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.KeepAlive = true;
                var response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine(response.StatusCode);
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
            // Print list of files
            try
            {
                fetchCurrentDirectoryItems();
                foreach (string file in currentDirFiles)
                { 
                    Console.WriteLine(file);
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

            // Read user submitted file names and add to list
            while ((item = Console.ReadLine()) != null)
            {
                item = ParseItem(item);
                if (item != null)
                {
                    downloadList.Add(item);
                }
            }

            listLength = downloadList.Count;

            // Print list of requested files
            for (int i = 0; i < listLength; i++)
            {
                Console.WriteLine("DL: " + downloadList[i]);
            }

            // Download files from ftp server
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
            string item;
            int listLength;
            bool stop = false;

            Console.WriteLine("To upload files, enter one filename per line." +
                "\nWhen you are done, press '^', and then 'Enter'.");

            var uploadList = new List<string>();

            // Read user submitted file names and add to list
            while (stop != true)
            {
                item = Console.ReadLine();
                stop = item.Equals("^");
                //item = ParseItem(item);
                if (stop != true)
                {
                    uploadList.Add(item);
                }
            }

            listLength = uploadList.Count;

            // Print list of requested files
            for (int i = 0; i < listLength; i++)
            {
                Console.WriteLine("UL: " + uploadList[i]);
            }
        }

        public void quit()
        {
            throw new NotImplementedException();
        }

        public void Local()
        {
            try
            {
                string path = Directory.GetCurrentDirectory();
                var localFiles = Directory.GetFiles(path);

                foreach (string file in localFiles)
                {
                    Console.WriteLine(Path.GetFileName(file));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private string ParseItem(string item)
        {
            bool found = false;

            try
            {
                if (!item.Contains("."))
                {
                    throw new Exception("Malformatted file: " + item);
                }

                if (currentDirFiles.Contains(item))
                {
                    Console.WriteLine("FOUND IT!!!");
                    found = true;
                    return item;
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