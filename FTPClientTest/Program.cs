using System;
using Kefong.Net;

namespace FTPClientTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // config
            string host = "ftp server host/ip";
            int port = 21;
            string username = "ftp server username";
            string password = "ftp server password";

            FTPClient ftp = new FTPClient();
            // connect ftp server
            ftp.connect(host, port);
            // login
            ftp.login(username, password);
            // change directory
            ftp.cwd("change directory name");
            // upload
            ftp.upload("local filename and folder");
            Console.WriteLine("Hello World!");
        }
    }
}
