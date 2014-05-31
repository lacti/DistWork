using System;
using System.Net.Sockets;

namespace DistWork.Core
{
    public interface IWork
    {
        void Execute(Socket endPoint);
    }
}