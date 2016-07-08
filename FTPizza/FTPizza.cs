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
            Console.WriteLine("Enter the url for the FTP Server:");
            string ftpUrl = Console.ReadLine();

            Console.WriteLine("Enter the username for the FTP Server:");
            string ftpUsername = Console.ReadLine();

            Console.WriteLine("Enter the password for the FTP Server:");
            string ftpPassword = Console.ReadLine();

            Console.Clear();

            while (true)
            {
                Ftp client = new Ftp(ftpUsername, ftpPassword, ftpUrl);
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
