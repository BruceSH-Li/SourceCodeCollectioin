using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace cmdServer
{
    internal class Program
    {
        static int port = 8888;
        static int clientNum = 0;
        static Socket receiveSocket;
        static Thread serverThread;
        static byte[] buffer = new byte[65535];

        static void Main(string[] args)
        {
            // 开启监听socket
            receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            receiveSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            receiveSocket.Listen(20); // 最多接受20个客户端的排队请求
            Console.WriteLine("服务器已开启");

            // 开启服务器接收请求的线程
            serverThread = new Thread(RecieveAccept);
            serverThread.Start();

            #region 旧服务器逻辑
            //while (true)
            //{
            //    // 接受客户端连接请求
            //    Socket socket = receiveSocket.Accept();
            //    clientNum++;
            //    // 接受一条消息
            //    byte[] receiveByte = new byte[2048];
            //    int msgLength = socket.Receive(receiveByte, 0, receiveByte.Length, SocketFlags.None);
            //    byte[] validData = new byte[msgLength];
            //    Array.Copy(receiveByte, validData, msgLength);
            //    string message = Encoding.Unicode.GetString(validData); // 得到json字符串
            //    // 先进行第一次反序列化
            //    var messageInfo = JsonHelp.JsonDeserialize<MessageInfo<object>>(message);
            //    if (messageInfo.type == 1) // 接收到新客户端连接消息
            //    {
            //        var checkNetworkMessage = JsonHelp.JsonDeserialize<MessageInfo<CheckNetwork>>(message);
            //        Console.WriteLine("有新客户端连接成功：" + checkNetworkMessage.info.text);
            //    }
            //    else if (messageInfo.type == 2) // 接收到注册消息
            //    {
            //        var userRegisterMessage = JsonHelp.JsonDeserialize<MessageInfo<UserRegister>>(message);
            //        using (var qqEntities = new QQEntities())
            //        {
            //            // 新账号为已有最大账号 + 1
            //            var number = "10001";
            //            var maxNumberUser = qqEntities.UserSet.OrderByDescending(m => m.Number).Take(1).FirstOrDefault();
            //            if (maxNumberUser != null)
            //            {
            //                long newNumber = Convert.ToInt64(maxNumberUser.Number) + 1;
            //                number = newNumber.ToString();
            //            }

            //            qqEntities.UserSet.Add(new UserSet()
            //            {
            //                AddTime = DateTime.Now,
            //                Number = number,
            //                Name = userRegisterMessage.info.name,
            //                Password = userRegisterMessage.info.password,
            //                Signal = "没有留下签名"
            //            });
            //            int res = qqEntities.SaveChanges();
            //            if (res > 0)
            //            {
            //                Console.WriteLine("发送消息给客户端，生成账号：" + number);
            //                var returnNumberMessage = new MessageInfo<UserNumberReturn>() { type = 3, info = new UserNumberReturn() { userNumber = number } };
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserNumberReturn>>(returnNumberMessage));
            //                socket.Send(sendMessage);
            //            }
            //        }
            //    }
            //    else if (messageInfo.type == 4) // 接收到登录请求
            //    {
            //        // 第二次更具体地反序列化
            //        var userLoginMessage = JsonHelp.JsonDeserialize<MessageInfo<UserLogin>>(message);
            //        using (var qqEntities = new QQEntities())
            //        {
            //            var user = qqEntities.UserSet.FirstOrDefault(m => m.Number == userLoginMessage.info.userNumber);
            //            if (user != null && user.Password == userLoginMessage.info.password) // 登录成功 
            //            {
            //                // 从好友请求表中，找到以当前登录用户为添加目标的好友请求记录
            //                var myFriendApply = qqEntities.FriendRelateSet.Where
            //                     (m => m.ToUserId.ToString() == userLoginMessage.info.userNumber && m.IsPass == false).ToList();
            //                // 将好友请求发送方的相关信息放到一个列表中
            //                var FriendApplys = new List<User>();
            //                for (int i = 0; i < myFriendApply.Count; i++)
            //                {
            //                    string newFriendNumber = myFriendApply[i].FromUserId.ToString();
            //                    var newFriend = qqEntities.UserSet.FirstOrDefault(m => m.Number == newFriendNumber);
            //                    FriendApplys.Add(new User() { name = newFriend.Name, number = newFriend.Number, friendRelateId = myFriendApply[i].Id });
            //                }
            //                // 获取好友列表
            //                var myFriendRelate = qqEntities.FriendRelateSet.Where
            //                     (m => (m.ToUserId.ToString() == userLoginMessage.info.userNumber || m.FromUserId.ToString() == userLoginMessage.info.userNumber)
            //                     && m.IsPass == true).ToList();
            //                var Friends = new List<User>();
            //                for (int i = 0; i < myFriendRelate.Count; i++)
            //                {
            //                    string newFriendNumber = myFriendRelate[i].FromUserId.ToString();
            //                    if (newFriendNumber == userLoginMessage.info.userNumber) // 当自己是好友请求记录的发起方时
            //                    {
            //                        newFriendNumber = myFriendRelate[i].ToUserId.ToString();
            //                    }
            //                    var newFriend = qqEntities.UserSet.FirstOrDefault(m => m.Number == newFriendNumber);
            //                    // 将好友的相关信息放到一个列表中
            //                    Friends.Add(new User() { name = newFriend.Name, number = newFriend.Number, friendRelateId = myFriendRelate[i].Id });
            //                }


            //                Console.WriteLine("用户{" + user.Name + "}登录成功");
            //                var returnLoginMessage = new MessageInfo<UserLoginReturn>()
            //                {
            //                    type = 5,
            //                    info = new UserLoginReturn()
            //                    {
            //                        type = 1,
            //                        message = "登录成功",
            //                        name = user.Name,
            //                        signal = user.Signal,
            //                        friendsApplyList = FriendApplys,
            //                        myFriends = Friends
            //                    }
            //                };
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserLoginReturn>>(returnLoginMessage));
            //                socket.Send(sendMessage);
            //                // 登录成功后记录到静态类中
            //                LoginSockets.dic.Remove(userLoginMessage.info.userNumber);
            //                LoginSockets.dic.Add(userLoginMessage.info.userNumber, socket);
            //            }
            //            else
            //            {
            //                // 登录失败
            //                Console.WriteLine("登陆失败：无此账号或密码输入错误");
            //                var returnLoginMessage = new MessageInfo<UserLoginReturn>() { type = 5, info = new UserLoginReturn() { type = 0, message = "登录失败：无此账号或密码输入错误" } };
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserLoginReturn>>(returnLoginMessage));
            //                socket.Send(sendMessage);
            //            }
            //        }
            //    }
            //    else if (messageInfo.type == 6) // 接收到添加好友请求
            //    {
            //        Console.WriteLine("收到好友添加请求");
            //        // 第二次更具体地反序列化
            //        var friendAddMessage = JsonHelp.JsonDeserialize<MessageInfo<FriendAdd>>(message);
            //        using (var qqEntities = new QQEntities())
            //        {
            //            var user = qqEntities.UserSet.FirstOrDefault(m => m.Number == friendAddMessage.info.toUser);
            //            if (user != null) // 如果要添加的用户存在
            //            {
            //                qqEntities.FriendRelateSet.Add(new FriendRelateSet()
            //                {
            //                    AddTime = DateTime.Now,
            //                    FromUserId = Convert.ToInt32(friendAddMessage.info.fromUser),
            //                    ToUserId = Convert.ToInt32(friendAddMessage.info.toUser),
            //                    IsPass = false
            //                });
            //                qqEntities.SaveChanges(); // 将好友记录添加到数据库中

            //                var friendAddReturn = new MessageInfo<FriendAddReturn>() { type = 7, info = new FriendAddReturn() { type = 1, message = "成功发送好友请求,等待对方同意" } };
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<FriendAddReturn>>(friendAddReturn));
            //                socket.Send(sendMessage);
            //            }
            //            else // 无此用户
            //            {
            //                var friendAddReturn = new MessageInfo<FriendAddReturn>() { type = 7, info = new FriendAddReturn() { type = 0, message = "查无此用户" } };
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<FriendAddReturn>>(friendAddReturn));
            //                socket.Send(sendMessage);
            //            }
            //        }
            //    }
            //    else if (messageInfo.type == 8) // 同意添加请求
            //    {
            //        Console.WriteLine("客户端同意好友添加请求");
            //        var agreeFriendApply = JsonHelp.JsonDeserialize<MessageInfo<object>>(message);
            //        using (var qqEntities = new QQEntities())
            //        {
            //            int id = Convert.ToInt32(agreeFriendApply.info); // 此时因为info是object类型，所以还要转换一下
            //            var user = qqEntities.FriendRelateSet.FirstOrDefault(m => m.Id == id);
            //            if (user != null) // 若能查到记录
            //            {
            //                user.IsPass = true;
            //                qqEntities.SaveChanges();

            //                var agreeReturn = new MessageInfo<object>() { type = 9, info = "已成为好友，可以聊天了" };
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(agreeReturn));
            //                socket.Send(sendMessage);
            //            }
            //            else // 无此用户
            //            {
            //                var agreeReturn = new MessageInfo<object>() { type = 9, info = "添加失败" };
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(agreeReturn));
            //                socket.Send(sendMessage);
            //            }
            //        }
            //    }
            //    else if (messageInfo.type == 10) // 收到聊天信息
            //    {
            //        var chatMessage = JsonHelp.JsonDeserialize<MessageInfo<ChatMessage>>(message);
            //        using (var qqEntities = new QQEntities())
            //        {
            //            Console.WriteLine($"收到 {chatMessage.info.name} 发出的聊天信息：" + chatMessage.info.content);
            //            qqEntities.MessageSet.Add(new MessageSet()
            //            {
            //                AddTime = DateTime.Now,
            //                Content = chatMessage.info.content,
            //                FromUserId = Convert.ToInt32(chatMessage.info.fromUserNumber),
            //                ToUserId = Convert.ToInt32(chatMessage.info.toUserNumber),
            //                IsRead = false
            //            });
            //            qqEntities.SaveChanges();
            //            // 看一下接收消息的用户是否处于登录状态
            //            var toSocket = LoginSockets.dic[chatMessage.info.toUserNumber];
            //            // 如果该用户在线，则转发聊天信息给他
            //            if (toSocket != null)
            //            {
            //                var msgInfo = new MessageInfo<ChatMessage>();
            //                msgInfo.type = 11; // 服务器转发客户端信息的type
            //                msgInfo.info = chatMessage.info;
            //                byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(msgInfo));
            //                toSocket.Send(sendMessage);
            //            }
            //        }
            //    }

            //} 
            #endregion
        }

        private static void RecieveAccept()
        {
            while(true)
            {
                // 在服务器接收线程中不断接收客户端发来的连接请求，为每一个客户端创建一个socket
                Socket socket = receiveSocket.Accept(); // 客户端socket
                var ip = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
                var port = (socket.RemoteEndPoint as IPEndPoint).Port;
                // 接收客户端的信息

                //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, 
                //        new AsyncCallback(RecieveCallBack), socket);
                
                Console.WriteLine("接收到客户端的连接请求，"+ip+":"+port);
                ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveData), socket);

                // 接受客户端连接请求
                //Socket socket = receiveSocket.Accept();
                //clientNum++;
            }
        }

        // 客户端线程执行的回调函数
        private static void ReceiveData(object result)
        {
            Socket socket = result as Socket;
            string clientIP = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            int clientPort = ((IPEndPoint)socket.RemoteEndPoint).Port;

            while (true)
            {
                byte[] receiveByte = new byte[2048];
                int msgLength = socket.Receive(receiveByte, 0, receiveByte.Length, SocketFlags.None);
                byte[] validData = new byte[msgLength];
                Array.Copy(receiveByte, validData, msgLength);
                string message = Encoding.Unicode.GetString(validData); // 得到json字符串
                Console.WriteLine($"收到客户端{clientIP}:{clientPort}的消息：" + message);

                // 先进行第一次反序列化
                var messageInfo = JsonHelp.JsonDeserialize<MessageInfo<object>>(message);

                if (messageInfo.type == 1) // 接收到新客户端连接消息
                {
                    var checkNetworkMessage = JsonHelp.JsonDeserialize<MessageInfo<CheckNetwork>>(message);
                    Console.WriteLine("有新客户端连接成功：" + checkNetworkMessage.info.text);
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 2) // 接收到注册消息
                {
                    Console.WriteLine("收到客户端的注册消息：" + message);
                    var userRegisterMessage = JsonHelp.JsonDeserialize<MessageInfo<UserRegister>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        // 新账号为已有最大账号 + 1
                        var number = "10001";
                        var maxNumberUser = qqEntities.UserSet.OrderByDescending(m => m.Number).Take(1).FirstOrDefault();
                        if (maxNumberUser != null)
                        {
                            long newNumber = Convert.ToInt64(maxNumberUser.Number) + 1;
                            number = newNumber.ToString();
                        }

                        qqEntities.UserSet.Add(new UserSet()
                        {
                            AddTime = DateTime.Now,
                            Number = number,
                            Name = userRegisterMessage.info.name,
                            Password = userRegisterMessage.info.password,
                            Signal = "没有留下签名"
                        });
                        int res = qqEntities.SaveChanges();
                        if (res > 0)
                        {
                            Console.WriteLine("发送消息给客户端，生成账号：" + number);
                            var returnNumberMessage = new MessageInfo<UserNumberReturn>() { type = 3, info = new UserNumberReturn() { userNumber = number } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserNumberReturn>>(returnNumberMessage));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 4) // 接收到登录请求
                {
                    Console.WriteLine("收到客户端的登录消息：" + message);
                    // 第二次更具体地反序列化
                    var userLoginMessage = JsonHelp.JsonDeserialize<MessageInfo<UserLogin>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        var user = qqEntities.UserSet.FirstOrDefault(m => m.Number == userLoginMessage.info.userNumber);
                        if (user != null && user.Password == userLoginMessage.info.password) // 登录成功 
                        {
                            // 从好友请求表中，找到以当前登录用户为添加目标的好友请求记录
                            var myFriendApply = qqEntities.FriendRelateSet.Where
                                 (m => m.ToUserId.ToString() == userLoginMessage.info.userNumber && m.IsPass == false).ToList();
                            // 将好友请求发送方的相关信息放到一个列表中
                            var FriendApplys = new List<User>();
                            for (int i = 0; i < myFriendApply.Count; i++)
                            {
                                string newFriendNumber = myFriendApply[i].FromUserId.ToString();
                                var newFriend = qqEntities.UserSet.FirstOrDefault(m => m.Number == newFriendNumber);
                                FriendApplys.Add(new User() { name = newFriend.Name, number = newFriend.Number, friendRelateId = myFriendApply[i].Id });
                            }
                            // 获取好友列表
                            var myFriendRelate = qqEntities.FriendRelateSet.Where
                                 (m => (m.ToUserId.ToString() == userLoginMessage.info.userNumber || m.FromUserId.ToString() == userLoginMessage.info.userNumber)
                                 && m.IsPass == true).ToList();
                            var Friends = new List<User>();
                            for (int i = 0; i < myFriendRelate.Count; i++)
                            {
                                string myFriendNumber = myFriendRelate[i].FromUserId.ToString();
                                if (myFriendNumber == userLoginMessage.info.userNumber) // 当自己是好友请求记录的发起方时
                                {
                                    myFriendNumber = myFriendRelate[i].ToUserId.ToString();
                                }
                                var myFriend = qqEntities.UserSet.FirstOrDefault(m => m.Number == myFriendNumber);
                                // 将好友的相关信息放到一个列表中
                                Friends.Add(new User() { name = myFriend.Name, number = myFriend.Number, friendRelateId = myFriendRelate[i].Id });
                            }


                            Console.WriteLine("用户{" + user.Name + "}登录成功");
                            var returnLoginMessage = new MessageInfo<UserLoginReturn>()
                            {
                                type = 5,
                                info = new UserLoginReturn()
                                {
                                    type = 1,
                                    message = "登录成功",
                                    name = user.Name,
                                    signal = user.Signal,
                                    friendsApplyList = FriendApplys,
                                    myFriends = Friends
                                }
                            };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserLoginReturn>>(returnLoginMessage));
                            socket.Send(sendMessage);
                            // 登录成功后记录到静态类中
                            LoginSockets.dic.Remove(userLoginMessage.info.userNumber);
                            LoginSockets.dic.Add(userLoginMessage.info.userNumber, socket);
                        }
                        else
                        {
                            // 登录失败
                            Console.WriteLine("登陆失败：无此账号或密码输入错误");
                            var returnLoginMessage = new MessageInfo<UserLoginReturn>() { type = 5, info = new UserLoginReturn() { type = 0, message = "登录失败：无此账号或密码输入错误" } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserLoginReturn>>(returnLoginMessage));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 6) // 接收到添加好友请求
                {
                    Console.WriteLine("收到好友添加请求");
                    // 第二次更具体地反序列化
                    var friendAddMessage = JsonHelp.JsonDeserialize<MessageInfo<FriendAdd>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        var user = qqEntities.UserSet.FirstOrDefault(m => m.Number == friendAddMessage.info.toUser);
                        if (user != null) // 如果要添加的用户存在
                        {
                            qqEntities.FriendRelateSet.Add(new FriendRelateSet()
                            {
                                AddTime = DateTime.Now,
                                FromUserId = Convert.ToInt32(friendAddMessage.info.fromUser),
                                ToUserId = Convert.ToInt32(friendAddMessage.info.toUser),
                                IsPass = false
                            });
                            qqEntities.SaveChanges(); // 将好友记录添加到数据库中

                            var friendAddReturn = new MessageInfo<FriendAddReturn>() { type = 7, info = new FriendAddReturn() { type = 1, message = "成功发送好友请求,等待对方同意" } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<FriendAddReturn>>(friendAddReturn));
                            socket.Send(sendMessage);
                        }
                        else // 无此用户
                        {
                            var friendAddReturn = new MessageInfo<FriendAddReturn>() { type = 7, info = new FriendAddReturn() { type = 0, message = "查无此用户" } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<FriendAddReturn>>(friendAddReturn));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 8) // 同意添加请求
                {
                    Console.WriteLine("客户端同意好友添加请求");
                    var agreeFriendApply = JsonHelp.JsonDeserialize<MessageInfo<object>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        int id = Convert.ToInt32(agreeFriendApply.info); // 此时因为info是object类型，所以还要转换一下
                        var user = qqEntities.FriendRelateSet.FirstOrDefault(m => m.Id == id);
                        if (user != null) // 若能查到记录
                        {
                            user.IsPass = true;
                            qqEntities.SaveChanges();

                            var agreeReturn = new MessageInfo<object>() { type = 9, info = "已成为好友，可以聊天了" };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(agreeReturn));
                            socket.Send(sendMessage);
                        }
                        else // 无此用户
                        {
                            var agreeReturn = new MessageInfo<object>() { type = 9, info = "添加失败" };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(agreeReturn));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 10) // 收到聊天信息
                {
                    var chatMessage = JsonHelp.JsonDeserialize<MessageInfo<ChatMessage>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        Console.WriteLine($"收到 {chatMessage.info.name} 发出的聊天信息：" + chatMessage.info.content);
                        qqEntities.MessageSet.Add(new MessageSet()
                        {
                            AddTime = DateTime.Now,
                            Content = chatMessage.info.content,
                            FromUserId = Convert.ToInt32(chatMessage.info.fromUserNumber),
                            ToUserId = Convert.ToInt32(chatMessage.info.toUserNumber),
                            IsRead = false
                        });
                        qqEntities.SaveChanges();
                        // 看一下接收消息的用户是否处于登录状态
                        var toSocket = LoginSockets.dic[chatMessage.info.toUserNumber];
                        // 如果该用户在线，则转发聊天信息给他
                        if (toSocket != null)
                        {
                            var msgInfo = new MessageInfo<ChatMessage>();
                            msgInfo.type = 11; // 服务器转发客户端信息的type
                            msgInfo.info = chatMessage.info;
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(msgInfo));
                            toSocket.Send(sendMessage);
                            Console.WriteLine("用户在线，转发消息");
                        }
                    }
                }
                else if (messageInfo.type == 12) // 收到文件传送消息
                {
                    var chatMessage = JsonHelp.JsonDeserialize<MessageInfo<FileSend>>(message);
                    Console.WriteLine($"{chatMessage.info.fromUserName}想要向{chatMessage.info.toUserName}发送文件");
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 13) // 更新好友列表的请求
                {
                    Console.WriteLine($"收到客户端{clientIP}:{clientPort}的更新好友列表请求");
                    // 第二次更具体地反序列化
                    var refresh = JsonHelp.JsonDeserialize<MessageInfo<RefreshFriendList>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        var user = qqEntities.UserSet.FirstOrDefault(m => m.Number == refresh.info.userNumber);
                        
                        // 获取好友列表
                        var myFriendRelate = qqEntities.FriendRelateSet.Where
                             (m => (m.ToUserId.ToString() == refresh.info.userNumber || m.FromUserId.ToString() == refresh.info.userNumber)
                             && m.IsPass == true).ToList();
                        var Friends = new List<User>();
                        for (int i = 0; i < myFriendRelate.Count; i++)
                        {
                            string myFriendNumber = myFriendRelate[i].FromUserId.ToString();
                            if (myFriendNumber == refresh.info.userNumber) // 当自己是好友请求记录的发起方时
                            {
                                myFriendNumber = myFriendRelate[i].ToUserId.ToString();
                            }
                            var myFriend = qqEntities.UserSet.FirstOrDefault(m => m.Number == myFriendNumber);
                            // 将好友的相关信息放到一个列表中
                            Friends.Add(new User() { name = myFriend.Name, number = myFriend.Number, friendRelateId = myFriendRelate[i].Id });
                        }
                        var returnLoginMessage = new MessageInfo<RefreshFriendListReturn>()
                        {
                            type = 5,
                            info = new RefreshFriendListReturn()
                            {
                                myFriends = Friends
                            }
                        };
                        byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<RefreshFriendListReturn>>(returnLoginMessage));
                        socket.Send(sendMessage);
                    }
                }
            }
        }

        #region 新服务器逻辑
        private static void RecieveCallBack(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;

            try
            {
                int end = socket.EndReceive(result); // 结束位置
                result.AsyncWaitHandle.Close();
                var ip = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
                var port = (socket.RemoteEndPoint as IPEndPoint).Port;
                var message = Encoding.Unicode.GetString(buffer, 0, end);
                Console.WriteLine("收到客户端的消息：" + message);
                var messageInfo = JsonHelp.JsonDeserialize<MessageInfo<object>>(message);
                // 先进行第一次反序列化
                if (messageInfo.type == 1) // 接收到新客户端连接消息
                {
                    var checkNetworkMessage = JsonHelp.JsonDeserialize<MessageInfo<CheckNetwork>>(message);
                    Console.WriteLine("有新客户端连接成功：" + checkNetworkMessage.info.text);
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 2) // 接收到注册消息
                {
                    Console.WriteLine("收到客户端的注册消息：" + message);
                    var userRegisterMessage = JsonHelp.JsonDeserialize<MessageInfo<UserRegister>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        // 新账号为已有最大账号 + 1
                        var number = "10001";
                        var maxNumberUser = qqEntities.UserSet.OrderByDescending(m => m.Number).Take(1).FirstOrDefault();
                        if (maxNumberUser != null)
                        {
                            long newNumber = Convert.ToInt64(maxNumberUser.Number) + 1;
                            number = newNumber.ToString();
                        }

                        qqEntities.UserSet.Add(new UserSet()
                        {
                            AddTime = DateTime.Now,
                            Number = number,
                            Name = userRegisterMessage.info.name,
                            Password = userRegisterMessage.info.password,
                            Signal = "没有留下签名"
                        });
                        int res = qqEntities.SaveChanges();
                        if (res > 0)
                        {
                            Console.WriteLine("发送消息给客户端，生成账号：" + number);
                            var returnNumberMessage = new MessageInfo<UserNumberReturn>() { type = 3, info = new UserNumberReturn() { userNumber = number } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserNumberReturn>>(returnNumberMessage));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 4) // 接收到登录请求
                {
                    Console.WriteLine("收到客户端的登录消息：" + message);
                    // 第二次更具体地反序列化
                    var userLoginMessage = JsonHelp.JsonDeserialize<MessageInfo<UserLogin>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        var user = qqEntities.UserSet.FirstOrDefault(m => m.Number == userLoginMessage.info.userNumber);
                        if (user != null && user.Password == userLoginMessage.info.password) // 登录成功 
                        {
                            // 从好友请求表中，找到以当前登录用户为添加目标的好友请求记录
                            var myFriendApply = qqEntities.FriendRelateSet.Where
                                 (m => m.ToUserId.ToString() == userLoginMessage.info.userNumber && m.IsPass == false).ToList();
                            // 将好友请求发送方的相关信息放到一个列表中
                            var FriendApplys = new List<User>();
                            for (int i = 0; i < myFriendApply.Count; i++)
                            {
                                string newFriendNumber = myFriendApply[i].FromUserId.ToString();
                                var newFriend = qqEntities.UserSet.FirstOrDefault(m => m.Number == newFriendNumber);
                                FriendApplys.Add(new User() { name = newFriend.Name, number = newFriend.Number, friendRelateId = myFriendApply[i].Id });
                            }
                            // 获取好友列表
                            var myFriendRelate = qqEntities.FriendRelateSet.Where
                                 (m => (m.ToUserId.ToString() == userLoginMessage.info.userNumber || m.FromUserId.ToString() == userLoginMessage.info.userNumber)
                                 && m.IsPass == true).ToList();
                            var Friends = new List<User>();
                            for (int i = 0; i < myFriendRelate.Count; i++)
                            {
                                string newFriendNumber = myFriendRelate[i].FromUserId.ToString();
                                if (newFriendNumber == userLoginMessage.info.userNumber) // 当自己是好友请求记录的发起方时
                                {
                                    newFriendNumber = myFriendRelate[i].ToUserId.ToString();
                                }
                                var newFriend = qqEntities.UserSet.FirstOrDefault(m => m.Number == newFriendNumber);
                                // 将好友的相关信息放到一个列表中
                                Friends.Add(new User() { name = newFriend.Name, number = newFriend.Number, friendRelateId = myFriendRelate[i].Id });
                            }


                            Console.WriteLine("用户{" + user.Name + "}登录成功");
                            var returnLoginMessage = new MessageInfo<UserLoginReturn>()
                            {
                                type = 5,
                                info = new UserLoginReturn()
                                {
                                    type = 1,
                                    message = "登录成功",
                                    name = user.Name,
                                    signal = user.Signal,
                                    friendsApplyList = FriendApplys,
                                    myFriends = Friends
                                }
                            };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserLoginReturn>>(returnLoginMessage));
                            socket.Send(sendMessage);
                            // 登录成功后记录到静态类中
                            LoginSockets.dic.Remove(userLoginMessage.info.userNumber);
                            LoginSockets.dic.Add(userLoginMessage.info.userNumber, socket);
                        }
                        else
                        {
                            // 登录失败
                            Console.WriteLine("登陆失败：无此账号或密码输入错误");
                            var returnLoginMessage = new MessageInfo<UserLoginReturn>() { type = 5, info = new UserLoginReturn() { type = 0, message = "登录失败：无此账号或密码输入错误" } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<UserLoginReturn>>(returnLoginMessage));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 6) // 接收到添加好友请求
                {
                    Console.WriteLine("收到好友添加请求");
                    // 第二次更具体地反序列化
                    var friendAddMessage = JsonHelp.JsonDeserialize<MessageInfo<FriendAdd>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        var user = qqEntities.UserSet.FirstOrDefault(m => m.Number == friendAddMessage.info.toUser);
                        if (user != null) // 如果要添加的用户存在
                        {
                            qqEntities.FriendRelateSet.Add(new FriendRelateSet()
                            {
                                AddTime = DateTime.Now,
                                FromUserId = Convert.ToInt32(friendAddMessage.info.fromUser),
                                ToUserId = Convert.ToInt32(friendAddMessage.info.toUser),
                                IsPass = false
                            });
                            qqEntities.SaveChanges(); // 将好友记录添加到数据库中

                            var friendAddReturn = new MessageInfo<FriendAddReturn>() { type = 7, info = new FriendAddReturn() { type = 1, message = "成功发送好友请求,等待对方同意" } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<FriendAddReturn>>(friendAddReturn));
                            socket.Send(sendMessage);
                        }
                        else // 无此用户
                        {
                            var friendAddReturn = new MessageInfo<FriendAddReturn>() { type = 7, info = new FriendAddReturn() { type = 0, message = "查无此用户" } };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer<MessageInfo<FriendAddReturn>>(friendAddReturn));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 8) // 同意添加请求
                {
                    Console.WriteLine("客户端同意好友添加请求");
                    var agreeFriendApply = JsonHelp.JsonDeserialize<MessageInfo<object>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        int id = Convert.ToInt32(agreeFriendApply.info); // 此时因为info是object类型，所以还要转换一下
                        var user = qqEntities.FriendRelateSet.FirstOrDefault(m => m.Id == id);
                        if (user != null) // 若能查到记录
                        {
                            user.IsPass = true;
                            qqEntities.SaveChanges();

                            var agreeReturn = new MessageInfo<object>() { type = 9, info = "已成为好友，可以聊天了" };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(agreeReturn));
                            socket.Send(sendMessage);
                        }
                        else // 无此用户
                        {
                            var agreeReturn = new MessageInfo<object>() { type = 9, info = "添加失败" };
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(agreeReturn));
                            socket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
                else if (messageInfo.type == 10) // 收到聊天信息
                {
                    var chatMessage = JsonHelp.JsonDeserialize<MessageInfo<ChatMessage>>(message);
                    using (var qqEntities = new QQEntities())
                    {
                        Console.WriteLine($"收到 {chatMessage.info.name} 发出的聊天信息：" + chatMessage.info.content);
                        qqEntities.MessageSet.Add(new MessageSet()
                        {
                            AddTime = DateTime.Now,
                            Content = chatMessage.info.content,
                            FromUserId = Convert.ToInt32(chatMessage.info.fromUserNumber),
                            ToUserId = Convert.ToInt32(chatMessage.info.toUserNumber),
                            IsRead = false
                        });
                        qqEntities.SaveChanges();
                        // 看一下接收消息的用户是否处于登录状态
                        var toSocket = LoginSockets.dic[chatMessage.info.toUserNumber];
                        // 如果该用户在线，则转发聊天信息给他
                        if (toSocket != null)
                        {
                            var msgInfo = new MessageInfo<ChatMessage>();
                            msgInfo.type = 11; // 服务器转发客户端信息的type
                            msgInfo.info = chatMessage.info;
                            byte[] sendMessage = Encoding.Unicode.GetBytes(JsonHelp.JsonSerializer(msgInfo));
                            toSocket.Send(sendMessage);
                        }
                    }
                    //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    //        new AsyncCallback(RecieveCallBack), socket);
                }
            }
            catch
            {
                throw;
            }
        } 
        #endregion
    }
}
