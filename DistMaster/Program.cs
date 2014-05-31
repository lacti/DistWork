using System.Threading;
using System.Threading.Tasks;
using DistFunctions;
using DistWork.Core;
using DistWork.Util;

namespace DistMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            var master = new Master(12345);
            Task.Factory.StartNew(master.Start);

            Thread.Sleep(1000);
            Task.Yield();

            while (master.ConnectedSlaveCount == 0)
                Thread.Sleep(0);

            Logger.Write("Master Start!");
            while (true)
            {
                master.DistributeWork(new FileSendWork("DistFunctions.dll"));
                master.DistributeWork(new RemoteSumWork(100, 200));
                Thread.Sleep(1000);
            }
        }
    }
}
