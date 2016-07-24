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

                Console.WriteLine("Loading with saved connection info...");
                string[] connection_file = System.IO.File.ReadAllLines(@"connection.txt");

                Console.WriteLine("The following users are available:");
                foreach (string item in connection_file)
                {
                    if (item.Contains("user="))
                    {
                        Console.WriteLine("--> '" + item.Replace("user=", "") + "'");
                    }
                }

                Console.WriteLine("Enter the name of the user you'd like to connect with, \notherwise enter 'create' to create a new user: ");
                string answer = Console.ReadLine();

                if (answer.Equals("create"))
                {
                    Console.WriteLine("Enter the url for the FTP Server:");
                    ftpUrl = Console.ReadLine();

                    Console.WriteLine("Enter the username for the FTP Server:");
                    ftpUsername = Console.ReadLine();

                    Console.WriteLine("Enter the password for the FTP Server:");
                    ftpPassword = Console.ReadLine();
                }
                else
                {
                    int userIndex = 0;
                    foreach (string item in connection_file)
                    {
                        if (item.Contains(answer))
                        {
                            break;
                        }
                        ++userIndex;
                    }
                    ftpUrl = connection_file[userIndex - 1].Replace("url=", "");
                    ftpUsername = connection_file[userIndex].Replace("user=", "");
                    ftpPassword = connection_file[userIndex + 1].Replace("pass=", "");
                }

                //Console.Clear();
                Ftp client = new Ftp(ftpUsername, ftpPassword, ftpUrl);
                if (!client.ValidateUserDestination()) continue;

                while (true)
                {                   
                    Console.WriteLine("Select which operation to perform: ");
                    Console.WriteLine("(L)ist, (LO)cal, (G)et, (P)ut, (Q)uit:");
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