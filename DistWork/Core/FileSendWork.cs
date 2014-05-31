using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DistWork.Core
{
    [Serializable]
    public class FileSendWork : IWork
    {
        private readonly string _fileName;
        private readonly byte[] _fileBytes;

        public FileSendWork(string filePath)
        {
            _fileName = Path.GetFileName(filePath);
            _fileBytes = File.ReadAllBytes(filePath);
        }

        public void Execute(Socket endPoint)
        {
            if (File.Exists(_fileName))
                return;

            using (
                var stream = new FileStream(_fileName, FileMode.CreateNew,
                                            FileAccess.Write, FileShare.Write))
            {
                stream.Write(_fileBytes, 0, _fileBytes.Length);
            }
        }
    }
}
