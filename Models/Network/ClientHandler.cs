using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Caro.Models.Network
{
    public class ClientHandler
    {
        private TcpClient       _tcpClient;
        private StreamWriter    _writer         { get; set; }
        private NetworkStream   _networkStream  { get; set; }
        public EventHandler<string>? ReceivedMessage;

        // Buffer to store received data
        private byte[] buffer = new byte[1024];

        public ClientHandler(TcpClient tcpClient)
        {
            _tcpClient      = tcpClient;
            _networkStream  = _tcpClient.GetStream();
            _writer         = new StreamWriter(_networkStream) { 
                                                AutoFlush = true
                                            };
        }

        // Thread to start receiving message
        public async void StartReceivingMessageAsync()
        {
            await ReadMessageAsync();
        }

        public async Task ReadMessageAsync()
        {
            try 
            {
                // Always listening for incoming messages until the connection is closed
                while(true) 
                {
                    int bytes = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytes == 0)
                    {
                        return;
                    }

                    string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytes);
                    // Up to the client to handle the message
                    ReceivedMessage?.Invoke(this, message);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Console.WriteLine("Connection is closed!");
            }
        }


        // Send message to server
        public void SendMessage(string message)
        {
            try 
            {
                _writer.Write(message);
            }
            catch (Exception)
            {
                Console.WriteLine("Connection is closed!");
            }
        }

        public void Disconnected()
        {
            SendMessage("<QUIT>");
        }
    }
}