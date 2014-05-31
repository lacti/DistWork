using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DistWork.Util;

namespace DistWork.Core
{
    public static class WorkSocketHelper
    {
        public static bool SendWork(this Socket socket, IWork work)
        {
            return SendWork(new[] {socket}, work) == 1;
        }

        public static int SendWork(this IEnumerable<Socket> sockets, IWork work)
        {
            using (var stream = new MemoryStream())
            {
                var lengthWriter = new BinaryWriter(stream);
                lengthWriter.Write(0);

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, work);

                lengthWriter.Seek(0, SeekOrigin.Begin);
                lengthWriter.Write((int)(stream.Length - sizeof(int)));

                var bytes = stream.ToArray();
                var tasks = sockets.Select(socket => socket.SendAsync(bytes)).ToList();
                try
                {
                    Task.WaitAll(tasks.Cast<Task>().ToArray());
                }
                catch (Exception)
                {
                    return tasks.Count(e => e.IsCompleted);
                }
                return tasks.Count();
            }
        }

        public static async Task<IWork> ReceiveWork(this Socket socket)
        {
            var lengthBytes = await socket.ReceiveAsync(sizeof (int));
            var length = BitConverter.ToInt32(lengthBytes, 0);
            var dataBytes = await socket.ReceiveAsync(length);
            using (var stream = new MemoryStream(dataBytes))
            {
                var formatter = new BinaryFormatter();
                var work = formatter.Deserialize(stream) as IWork;
                if (work == null)
                    throw new NullReferenceException();

                return work;
            }
        }
    }
}