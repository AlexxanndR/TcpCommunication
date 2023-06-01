using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP
{
    public class TCPClient
    {
        public delegate void MessageReceivedEventHandler(string message);
        public event MessageReceivedEventHandler? MessageReceived;

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;

        private string _hostname;
        private uint _port;

        public TCPClient()
        {
            _tcpClient = new TcpClient();
        }

        async private Task ReceiveMessageAsync()
        {
            string? message = null;

            StreamReader reader = new StreamReader(_networkStream, Encoding.ASCII);

            message = await reader.ReadLineAsync();
            MessageReceived?.Invoke(message);
        }

        async public Task ConnectAsync(string hostname, uint port)
        {
            _hostname = hostname;
            _port = port;

            if (_tcpClient.Connected)
                _tcpClient.Close();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Task connectTask = _tcpClient.ConnectAsync(_hostname, (int)_port);
            Task completeTask = await Task.WhenAny(connectTask, Task.Delay(500, cancellationToken));

            if (completeTask != connectTask)
            {
                cancellationTokenSource.Cancel();
                throw new TimeoutException("Connection timeout occured.");
            }

            _networkStream = _tcpClient.GetStream();

            while (true)
                await ReceiveMessageAsync();
        }

        public void CloseConnection()
        {
            if (_tcpClient.Connected)
                _tcpClient.Close();
        }

        ~TCPClient() => CloseConnection();
    }
}
