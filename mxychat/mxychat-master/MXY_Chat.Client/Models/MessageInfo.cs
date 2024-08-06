using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MXY_Chat.Client.Models
{
    public class MessageInfo<T>
    {
        public int type { get; set; }
        public T info { get; set; } // 不同的消息，带有不同种类的信息
    }

    /// <summary>
    /// 下面是info的不同类型
    /// </summary>
    public class CheckNetwork // type : 1
    {
        public string text { get; set; }
    }
    public class UserRegister // type : 2
    {
        public string name { get; set; }
        public string password { get; set; }
    }
    public class UserNumberReturn // type : 3
    {
        public string userNumber { get; set; }
    }
    public class UserLogin // type : 4
    {
        public string userNumber { get; set; }
        public string password { get; set; }
    }
    public class UserLoginReturn // type：5
    {
        public int type { get; set; } // 1：表示登录成功；0：表示登录失败
        public string message { get; set; }
        public string name { get; set; }
        public string signal { get; set; }
        public List<User> friendsApplyList { get; set; } // 记录当前登录用户的好友请求列表
        public List<User> myFriends { get; set; } // 记录当前登录用户的好友关联信息
    }
    public class FriendAdd // type : 6
    {
        public string fromUser { get; set; }
        public string toUser { get; set; }
    }
    public class FriendAddReturn // type : 7
    {
        public int type { get; set; } // 1：表示登录成功；0：表示登录失败
        public string message { get; set; }
    }
    // 同意好友请求。type : 8
    // 返回同意结果。type : 9
    public class ChatMessage // type : 10
    {
        public string name { get; set; }
        public string fromUserNumber { get; set; }
        public string toUserNumber { get; set; }
        public string content { get; set; }
    }
    // 服务器转发聊天信息。type : 11 
    public class FileSend // type : 12
    {
        public string fromUserName { get; set; }
        public string toUserName { get; set; }
    }
    public class RefreshFriendList // type：13
    {
        public string userNumber { get; set; }
    }
    public class RefreshFriendListReturn // type：14
    {
        public List<User> myFriends { get; set; } // 记录当前登录用户的好友关联信息
    }
}
