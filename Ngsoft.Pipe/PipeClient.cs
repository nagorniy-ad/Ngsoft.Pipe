using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace Ngsoft.Pipe
{
    public class PipeClient : PipeBase
    {
        public PipeClient(string pipeName, Encoding encoding) : base(pipeName, encoding) { }

        public async Task<string> SendMessage(string message, int timeout = 10000)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be empty.", nameof(message));
            }
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout value cannot be negative.");
            }

            using (var client = new NamedPipeClientStream(serverName: ".", pipeName: PipeName, direction: PipeDirection.InOut))
            {
                client.Connect(timeout);
                client.ReadMode = PipeTransmissionMode.Message;
                await Task.Run(() =>
                {
                    WriteOutput(client, message);
                });
                return await Task.Run(() =>
                {
                    return ReadInput(client);
                });
            }
        }
    }
}
