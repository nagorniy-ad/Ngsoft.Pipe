# Ngsoft.Pipe
## Quick use
### Server
Example for console application:
```c#
var cts = new CancellationTokenSource();
Task.Factory.StartNew(() =>
{
  var server = new PipeServer("test", messageHandler: m => $"{m ?? string.Empty}_received", Encoding.UTF8);
  server.Run(cts.Token);
}, TaskCreationOptions.LongRunning);
Console.WriteLine("Server started. Press enter to stop it and exit.");
Console.ReadLine();
```
Ensure that message handler never return ```null``` value.
### Client
Example for console application:
```c#
while (true)
{
  var client = new PipeClient("test", Encoding.UTF8);
  Console.Write("Enter message: ");
  var message = Console.ReadLine();
  var response = await client.SendMessage(message);
  Console.WriteLine(response);
}
```
Ensure that client and server using the same pipe name.
