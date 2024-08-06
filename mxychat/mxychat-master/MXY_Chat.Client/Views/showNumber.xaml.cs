using System.Windows;

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// showNumber.xaml 的交互逻辑
    /// </summary>
    public partial class showNumber : Window
    {
        private string userNumber;

        public showNumber()
        {
            InitializeComponent();
        }

        public showNumber(string userNumber)
        {
            InitializeComponent(); // 记得每个构造函数都要带上这个初始化方法
            this.userNumber = userNumber;
            tboxNumber.Text = userNumber;
        }
    }
}
