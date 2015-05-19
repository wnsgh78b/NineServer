using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class User
    {
        public TcpClient client;
        public string userId;

        public User(string _userId,TcpClient _client)
        {
            this.userId = _userId;
            this.client = _client;
        }
    }
}

