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

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + ftpUrl);
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            try
            {
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                Console.WriteLine(reader.ReadToEnd());

                reader.Close();
                response.Close();
            }
            catch (WebException e)
            {

                Console.WriteLine(e.ToString());
            }
           
        }
    }
}
