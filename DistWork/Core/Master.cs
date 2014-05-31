using System;
using System.Net;
using System.Net.Sockets;
using DistWork.Util;

namespace DistWork.Core
{
    public class Master
    {
        public delegate Socket SelectSocketDecl(DistributeContext context);

        private readonly SocketContainer _container = new SocketContainer();

        private readonly int _port;
        private int _defaultSocketSelectedIndex;

        private SelectSocketDecl _socketSelector;

        public Master(int port)
        {
            _port = port;
            _socketSelector = DefaultSocketSelector;
        }

        public SelectSocketDecl SocketSelector
        {
            set { _socketSelector = value; }
        }

        public int ConnectedSlaveCount
        {
            get { return _container.Count; }
        }

        public async void Start()
        {
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndPoint = new IPEndPoint(IPAddress.Any, _port);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    var clientSocket = await listener.AcceptAsync();
                    ProcessSocket(clientSocket);
                }
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
        }

        private Socket DefaultSocketSelector(DistributeContext context)
        {
            var index = _defaultSocketSelectedIndex%context.Sockets.Count;
            var socket = context.Sockets[index].Key;
            ++_defaultSocketSelectedIndex;
            return socket;
        }

        public void DistributeWork(IWork work)
        {
            var socket = _container.SelectSocket(sockets => _socketSelector(new DistributeContext(_container, sockets.AsReadOnly(), work)));
            if (socket == null)
                throw new NullReferenceException();

            socket.SendWork(work);
        }

        public void BroadcastWork(IWork work)
        {
            _container.EnumerateSockets(sockets => sockets.SendWork(work));
        }

        private async void ProcessSocket(object threadState)
        {
            var socket = (Socket) threadState;
            _container.AddSocket(socket);

            Logger.Write("Connected from: " + socket.RemoteEndPoint);
            try
            {
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

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }

            _container.RemoveSocket(socket);
        }
    }
}