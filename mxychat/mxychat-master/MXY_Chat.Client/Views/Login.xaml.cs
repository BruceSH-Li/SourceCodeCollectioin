using MXY_Chat.Client.Helpers;
using MXY_Chat.Client.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Socket ClientSocket { get; set; }
        public Login()
        {
            InitializeComponent();
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tboxNumber.Focus();
        }

        private void tboxNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrEmpty(tboxNumber.Text) && tboxNumber.Text.Length > 0)
            {
                textEmail.Visibility = Visibility.Collapsed;
            }
            else
            {
                textEmail.Visibility = Visibility.Visible;
            }
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pboxPassword.Focus();
        }

        private void pboxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(pboxPassword.Password) && pboxPassword.Password.Length > 0)
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var number = this.tboxNumber.Text;
            var pwd = this.pboxPassword.Password;
            var msgInfo = new MessageInfo<UserLogin>();
            msgInfo.type = 4; // 登录信息的type == 4
            msgInfo.info = new UserLogin() { userNumber = number, password = pwd };
            string message = JsonHelp.JsonSerializer<MessageInfo<UserLogin>>(msgInfo);
            socketHelper.SendMessage(message);

            // 接收注册结果（先接收到json字符串） 需要进行2次反序列化
            var receiveMessage = socketHelper.ReceiveMessage();
            // 第一次反序列化，先判断信息的type，然后才能确定是哪种信息
            var registerReturn = JsonHelp.JsonDeserialize<MessageInfo<object>>(receiveMessage);
            if (registerReturn.type == 5) // 接收到服务器返回的登录结果信息
            {
                // 第二次更具体的反序列化
                var loginReturn = JsonHelp.JsonDeserialize<MessageInfo<UserLoginReturn>>(receiveMessage);
                if(loginReturn.info.type == 1) // 1表示登录成功
                {
                    LoginUser.number = number;
                    LoginUser.name = loginReturn.info.name;
                    LoginUser.signal = loginReturn.info.signal;

                    // 加载主界面，再关掉登录界面
                    this.Hide();
                    var dlg = new MainWindow(LoginUser.name, LoginUser.signal, 
                        loginReturn.info.friendsApplyList, loginReturn.info.myFriends);
                    dlg.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(loginReturn.info.message);
                }
            }
        }

        #region 客户端接收登录成功的信息
        public void ReceiveData(object socket)
        {
            var proxSocket = socket as Socket;
            byte[] data = new byte[1024 * 1024];
            while (true)
            {
                
            }
        }
        #endregion

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
                new SignUp().Show();
        }

    }

}
