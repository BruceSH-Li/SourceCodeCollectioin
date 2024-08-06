using MXY_Chat.Client.Helpers;
using MXY_Chat.Client.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MXY_Chat.Client.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string name;
        private string signal;
        private List<User> friendsApplyList;
        private List<User> myFriends;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string name, string signal, List<User> friendsApplyList, List<User> myFriends)
        {
            InitializeComponent();
            this.name = name;
            this.signal = signal;
            this.friendsApplyList = friendsApplyList;
            this.myFriends= myFriends;
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AddFriend();
            dlg.ShowDialog();
        }

        private void dealApply_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new DealFriendApply(friendsApplyList);
            dlg.ShowDialog();
        }

        private void tbName_Loaded(object sender, RoutedEventArgs e)
        {
            tbName.Text = name;
            tbSignal.Text = signal;
            BitmapImage bitmapImage = new BitmapImage(new Uri($"../Sources/{name}.jpg", UriKind.Relative));
            imgTouXiang.Source = bitmapImage;
        }

        private void FriendsStackPanel_304_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in myFriends)
            {
                // 头像
                var image = new Image();
                image.Width = 30;
                image.Height = 30;
                image.Source = new BitmapImage(new Uri($"../Sources/{item.name}.jpg", UriKind.Relative));
                image.Margin = new Thickness(15,3,3,3);

                // 好友名字
                var tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Text = item.name;
                tb.Tag = item.number;

                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Width = 240;
                stackPanel.Children.Add(image);
                stackPanel.Children.Add(tb);

                var btn = new Button();
                btn.Background = Brushes.Transparent;
                btn.Height = 50;
                var chatUser = new ChatUser();
                chatUser.name = item.name;
                chatUser.number = item.number;
                btn.Tag = chatUser;
                btn.BorderThickness = new Thickness(0);
                btn.Content = stackPanel; // 头像和名字放在stackPanel里面，然后塞到button里面
                btn.Click += btn_Click;

                this.FriendsStackPanel_304.Children.Add(btn); // 往304好友列表里面➕一个人
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var chatUser = btn.Tag as ChatUser;
            var dlg = new Chat(chatUser);
            dlg.Show();
        }
        public class ChatUser
        {
            public string name;
            public string number;
        }

        private void btnFresh_Click(object sender, RoutedEventArgs e)
        {
            var msgInfo = new MessageInfo<RefreshFriendList>(); // 向服务器发起好友列表刷新请求
            msgInfo.type = 13; 
            msgInfo.info = new RefreshFriendList() { userNumber = LoginUser.number};
            string message = JsonHelp.JsonSerializer<MessageInfo<RefreshFriendList>>(msgInfo);
            socketHelper.SendMessage(message);

            var receiveMessage = socketHelper.ReceiveMessage();

            var refreshFriendListReturn = JsonHelp.JsonDeserialize<MessageInfo<RefreshFriendListReturn>>(receiveMessage);

            this.FriendsStackPanel_304.Children.Clear();
            foreach (var item in refreshFriendListReturn.info.myFriends)
            {
                // 头像
                var image = new Image();
                image.Width = 30;
                image.Height = 30;
                image.Source = new BitmapImage(new Uri($"../Sources/{item.name}.jpg", UriKind.Relative));
                image.Margin = new Thickness(15, 3, 3, 3);

                // 好友名字
                var tb = new TextBlock();
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.Text = item.name;
                tb.Tag = item.number;

                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Width = 240;
                stackPanel.Children.Add(image);
                stackPanel.Children.Add(tb);

                var btn = new Button();
                btn.Background = Brushes.Transparent;
                btn.Height = 50;
                var chatUser = new ChatUser();
                chatUser.name = item.name;
                chatUser.number = item.number;
                btn.Tag = chatUser;
                btn.BorderThickness = new Thickness(0);
                btn.Content = stackPanel; // 头像和名字放在stackPanel里面，然后塞到button里面
                btn.Click += btn_Click;

                this.FriendsStackPanel_304.Children.Add(btn); // 往304好友列表里面➕一个人
            }
        }

        private void btn_EmotionAnalyse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AI();
            dlg.Show();
        }
    }
}

