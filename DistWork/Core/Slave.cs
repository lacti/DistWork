using System;
using System.Net.Sockets;
using System.Threading;
using DistWork.Util;

namespace DistWork.Core
{
    public class Slave
    {
        private readonly string _host;
        private readonly int _port;

        public Slave(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async void Start()
        {
            try
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(_host, _port);

                while (true)
                {
                    var work = await socket.ReceiveWork();
                    work.Execute(socket);
                }
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
        }
    }
}