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
        private List<string> userList;
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
                //Load data from file to List connectionInfo and userList
                GetUserList();
                foreach(string item in userList)
                {
                    Console.WriteLine("--> '" + item + "'");
                }
                Console.WriteLine();
            }
        }

        private void GetUserList()
        {
            connectionInfo = new List<string>();
            userList = new List<string>();

            foreach (string item in connectionFile)
            {
                connectionInfo.Add(item);
                if (item.Contains("user="))
                {
                    //Update user list
                    userList.Add(item.Replace("user=", ""));
                }
            }
        }

        private void RequstUserName()
        {
            Console.WriteLine("Enter the name of the user you'd like to connect with, \notherwise enter 'create' to create a new user: ");
            answer = Console.ReadLine();
        }

        public void selectUser()
        {

            if (!newUser && connectionFile.Length > 0)
            {
                RequstUserName();
            }

            if (!userList.Contains(answer) && !answer.Equals("create"))
            {
                Console.WriteLine("Error: --->>> User name "+ answer + " not found! Type 'Create' to make new user!");
                Console.WriteLine("---------------------------------------------------------");
                selectUser();
            }
            else if (newUser || connectionFile.Length < 3 || answer.Equals("create"))
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
                ///Check if user exist. If true ask user again to type once more
                while (userList.Contains(ftpUsername))
                {
                    Console.WriteLine("Error --->>> User with this name already exist!!! Try again...");
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Enter the username for the FTP Server:");
                    ftpUsername = Console.ReadLine();
                }
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
