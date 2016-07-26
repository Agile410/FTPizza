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
                ConnectionManager connection = new ConnectionManager();

                try
                {
                    connection.displayAvailableUsers();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }

                connection.selectUser();

                ftpUrl = connection.getFtpUrl();
                ftpUsername = connection.getFtpUsername();
                ftpPassword = connection.getFtpPassword();

                //Console.Clear();
                Ftp client = new Ftp(ftpUsername, ftpPassword, ftpUrl);
                Console.Clear();
                if (!client.ValidateUserDestination()) continue;

                connection.saveUserInfo();

                while (true)
                {
                    Console.WriteLine("/////////////////////////////////////");
                    Console.WriteLine("Select which operation to perform: ");
                    Console.WriteLine("(L)ist, (LO)cal, (G)et, (P)ut, (D)elete, (Q)uit:");
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
                        case "d":
                            client.delete();
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