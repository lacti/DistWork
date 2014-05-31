using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DistWork.Core;
using DistWork.Util;

namespace DistWork
{
    [Serializable]
    class SlaveWork : IWork
    {
        public void Execute(Socket endPoint)
        {
            Console.WriteLine("Do my work: " + endPoint.RemoteEndPoint);
            endPoint.SendWork(new MasterWork(new SlaveResult("TEST MESSAGE")));
        }
    }

    [Serializable]
    class SlaveResult
    {
        public readonly string SlaveGeneratedMessage;

        public SlaveResult(string message)
        {
            SlaveGeneratedMessage = message;
        }
    }

    [Serializable]
    class MasterWork : IWork
    {
        private readonly SlaveResult _result;

        public MasterWork(SlaveResult result)
        {
            _result = result;
        }

        public void Execute(Socket endPoint)
        {
            Console.WriteLine("Do master work: " + endPoint.RemoteEndPoint);
            Console.WriteLine("Received from slave: " + _result.SlaveGeneratedMessage);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var master = new Master(12345);
            Task.Factory.StartNew(master.Start);

            Thread.Sleep(1000);

            const int slaveCount = 10;
            foreach (var index in Enumerable.Range(0, slaveCount))
            {
                var slave = new Slave("127.0.0.1", 12345);
                Task.Factory.StartNew(slave.Start);
            }

            while (master.ConnectedSlaveCount != slaveCount)
                Thread.Sleep(0);

            Logger.Write("Start!");
            while (true)
            {
                master.DistributeWork(new SlaveWork());
                Thread.Sleep(1000);
            }
            Console.ReadLine();
        }
    }
}