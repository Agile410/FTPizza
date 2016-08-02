﻿using System;
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
                Console.WriteLine("ERROR: Failed to connect to ftp server. Check credentials and try again...");
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

        public void Get()
        {
            Console.WriteLine("To download files, enter one filename per line." +
                "\nWhen you are done, press '^' and then 'Enter'.");

            var requestedList = new List<string>();

            // Read user submitted file names and add to list
            GetFiles(requestedList, currentRemDirFiles);

            // Print list of requested files
            foreach (string file in requestedList)
            {
                Console.WriteLine("Download List: " + file);
            }
   
            DownloadUsingFtp(requestedList);
        }

        public void DownloadUsingFtp(ICollection<string> requestedFiles)
        {
            foreach (string file in requestedFiles)
            {
                WebClient request = new WebClient();
                request.Credentials = new NetworkCredential(_userName, _userPass);

                byte[] fileData = request.DownloadData("ftp://" + _userUrl + "/" + file);
                FileStream writer = new FileStream(file, FileMode.Create);
                writer.Write(fileData, 0, fileData.Length); 
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
                Console.WriteLine("Upload List: " + file);
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

        public void delete()
        {
            Console.WriteLine("To delete files, enter one filename per line." +
                           "\nWhen you are done, press '^' and then 'Enter'.");
            var deleteList = new List<string>();

            // Read user submitted file names and add to list
            GetFiles(deleteList, currentRemDirFiles);

            // Print list of requested files
            foreach (string file in deleteList)
            {
                Console.WriteLine("Delete List: " + file);
            }

            foreach (string file in deleteList)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl + "/" + file);
                request.Credentials = new NetworkCredential(_userName, _userPass);

                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
        }

        public void create_directory()
        {
            Console.WriteLine("To create a directory: enter a directory name and then press 'Enter'.");

            string input = Console.ReadLine();

            if (verifyItem(input, currentRemDirFiles))
            {
                Console.WriteLine("ERROR: " + input + " already exists.");
                return;
            }
  
            try
            {
                var request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl + "/" + input);
                request.Credentials = new NetworkCredential(_userName, _userPass);

                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void delete_directory()
        {
            Console.WriteLine("To delete a directory: enter the name of an existing directory, and then press 'Enter'.");

            string input = Console.ReadLine();

            if (!verifyItem(input, currentRemDirFiles))
            {
                Console.WriteLine("ERROR: " + input + " does not exist.");
                return;
            }
            Console.WriteLine("Deleting directory: " + input);
            try
            {
                var request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl + "/" + input);
                request.Credentials = new NetworkCredential(_userName, _userPass);

                request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public void quit()
        {
            //Create a request object to ensure keepalive is false
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + _userUrl);
            request.Credentials = new NetworkCredential(_userName, _userPass);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            var response = request.GetResponse();
            response.Close();
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
       
            if (list.Contains(item))
            {
                Console.WriteLine("FOUND IT!!!");
                found = true;
            }

            else
            {
                Console.WriteLine("Unable to locate item: " + item);
            }

            return found;
        }
    }
}
