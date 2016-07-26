using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace FTPizza
{
    class ConnectionManager
    {

        private string[] connectionFile;
        private List<string> connectionInfo;
        private string ftpUrl;
        private string ftpUsername;
        private string ftpPassword;
        private int userIndex;
        private string answer;
        private string selectedUser;
        private bool newUser;

        public ConnectionManager()
        {
            Console.WriteLine("/////////////////////////////////////");
            Console.WriteLine("Loading with saved connection info...");
            Console.WriteLine("/////////////////////////////////////");

            try
            {
                connectionFile = File.ReadAllLines(@"connection.txt");
                newUser = false;
            }
            catch (FileNotFoundException)
            {
                //Calls GC on object after creation to allow access other processes
                File.Create("connection.txt").Dispose();
                connectionFile = new string[] { };
                newUser = true;
            }

        }

        public void displayAvailableUsers()
        {
            Console.WriteLine(connectionFile.Length);
            if (connectionFile.Length > 0 && ((connectionFile.Length % 3) != 0))
            {
                System.Diagnostics.Debug.WriteLine("ERROR: connection.txt is corrupt.");
                throw new Exception("ERROR: connection.txt is corrupt.");
            }

            if (!newUser)
            {
                Console.WriteLine("\n/////////////////////////////////////");
                Console.WriteLine("The following users are available:");
                Console.WriteLine();
                foreach (string item in connectionFile)
                {
                    if (item.Contains("user="))
                    {
                        Console.WriteLine("--> '" + item.Replace("user=", "") + "'");
                    }
                }
                Console.WriteLine();
            }
        }

        public void selectUser()
        {
            connectionInfo = new List<string>();

            if(!newUser && connectionFile.Length > 0)
            {
                Console.WriteLine("Enter the name of the user you'd like to connect with, \notherwise enter 'create' to create a new user: ");
                answer = Console.ReadLine();
            }

            foreach (string item in connectionFile)
            {
                connectionInfo.Add(item);
            }

            if (newUser || connectionFile.Length < 3 || answer.Equals("create"))
            {
                Console.WriteLine("/////////////////////////////////////");
                Console.WriteLine("Creating a new user...");
                Console.WriteLine("/////////////////////////////////////");
                Console.WriteLine("Enter the url for the FTP Server:");
                ftpUrl = Console.ReadLine();
                connectionInfo.Add("url=" + ftpUrl);

                Console.WriteLine("Enter the username for the FTP Server:");
                ftpUsername = Console.ReadLine();
                connectionInfo.Add("user=" + ftpUsername);
                selectedUser = ftpUsername;

                Console.WriteLine("Enter the password for the FTP Server:");
                ftpPassword = Console.ReadLine();
                connectionInfo.Add("pass=" + ftpPassword);

            }
            else
            {
                selectedUser = answer;
            }
            selectConnection();  
        } 

        public void selectConnection()
        {
            userIndex = 0;
            foreach (string item in connectionInfo)
            {
                if (item.Contains(selectedUser))
                {
                    break;
                }
                ++userIndex;
            }
        }

        public void saveUserInfo()
        {
            if (newUser || connectionFile.Length < 3 || answer.Equals("create"))
            {
                string[] newConnectionInfo = new string[connectionInfo.Count];
                int index = 0;
                foreach (string item in connectionInfo)
                {
                    newConnectionInfo[index] = item;
                    ++index;
                }
                File.WriteAllLines(@"connection.txt", newConnectionInfo);
            }
            Console.WriteLine("/////////////////////////////////////\n");
        }

        public string getFtpUrl()
        {
            if (newUser || connectionFile.Length < 3 || answer.Equals("create"))
            {
                return ftpUrl; 
            }
            else
            {
                return connectionFile[userIndex - 1].Replace("url=", "");
            }
        }

        public string getFtpUsername()
        {
            if (newUser || connectionFile.Length < 3 || answer.Equals("create"))
            {
                return ftpUsername; 
            }
            else
            {
                return connectionFile[userIndex].Replace("user=", "");
            }
        }

        public string getFtpPassword()
        {
            if (newUser || connectionFile.Length < 3 || answer.Equals("create"))
            {
                return ftpPassword;
            }
            else
            {
                return connectionFile[userIndex + 1].Replace("pass=", "");
            }
        }
    }
}
