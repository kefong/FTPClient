using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kefong.Net
{
    /// <summary>
    /// FTPC lient.
    /// </summary>
    /// <remarks>By Muir 20171116</remarks>
    public class FTPClient
    {
        private int debugging = 0;
        private int timeouttime = 30000;
        private string welcome = null;
        private Socket socketControl = null;
        private Socket socketData = null;
        private int replycode = 0;
        private string replymessage = "";

        public FTPClient()
        {
            
        }

        /// <summary>
        /// Connect the specified host and port.
        /// </summary>
        /// <returns>The connect.</returns>
        /// <param name="host">Host.</param>
        /// <param name="port">Port.</param>
        public void connect(string host, int port = 21){
            this.socketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(host), port);
            try
            {
                this.socketControl.Connect(ep);
                this.welcome = this.readline();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Quit, and close the connection.
        /// </summary>
        public void quit(){
            this.closeDataSocket();

            if(socketControl != null){
                this.sendcmd("QUIT");
                socketControl.Close();
                socketControl = null;
            }
        }

        /// <summary>
        /// Login the specified username and password.
        /// </summary>
        /// <returns>The login.</returns>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public void login(string username = "", string password = ""){
            try
            {
                username = string.IsNullOrWhiteSpace(username) ? "anonymous" : username;
                this.sendcmd("USER " + username);
                this.sendcmd("PASS "+ password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the welcome message from the server.
        /// </summary>
        /// <returns>The getwelcome.</returns>
        public string getwelcome(){
            return this.welcome;
        }

        /// <summary>
        /// make dir
        /// </summary>
        /// <returns>The mkd.</returns>
        /// <param name="dir">Dir.</param>
        public void mkd(string dir){
            try
            {
                this.sendcmd("MKD " + dir);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// remove dir
        /// </summary>
        /// <returns>The rmd.</returns>
        /// <param name="dir">Dir.</param>
        public void rmd(string dir){
            try
            {
                this.sendcmd("RMD "+ dir);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// delete file
        /// </summary>
        /// <returns>The dele.</returns>
        /// <param name="filename">Filename.</param>
        public void dele(string filename){
            try
            {
                this.sendcmd(filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Change directory
        /// </summary>
        /// <returns>The cwd.</returns>
        /// <param name="str">String.</param>
        public void cwd(string str)
        {
            try
            {
                this.sendcmd("CWD " + str);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  PASV mode
        /// </summary>
        public void usepasv()
        {
            this.sendcmd("PASV");
            return;
        }

        /// <summary>
        /// Store a file in binary mode.  A new port is created for you.
        /// </summary>
        public void storbinary()
        {

        }

        public string[] list(string dir = ""){
            return null;

            /*Byte[] buffer = new Byte[512];

            try
            {
                this.createDataSocket();
                sendcmd("LIST " + dir);
                if(!(this.replycode == 150 || this.replycode == 125 || this.replycode == 226)){
                    throw new IOException(replymessage);
                }

                string str = "";
                while (this.socketData.Available >0)
                {
                    System.Threading.Thread.Sleep(50);

                    int bytes = this.socketData.Receive(buffer, buffer.Length, 0);
                    str += Encoding.ASCII.GetString(buffer, 0, bytes);

                    System.Threading.Thread.Sleep(50);

                    if (bytes < buffer.Length)
                    {
                        break;
                    }
                }

                this.closeDataSocket();

                // Console.WriteLine(str);
                string[] separator = { "\r\n" };
                string[] fileList = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }*/
        }

        private void createDataSocket(){
            try
            {
                string ipAddress = "";
                int port = 0;
                string[] pasv;

                this.sendcmd("PASV");
                // 227 Entering Passive Mode (192,168,2,28,233,152)
                pasv = this.replymessage.Substring(this.replymessage.IndexOf('(') + 1, this.replymessage.IndexOf(')') - this.replymessage.IndexOf('(') - 1).Split(',');

                ipAddress = String.Format("{0}.{1}.{2}.{3}", pasv[0], pasv[1], pasv[2], pasv[3]);
                port = (int.Parse(pasv[4]) << 8) + int.Parse(pasv[5]);

                this.socketData = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                this.socketData.Connect(ep);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void closeDataSocket(){
            try
            {
                if(this.socketData != null){
                    if(this.socketData.Connected){
                        this.socketData.Close();
                    }
                    this.socketData = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void download(string source, string target)
        {
            try
            {
                //this.sendcmd("");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool upload(string filename)
        {
            Byte[] buffer = new Byte[512];
            int bytes = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(filename))
                {
                    throw new Exception("filename is required.");
                }

                this.createDataSocket();
                this.sendcmd("STOR "+ Path.GetFileName(filename));
                if(!(this.replycode == 125 || this.replycode == 150)){
                    throw new IOException(this.replymessage);
                }

                FileStream stream = new FileStream(filename, FileMode.Open);
                while((bytes = stream.Read(buffer, 0, buffer.Length)) > 0){
                    this.socketData.Send(buffer, bytes, 0);
                }
                stream.Close();
                this.closeDataSocket();
                this.readline();

                if (!(this.replycode == 226 || this.replycode == 250))
                {
                    throw new IOException(this.replymessage.Substring(4));
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// send cmd
        /// </summary>
        /// <returns>The sendcmd.</returns>
        /// <param name="str">String.</param>
        public void sendcmd(string cmd){
            try
            {
                if (this.debugging == 1)
                    Console.WriteLine("*cmd* {0}", cmd);

                Byte[] cmdBytes = Encoding.ASCII.GetBytes((cmd + "\r\n").ToCharArray());
                socketControl.Send(cmdBytes, cmdBytes.Length, 0);

                string message = this.readline();

                if (this.debugging == 1)
                    Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Reads the line.
        /// </summary>
        /// <returns>The line.</returns>
        private string readline(){
            string str = "";
            Byte[] buffer = new Byte[512];
            int msecspassed = 0;

            try
            {
                while(socketControl.Available < 1){
                    System.Threading.Thread.Sleep(50);
                    msecspassed += 50;
                    if(msecspassed > this.timeouttime){
                        this.quit();
                        Console.WriteLine("Timed out waiting on server to respond.");
                        return null;
                    }
                }

                while (this.socketControl.Available > 0)
                {
                    string message = "";
                    int bytes = socketControl.Receive(buffer, buffer.Length, 0);
                    message = Encoding.ASCII.GetString(buffer, 0, bytes);
                    str += message;

                    /*if (bytes < buffer.Length){
                        break;
                    }*/
                    System.Threading.Thread.Sleep(50);
                }

                char[] seperator = { '\n' };
                string[] mess = str.Split(seperator);

                if(str.Length > 2){
                    str = mess[mess.Length -2];
                }else{
                    str = mess[0];
                }

                this.replymessage = str;
                this.replycode = int.Parse(str.Substring(0, 3));

                if(!str.Substring(3, 1).Equals(" ")){
                    return readline();
                }

                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Set the debugging level.
        /// </summary>
        /// <returns>The setdebug.</returns>
        /// <param name="level">Level.</param>
        public void setdebug(int level = 0){
            this.debugging = level;
        }
    }
}
