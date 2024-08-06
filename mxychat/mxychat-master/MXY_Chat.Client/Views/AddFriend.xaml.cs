using MXY_Chat.Client.Helpers;
using MXY_Chat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// AddFriend.xaml 的交互逻辑
    /// </summary>
    public partial class AddFriend : Window
    {
        public AddFriend()
        {
            InitializeComponent();
        }

        private void btnAddFriend_Click(object sender, RoutedEventArgs e)
        {
            var number = this.tboxNumber.Text;
            var msgInfo = new MessageInfo<FriendAdd>();
            msgInfo.type = 6; // 添加好友的信息的type == 6
            msgInfo.info = new FriendAdd() { fromUser = LoginUser.number, toUser = number};
            string message = JsonHelp.JsonSerializer<MessageInfo<FriendAdd>>(msgInfo);
            socketHelper.SendMessage(message);

            // 接收注册结果（先接收到json字符串） 需要进行2次反序列化
            var receiveMessage = socketHelper.ReceiveMessage();
            // 第一次反序列化，先判断信息的type，然后才能确定是哪种信息
            var registerReturn = JsonHelp.JsonDeserialize<MessageInfo<object>>(receiveMessage);
            if (registerReturn.type == 7) // 接收到服务器返回的添加结果信息
            {
                // 第二次更具体的反序列化
                var friendAddReturn = JsonHelp.JsonDeserialize<MessageInfo<FriendAddReturn>>(receiveMessage);
                if (friendAddReturn.info.type == 1) // 1表示添加成功
                {
                    MessageBox.Show(friendAddReturn.info.message);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(friendAddReturn.info.message);
                }
            }
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
    }
}
