using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FTPizza
{
    class FTPizza
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string ftpUrl;
                string ftpUsername;
                string ftpPassword;

                Console.WriteLine("/////////////////////////////////////");
                Console.WriteLine("Loading with saved connection info...");
                Console.WriteLine("/////////////////////////////////////");
                string[] connectionFile = System.IO.File.ReadAllLines(@"connection.txt");
                List<string> connectionInfo = new List<string>();

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

                Console.WriteLine("Enter the name of the user you'd like to connect with, \notherwise enter 'create' to create a new user: ");
                string answer = Console.ReadLine();

                if (answer.Equals("create"))
                {
                    foreach (string item in connectionFile)
                    {
                        connectionInfo.Add(item);
                    }

                    Console.WriteLine("/////////////////////////////////////");
                    Console.WriteLine("Creating a new user...");
                    Console.WriteLine("/////////////////////////////////////");
                    Console.WriteLine("Enter the url for the FTP Server:");
                    ftpUrl = Console.ReadLine();
                    connectionInfo.Add("url=" + ftpUrl);

                    Console.WriteLine("Enter the username for the FTP Server:");
                    ftpUsername = Console.ReadLine();
                    connectionInfo.Add("user=" + ftpUsername);

                    Console.WriteLine("Enter the password for the FTP Server:");
                    ftpPassword = Console.ReadLine();
                    connectionInfo.Add("pass=" + ftpPassword);
                    
                }
                else
                {
                    int userIndex = 0;
                    foreach (string item in connectionFile)
                    {
                        if (item.Contains(answer))
                        {
                            break;
                        }
                        ++userIndex;
                    }
                    ftpUrl = connectionFile[userIndex - 1].Replace("url=", "");
                    ftpUsername = connectionFile[userIndex].Replace("user=", "");
                    ftpPassword = connectionFile[userIndex + 1].Replace("pass=", "");
               
                }  

                //Console.Clear();
                Ftp client = new Ftp(ftpUsername, ftpPassword, ftpUrl);
                if (!client.ValidateUserDestination()) continue;

                if (answer.Equals("create"))
                {
                    string[] newConnectionInfo = new string[connectionInfo.Count];
                    int index = 0;
                    foreach (string item in connectionInfo)
                    {
                        newConnectionInfo[index] = item;
                        ++index;
                    }
                    System.IO.File.WriteAllLines(@"connection.txt", newConnectionInfo);
                }
                Console.WriteLine("/////////////////////////////////////\n");

                while (true)
                {
                    Console.WriteLine("/////////////////////////////////////");
                    Console.WriteLine("Select which operation to perform: ");
                    Console.WriteLine("(L)ist, (LO)cal, (G)et, (P)ut, (Q)uit:");
                    Console.WriteLine("/////////////////////////////////////");
                    string input = Console.ReadLine();

                    switch (input.ToLower())
                    {
                        case "l":
                            client.list();
                            break;
                        case "lo":
                            client.Local();
                            break;
                        case "g":
                            client.get();
                            break;
                        case "p":
                            client.put();
                            break;
                        case "q":
                            client.quit();
                            break;
                        default:
                            client.quit();
                            break;
                   }
                }
            }
        }
    }
}