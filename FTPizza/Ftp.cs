using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;

namespace FTPizza
{
    public class Ftp
    {
        public string _userName { get; set; }
        public string _userPass { get; set; }
        public string _userUrl { get; set; }
        private List<string> currentRemDirFiles;
        private List<string> currentLocDirFiles;

        public Ftp(string userName, string userPass, string userUrl)
        {
            _userName = userName;
            _userPass = userPass;
            _userUrl = userUrl;

            fetchCurrentRemoteDirectoryItems();
            fetchCurrentLocalDirectoryItems();
        }

        /// <summary>
        /// Validate user info trying to get response from server
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
                Console.WriteLine("Fail to connect to ftp server check credentials and try again...");
                return false;
            }
        }

        public void fetchCurrentRemoteDirectoryItems()
        {
            // Connect to ftp server
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl);
            request.Credentials = new NetworkCredential(_userName, _userPass);
            currentRemDirFiles = new List<string>();

            // Send request for directory files
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
                    currentRemDirFiles.Add(line);
                    line = reader.ReadLine();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void fetchCurrentLocalDirectoryItems()
        {
            currentLocDirFiles = new List<string>();

            try
            {
                string path = Directory.GetCurrentDirectory();
                var localFiles = Directory.GetFiles(path);

                foreach (string file in localFiles)
                {
                    currentLocDirFiles.Add(Path.GetFileName(file));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public void list()
        {
            // Print list of files
            try
            {
                fetchCurrentRemoteDirectoryItems();
                foreach (string file in currentRemDirFiles)
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
            Console.WriteLine("To download files, enter one filename per line." +
                "\nWhen you are done, press '^' and then 'Enter'.");

            var downloadList = new List<string>();

            // Read user submitted file names and add to list
            GetFiles(downloadList, currentRemDirFiles);

            // Print list of requested files
            foreach (string file in downloadList)
            {
                Console.WriteLine("DL: " + file);
            }

            // Download files from ftp server
            foreach (string remoteFile in downloadList)
            {
                try
                {
                    var request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl + "/"
                                                                   + remoteFile);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = false;

                    request.Credentials = new NetworkCredential(_userName, _userPass);

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream writer = new FileStream(remoteFile, FileMode.Create))
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
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void put()
        {
            Console.WriteLine("To upload files, enter one filename per line." +
                "\nWhen you are done, press '^' and then 'Enter'.");
            var uploadList = new List<string>();

            // Read user submitted file names and add to list
            GetFiles(uploadList, currentLocDirFiles);

            // Print list of requested files
            foreach (string file in uploadList)
            {
                Console.WriteLine("UL: " + file);
            }

            foreach (string file in uploadList)
            {
                var request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl + "/" + file);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(_userName, _userPass);

                StreamReader sourceStream = new StreamReader(file);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
        }

        //TODO Move verify item into here
        private void GetFiles(ICollection<string> requestList, ICollection<string> DirList)
        {
            string input = Console.ReadLine();
            while (input != "^")
            {
                if (verifyItem(input, DirList))
                {
                    requestList.Add(input);
                }
                input = Console.ReadLine();

            }
        }

        public void quit()
        {
            //Nothing to clean up. Quit console.
            Environment.Exit(0);
        }

        public void Local()
        {
            try
            {
                fetchCurrentLocalDirectoryItems();
                foreach (string file in currentLocDirFiles)
                {
                    Console.WriteLine(file);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool verifyItem(string item, ICollection<string> list)
        {
            bool found = false;

            if (!item.Contains(".") || item.Equals("^"))
            {
                Console.WriteLine("Malformatted file: " + item);
            }

            else if (list.Contains(item))
            {
                Console.WriteLine("FOUND IT!!!");
                found = true;
            }

            else
            {
                Console.WriteLine("Unable to locate file: " + item);
            }

            return found;
        }
    }
}
