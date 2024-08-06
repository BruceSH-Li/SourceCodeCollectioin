using MXY_Chat.Client.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace MXY_Chat.Client.Helpers
{
    public static class socketHelper
    {
        // 要知道服务器的ip与端口
        public static String serverIp = ServerInfo.serverIp;
        public static int port = ServerInfo.port;
        static IPAddress ip = IPAddress.Parse(serverIp);
        public static Socket clientSocket;
        static IPEndPoint endPoint = new IPEndPoint(ip, port); // 要链接的端点

        static socketHelper()
        {
            //Init();
        }

        private static void Init()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   
            IPAddress ip = IPAddress.Parse(serverIp);
            IPEndPoint endPoint = new IPEndPoint(ip, port); // 要链接的端点
            clientSocket.Connect(endPoint);
        }
        public static int SendMessage(string msg)
        {
            //Init();
            return clientSocket.Send(Encoding.Unicode.GetBytes(msg));
        }

        /// <summary>
        /// 客户端接收服务器返回的信息
        /// </summary>
        /// <r eturns></returns>
        public static String ReceiveMessage()
        {
            //Init();
            byte[] receive = new byte [1024];
            string message = string.Empty;
            int msgLength = clientSocket.Receive(receive, receive.Length, 0);
            message += Encoding.Unicode.GetString(receive, 0, msgLength);

            //clientSocket.Close();
            return message;
        }
    }
}
 