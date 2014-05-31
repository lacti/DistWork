using System.Collections.Generic;
using System.Net.Sockets;

namespace DistWork.Core
{
    public sealed class DistributeContext
    {
        public readonly IList<KeyValuePair<Socket, SocketInformation>> Sockets;
        public readonly IWork Work;
        private readonly SocketContainer _container;

        internal DistributeContext(SocketContainer container,
                                   IList
                                       <KeyValuePair<Socket, SocketInformation>>
                                       sockets,
                                   IWork work)
        {
            _container = container;
            Sockets = sockets;
            Work = work;
        }

        public void UpdateSocketInfomation(Socket socket, SocketInformation info)
        {
            _container.UpdateSocketInformation(socket, info);
        }
    }
}