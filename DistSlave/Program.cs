using System;
using System.Threading.Tasks;
using DistWork.Core;
using DistWork.Util;

namespace DistSlave
{
    class Program
    {
        static void Main()
        {
            var slave = new Slave("127.0.0.1", 12345);
            Task.Factory.StartNew(slave.Start);

            Logger.Write("Slave Start!");
            Console.ReadKey();
        }
    }
}
