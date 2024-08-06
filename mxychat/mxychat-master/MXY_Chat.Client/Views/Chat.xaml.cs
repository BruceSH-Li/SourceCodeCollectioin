using MXY_Chat.Client.Helpers;
using MXY_Chat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MXY_Chat.Client.Views.MainWindow;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// Chat.xaml 的交互逻辑
    /// </summary>
    public partial class Chat : Window
    {
        ChatUser chatUser;
        static byte[] buffer = new byte[65535];
        private Thread receiveThread;
        private bool isRuning = false;
        private delegate void UpdateChatUIHandler(string message);

        public Chat()
        {
            InitializeComponent();
        }

        public Chat(ChatUser chatUser)
        {
            InitializeComponent();
            this.chatUser = chatUser; 
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // 拖动窗体
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isRuning = true;
            // 加载聊天对象的头像和名字
            this.chatUserName.Text = this.chatUser.name;
            this.chatUserImg.Source = new BitmapImage(new Uri($"../Sources/{this.chatUser.name}.jpg", UriKind.Relative));

            receiveThread = new Thread(ListenChatMessage);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ListenChatMessage()
        {
            while (isRuning)
            {
                try
                {
                    var receiveMessage = socketHelper.ReceiveMessage();
                    var chatMessage = JsonHelp.JsonDeserialize<MessageInfo<ChatMessage>>(receiveMessage);
                    UpdateUI(chatMessage);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpdateUI(MessageInfo<ChatMessage> chatMessage)
        {
            //Dispatcher.Invoke(() => UpdateChatUI(chatMessage));
            if (Dispatcher.CheckAccess())
            {
                // UI 线程上执行更新操作
                this.tbox_messageShow.Text += this.chatUser.name + "说：" + chatMessage.info.content + "\n";
            }
            else
            {
                // 非 UI 线程，通过 Dispatcher 在 UI 线程上执行更新操作
                Dispatcher.Invoke(() => UpdateUI(chatMessage));
            }
        }

        private void UpdateChatUI(MessageInfo<ChatMessage> chatMessage)
        {
            this.tbox_messageShow.Text += this.chatUser.name + "说：" + chatMessage.info.content + "\n";
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            var message = this.tbox_messageBox.Text;
            this.tbox_messageShow.Text += LoginUser.name + "说：" + message + "\n";

            var msgInfo = new MessageInfo<ChatMessage>();
            msgInfo.type = 10; 
            msgInfo.info = new ChatMessage() {name = LoginUser.name, fromUserNumber = LoginUser.number, 
                            toUserNumber = this.chatUser.number, content = message};
            string json = JsonHelp.JsonSerializer<MessageInfo<ChatMessage>>(msgInfo);
            socketHelper.SendMessage(json);
            tbox_messageBox.Text = string.Empty;
        }

        private void btn_fileSend_Click(object sender, RoutedEventArgs e)
        {
            var msgInfo = new MessageInfo<FileSend>();
            msgInfo.type = 12; // 添加好友的信息的type == 6
            msgInfo.info = new FileSend() {fromUserName = LoginUser.name, toUserName = this.chatUser.name};
            string json = JsonHelp.JsonSerializer<MessageInfo<FileSend>>(msgInfo);
            socketHelper.SendMessage(json);
        }
    }
}
