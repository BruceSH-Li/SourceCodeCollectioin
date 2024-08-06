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
using System.Xml.Linq;

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// DealFriendApply.xaml 的交互逻辑
    /// </summary>
    public partial class DealFriendApply : Window
    {
        private List<User> friendsApplyList;

        public DealFriendApply()
        {
            InitializeComponent();
        }

        public DealFriendApply(List<User> friendsApplyList)
        {
            InitializeComponent();
            this.friendsApplyList = friendsApplyList;
            // 便利好友请求列表中的每一项，分别读取每一项的相关信息
            for (int i = 0; i < friendsApplyList.Count; i++)
            {
                // 请求方头像
                var image = new Image();
                image.Width = 40;
                image.Height = 40;
                image.Source = new BitmapImage(new Uri($"../Sources/{friendsApplyList[i].name}.jpg", UriKind.Relative));
                image.Margin = new Thickness(10, 3, 10, 3);
                // 请求方名字
                TextBlock textBlock = new TextBlock();
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Text = friendsApplyList[i].name;
                textBlock.Tag = friendsApplyList[i].friendRelateId;
                // 同意按钮
                var btnAgree = new Button();
                btnAgree.Content = "  同意  ";
                btnAgree.Foreground = Brushes.Green;
                btnAgree.Margin = new Thickness(0, 10, 10, 10);
                btnAgree.Padding = new Thickness(5);
                btnAgree.Background = Brushes.Transparent;
                btnAgree.BorderThickness = new Thickness(0);
                btnAgree.Tag = friendsApplyList[i].friendRelateId;
                btnAgree.Click += btnAgree_Click;
                // 不同意按钮
                var btnDisagree = new Button();
                btnDisagree.Content = "  不同意  ";
                btnDisagree.Foreground = Brushes.Red;
                btnDisagree.Margin = new Thickness(0, 10, 10, 10);
                btnDisagree.Padding = new Thickness(5);
                btnDisagree.Background = Brushes.Transparent;
                btnDisagree.BorderThickness = new Thickness(0);

                var stackPanelChild = new StackPanel();
                stackPanelChild.Width = 240;
                stackPanelChild.Orientation = Orientation.Horizontal;
                stackPanelChild.Children.Add(image);
                stackPanelChild.Children.Add(textBlock);
                stackPanelChild.Children.Add(btnAgree);
                stackPanelChild.Children.Add(btnDisagree);
                
                // 一条好友请求信息的外层stackpanel
                var stackPanelParent = new StackPanel();
                stackPanelParent.Background = Brushes.Transparent;
                stackPanelParent.Height = 50;
                stackPanelParent.Margin = new Thickness(0, 12, 0, 0);
                stackPanelParent.Children.Add(stackPanelChild);
            
                this.ApplyUserList.Children.Add(stackPanelParent);
            }
            

        }

        private void btnAgree_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var msgInfo = new MessageInfo<object>();
            msgInfo.type = 8; // 登录信息的type == 4
            msgInfo.info = btn.Tag;
            string message = JsonHelp.JsonSerializer<MessageInfo<object>>(msgInfo);
            socketHelper.SendMessage(message);
            var rtMsg = socketHelper.ReceiveMessage();
            var messageInfo = JsonHelp.JsonDeserialize<MessageInfo<object>>(rtMsg);
            if (messageInfo.type == 9) // 接收到服务器返回的添加结果信息
            {
                
                MessageBox.Show(messageInfo.info.ToString());
            }
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // 拖动窗体
        }
    }
}
