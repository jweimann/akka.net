using RTS.Core.Structs;
using RTS.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Networking
{
    public class MmoNetworkClient : IMmoNetworkClient
    {
        TcpClient _client;
        NetworkStream _stream;
        object _state;
        object _lock = new object();

        public MmoNetworkClient(TcpClient client)
        {
            _client = client;
        }
        public void SetClient(TcpClient client)
        {
            _client = client;
        }

        public void WriteVector2(Vector2 value)
        {
            byte[] bytes = value.ToBytes();
            WriteBytes(bytes);
        }
        private void WriteBytes(byte[] bytes)
        {
            lock (_lock)
            {
                if (_client == null || _client.Connected == false)
                {
                    return;
                }

                _stream = _client.GetStream();
                _stream.Write(bytes, 0, bytes.Length);
                Console.WriteLine("Sent: {0}", bytes.Length);
            }
        }
        public void WriteToClient(string message)
        {
            lock (_lock)
            {
                if (_client == null || _client.Connected == false)
                {
                    return;
                }
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing. 
                //  Stream stream = client.GetStream();

                _stream = _client.GetStream();
                {
                    //_stream.Seek(0, System.IO.SeekOrigin.Begin);
                    // Send the message to the connected TcpServer. 
                    _stream.Write(data, 0, data.Length);

                    Console.WriteLine("Sent: {0}", message);

                    // Receive the TcpServer.response. 

                    // Buffer to store the response bytes.
                    data = new Byte[256];

                    // String to store the response ASCII representation.
                    String responseData = String.Empty;

                    //// Read the first batch of the TcpServer response bytes.
                    //Int32 bytes = _stream.Read(data, 0, data.Length);
                    //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    //Console.WriteLine("Received: {0}", responseData);

                    //stream.Close();

                    // Close everything.
                }
                //_client.Close();
                //_listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), _state);
            }
        }
        public void WriteString(string value)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            WriteBytes(bytes);
        }


        public void WriteInt32(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBytes(bytes);
        }
    }
}
