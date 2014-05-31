using System;
using System.Net.Sockets;

namespace DistWork.Core
{
    [Serializable]
    public abstract class RpcWork<TReturn> : IWork
    {
        [Serializable]
        private class ReturnWork : IWork
        {
            private readonly RpcWork<TReturn> _parentWork;
            private readonly TReturn _returnValue;

            public ReturnWork(RpcWork<TReturn> parentWork, TReturn returnValue)
            {
                _parentWork = parentWork;
                _returnValue = returnValue;
            }

            public void Execute(Socket endPoint)
            {
                _parentWork.ExecuteReturn(_returnValue, endPoint);
            }
        }

        public void Execute(Socket endPoint)
        {
            var returnValue = ExecuteWork(endPoint);
            endPoint.SendWork(new ReturnWork(this, returnValue));
        }

        protected abstract TReturn ExecuteWork(Socket endPoint);
        protected abstract void ExecuteReturn(TReturn returnValue, Socket endPoint);
    }
}