using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cmdServer
{
    class Client
    {//自定义一个类，方便统一管理客户端的资源
        public Client(Socket ClientSocket, Thread ClientThread)
        {
            this.ClientSocket = ClientSocket;
            this.ClientThread = ClientThread;
        }
        public Socket ClientSocket { get; set; }
        public Thread ClientThread { get; set; }
    }

    class SocketTest
    {
        public delegate void MyEvent(string ReceiveMsg);//定义一个委托，用来将接收数据传出去
        public event MyEvent HaveYourMsg;
        public delegate void ClientsChangedEvent(List<string> Clients);//当字典变化时将数据传递给窗体，用于实时显示在线客户端
        public event ClientsChangedEvent ClientsChanged;
        private Dictionary<string, Client> DicSocket = new Dictionary<string, Client>();//字典，用来存储客户端socket对象

        //服务端监听
        public bool Listen(IPAddress ip, int port, int MaxClientNum)
        {
            try
            {
                Socket socketwatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(ip, port);
                socketwatch.Bind(endPoint);
                socketwatch.Listen(MaxClientNum);//开始监听，参数表示最大允许几台主机连接我
                new Thread(new ThreadStart(() =>
                {
                    while (true)
                    {
                        Socket ClientSocket = socketwatch.Accept();//如果有客户端连接，就会返回这个客户端的sockeet对象
                        Thread ClientThread = new Thread(new ParameterizedThreadStart(recmsg));
                        ClientThread.IsBackground = true;//设置为后台线程
                        ClientThread.Start(ClientSocket);//把获取到的客户端Socket作为参数传到线程中
                        //把当前客户端的资源放到字典中管理
                        DicSocket.Add(ClientSocket.RemoteEndPoint.ToString(), new Client(ClientSocket, ClientThread));
                        ClientsChanged.Invoke(DicSocket.Keys.ToList());
                    }
                })).Start();
                return true;
            }
            catch
            {
                return false;
            }
        }
        //向客户端发消息
        public void Send(List<string> SendTo, string str)
        {
            try
            {
                byte[] arrsendmsg = Encoding.UTF8.GetBytes(str);
                foreach (string client in SendTo)
                {
                    if (DicSocket[client].ClientSocket.Send(arrsendmsg) != arrsendmsg.Length)
                        HaveYourMsg.Invoke("提示信息：向" + client + "发送失败！！！");
                }
            }
            catch (Exception ex)
            {
                HaveYourMsg.Invoke("警告：发送遇到错误," + ex.Message);
            }
        }

        public void recmsg(object ClientSocket)
        {
            //给线程传参的一种方式，不可以直接传别的类型
            Socket ThisClientSocket = ClientSocket as Socket;
            while (true)
            {
                try
                {
                    byte[] arrserverrecmsg = new byte[1024];
                    int length = ThisClientSocket.Receive(arrserverrecmsg);
                    if (length <= 0)
                        continue;
                    string ReceiveStr = Encoding.UTF8.GetString(arrserverrecmsg, 0, length);
                    //将收到的字符串通过委托事件传给窗体，用于窗体显示
                    HaveYourMsg.Invoke(ThisClientSocket.RemoteEndPoint.ToString() + "：" + ReceiveStr);
                    //直接转发给所有客户端
                    Send(DicSocket.Keys.ToList(), ThisClientSocket.RemoteEndPoint.ToString() + "：" + ReceiveStr);
                }
                catch
                {
                    HaveYourMsg.Invoke("提示信息：" + ThisClientSocket.RemoteEndPoint.ToString() + "的连接已断开！！！");
                    //断开后将字典中维护的客户端移除掉
                    DicSocket.Remove(ThisClientSocket.RemoteEndPoint.ToString());
                    ClientsChanged.Invoke(DicSocket.Keys.ToList());
                    return;
                }
                Thread.Sleep(200);
            }
        }

        public List<string> GetClients()
        {
            return DicSocket.Keys.ToList();
        }

        public void Colse(List<string> CloseWho)
        {
            foreach (string client in CloseWho)
            {
                if (DicSocket[client].ClientThread.ThreadState == ThreadState.Running)
                    DicSocket[client].ClientThread.Abort();
                //如果接收线程正在执行那么先关闭线程在关闭socket
                DicSocket[client].ClientSocket.Close();
            }
        }
    }
}
