﻿using System;
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
            fetchCurrentRemoteDirectoryItems();

            foreach (string file in currentRemDirFiles)
            {
                Console.WriteLine(file);
            }
        }

        public void get()
        {
            string item;
            int listLength;
            bool stop = false;

            //Changed "Ctrl+Z" to "^" so that mac users can test (Ctrl+Z stops the program from running)
            //This requires some changes to the logic
            Console.WriteLine("To download files, enter one filename per line." +
                "\nWhen you are done, press '^' and then 'Enter'.");

            var downloadList = new List<string>();

            // Read user submitted file names and add to list
            GetFiles(downloadList);

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
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void put()
        {
            string item;
            int listLength;
            bool stop = false;

            fetchCurrentLocalDirectoryItems();

            //Changed "Ctrl+Z" to "^" so that mac users can test (Ctrl+Z stops the program from running)
            //This requires some changes to the logic
            Console.WriteLine("To upload files, enter one filename per line." +
                "\nWhen you are done, press '^', and then 'Enter'.");

            var uploadList = new List<string>();

            // Read user submitted file names and add to list
            while (stop != true)
            {
                item = Console.ReadLine();
                stop = item.Equals("^");
                if (stop)
                    break;
                item = verifyLocalItem(item);
                uploadList.Add(item);

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

        private string verifyRemoteItem(string item)
        {
            bool found = false;

            try
            {
                if (!item.Contains(".") || item.Equals("^"))
                {
                    throw new Exception("Malformatted file: " + item);
                }

                if (currentRemDirFiles.Contains(item))
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


        private string verifyLocalItem(string item)
        {
            bool found = false;

            try
            {
                if (!item.Contains(".") || item.Equals("^"))
                {
                    throw new Exception("Malformatted file: " + item);
                }

                if (currentLocDirFiles.Contains(item))
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