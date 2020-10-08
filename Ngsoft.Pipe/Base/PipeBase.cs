using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Ngsoft.Pipe
{
    public abstract class PipeBase
    {
        protected string PipeName { get; }
        protected Encoding Encoding { get; }

        public PipeBase(string pipeName, Encoding encoding)
        {
            PipeName = string.IsNullOrWhiteSpace(pipeName) == false ? pipeName : throw new ArgumentException("Pipe name cannot be empty.", nameof(pipeName));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
        }

        protected string ReadInput(PipeStream pipe)
        {
            var buffer = new byte[1024];
            using (var stream = new MemoryStream())
            {
                do
                {
                    var count = pipe.Read(buffer, offset: 0, count: buffer.Length);
                    stream.Write(buffer, offset: 0, count);
                }
                while (pipe.IsMessageComplete == false);
                return Encoding.GetString(bytes: stream.ToArray());
            }
        }

        protected void WriteOutput(PipeStream pipe, string output)
        {
            var data = Encoding.GetBytes(output);
            pipe.Write(buffer: data, offset: 0, count: data.Length);
            pipe.WaitForPipeDrain();
        }
    }
}
