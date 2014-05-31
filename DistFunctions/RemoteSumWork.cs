using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DistWork.Core;

namespace DistFunctions
{
    [Serializable]
    public class RemoteSumWork : RpcWork<int>
    {
        private readonly int _leftValue;
        private readonly int _rightValue;

        public RemoteSumWork(int left, int right)
        {
            _leftValue = left;
            _rightValue = right;
        }

        protected override int ExecuteWork(Socket endPoint)
        {
            return _leftValue + _rightValue;
        }

        protected override void ExecuteReturn(int returnValue, Socket endPoint)
        {
            Console.WriteLine("Return from: " + returnValue);
        }
    }

}
