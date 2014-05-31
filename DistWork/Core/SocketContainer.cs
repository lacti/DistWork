using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using DistWork.Util;

namespace DistWork.Core
{
    internal class SocketContainer
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<Socket, SocketInformation> _sockets = new Dictionary<Socket, SocketInformation>();

        public int Count
        {
            get { return _lock.DoReadLock(() => _sockets.Count); }
        }

        public void AddSocket(Socket socket)
        {
            _lock.DoWriteLock(() => _sockets.Add(socket, SocketInformation.Invalid));
        }

        public void RemoveSocket(Socket socket)
        {
            _lock.DoWriteLock(() => _sockets.Remove(socket));
        }

        public void UpdateSocketInformation(Socket socket, SocketInformation information)
        {
            _lock.DoWriteLock(() =>
                {
                    if (_sockets.ContainsKey(socket))
                        _sockets[socket] = information;
                });
        }

        public void EnumerateSockets(Action<IEnumerable<Socket>> actor)
        {
            _lock.DoReadLock(() => actor(_sockets.Keys));
        }

        public Socket SelectSocket(Func<List<KeyValuePair<Socket, SocketInformation>>, Socket> selector)
        {
            return _lock.DoReadLock(() => selector(_sockets.ToList()));
        }
    }
}