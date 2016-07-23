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
                Console.WriteLine("Do you have stored connection info you'd like to use? (y/n):");
                string answer = Console.ReadLine();
                string ftpUrl;
                string ftpUsername;
                string ftpPassword;

                if (answer.Equals("y"))
                {
                    Console.WriteLine("Connecting with saved connection info...");
                    ftpUrl = "a_url";
                    ftpUsername = "a_username";
                    ftpPassword = "a_password";
                }
                else
                {
                    Console.WriteLine("Enter the url for the FTP Server:");
                    ftpUrl = Console.ReadLine();

                    Console.WriteLine("Enter the username for the FTP Server:");
                    ftpUsername = Console.ReadLine();

                    Console.WriteLine("Enter the password for the FTP Server:");
                    ftpPassword = Console.ReadLine();
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