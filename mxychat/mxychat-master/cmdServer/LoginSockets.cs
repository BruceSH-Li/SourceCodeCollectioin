using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace cmdServer
{
    public static class LoginSockets
    {
        public static Dictionary<string, Socket> dic = new Dictionary<string, Socket>();
    }
}
