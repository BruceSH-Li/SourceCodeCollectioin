using MXY_Chat.Client.Helpers;
using MXY_Chat.Client.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace MXY_Chat.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        // 要知道服务器的ip与端口
        String serverIp = ServerInfo.serverIp;
        int port = ServerInfo.port;
        // 开启客户端socket
        Socket clientSocket;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IPAddress ip = IPAddress.Parse(serverIp);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(ip, port); // 要链接的端点
            try
            {
                clientSocket.Connect(endPoint);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // 连接成功就发送一条消息给服务器端
            var msgInfo = new MessageInfo<CheckNetwork>();
            msgInfo.type = 1; // 新客户端连接信息的type == 1
            msgInfo.info = new CheckNetwork() { text = "有一个新的客户端连接成功" };
            string message = JsonHelp.JsonSerializer<MessageInfo<CheckNetwork>>(msgInfo);
            clientSocket.Send(Encoding.Unicode.GetBytes(message));
            socketHelper.clientSocket = clientSocket;
        }
    }
}
