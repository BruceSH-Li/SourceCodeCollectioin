using MXY_Chat.Client.Helpers;
using MXY_Chat.Client.Models;
using System.Windows;

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// SignUp.xaml 的交互逻辑
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            var name = this.tboxUserName.Text;
            var pwd = this.pboxPwd.Password;
            var msgInfo = new MessageInfo<UserRegister>();
            msgInfo.type = 2; // 注册信息的type == 2
            msgInfo.info = new UserRegister() { name = name, password = pwd };
            string message = JsonHelp.JsonSerializer<MessageInfo<UserRegister>>(msgInfo);
            socketHelper.SendMessage(message);

            // 接收注册结果（先接收到json字符串） 需要进行2次反序列化
            var receiveMessage = socketHelper.ReceiveMessage();
            // 第一次反序列化，先判断信息的type，然后才能确定是哪种信息
            var registerReturn = JsonHelp.JsonDeserialize<MessageInfo<object>>(receiveMessage);
            if (registerReturn.type == 3) // 接收到服务器返回的注册结果信息
            {
                // 第二次更具体的反序列化
                var userNumberReturn = JsonHelp.JsonDeserialize<MessageInfo<UserNumberReturn>>(receiveMessage);
                var show = new showNumber(userNumberReturn.info.userNumber);
                show.ShowDialog();                
            }
        }
    }
}
