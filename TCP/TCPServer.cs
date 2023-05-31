using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCP
{
    public class TCPServer
    {
        private TcpClient _tcpClient;
        private TcpListener _tcpListener;
        private NetworkStream _networkStream;

        private string _hostname;
        private uint _port;

        public TCPServer()
        {
            _tcpClient = null;
            _tcpListener = null;
        }

        async public Task ListenAsync(string hostname, uint port)
        {
            _hostname = hostname;
            _port = port;

            _tcpListener = new TcpListener(IPAddress.Parse(_hostname), (int)_port);
            _tcpListener.Start();

            _tcpClient = await _tcpListener.AcceptTcpClientAsync();

            _networkStream = _tcpClient.GetStream();
        }

        public void CloseConnection()
        {
            if (_tcpClient.Connected)
                _tcpClient.Close();

            _tcpListener.Stop();
        }

        async public Task SendMessageAsync(string message)
        {
            StreamWriter writer = new StreamWriter(_networkStream, Encoding.ASCII);

            await writer.WriteAsync(message);
            await writer.FlushAsync();
        }

        ~TCPServer() => CloseConnection();
    }
}
