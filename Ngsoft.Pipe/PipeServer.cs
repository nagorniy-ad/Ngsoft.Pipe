using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ngsoft.Pipe
{
    public class PipeServer : PipeBase
    {
        private readonly Func<string, string> _messageHandler;

        public PipeServer(string pipeName, Func<string, string> messageHandler, Encoding encoding) : base(pipeName, encoding)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        public async Task Run(CancellationToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            while (token.IsCancellationRequested == false)
            {
                using (var server = new NamedPipeServerStream(pipeName: PipeName, direction: PipeDirection.InOut, maxNumberOfServerInstances: NamedPipeServerStream.MaxAllowedServerInstances, transmissionMode: PipeTransmissionMode.Message))
                {
                    server.WaitForConnection();
                    try
                    {
                        var input = await Task.Run(() =>
                        {
                            return ReadInput(server);
                        });
                        var output = _messageHandler.Invoke(input);
                        if (string.IsNullOrWhiteSpace(output))
                        {
                            throw new InvalidOperationException("Message handler result cannot be empty.");
                        }
                        await Task.Run(() =>
                        {
                            WriteOutput(server, output);
                        });
                        server.Disconnect();
                    }
                    catch
                    {
                        if (server.IsConnected)
                        {
                            server.Disconnect();
                        }
                        throw;
                    }
                }
            }
        }
    }
}
