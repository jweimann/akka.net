using Helios.Net;
using RTS.Commands;
using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace RTS.Networking.Helios
{
    public class RTSHeliosNetworkClient : IPlayerConnection
    {
        private static int _idIndex = 500;

        private IConnection _connection;
        private int _id = _idIndex++;
        public IConnection Connection { get { return _connection; } }
        //public delegate void CommandRecievedEventHandler(object sender, MmoCommand command);
        public event CommandRecievedEventHandler CommandRecieved;
        public int Id { get { return _id; } }
        public RTSHeliosNetworkClient(IConnection connection)
        {
            _connection = connection;
            _connection.BeginReceive(_connection_Receive);//
            _connection.OnDisconnection += _connection_OnDisconnection;
            //_connection.Receive += _connection_Receive;
        }

        void _connection_OnDisconnection(global::Helios.Exceptions.HeliosConnectionException reason, IConnection closedChannel)
        {
            _connection = null;
        }

        void _connection_Receive(NetworkData incomingData, IConnection responseChannel)
        {
            try
            {
                //Console.WriteLine(String.Format("{0}  ConnectionReceive  Host: {1}  ConnectionId: {2}  ConnectionAddress: {3}", DateTime.Now.ToString(), responseChannel.RemoteHost.ToString(), _id, _connection.RemoteHost.ToString()));
                if (incomingData.Buffer.Length == 0)
                {
                    return;
                }
                MmoCommand command = MmoCommand.FromBytes<MmoCommand>(incomingData.Buffer);
                if (CommandRecieved != null && command != null)
                {
                    if (command is MmoCommand)
                    {
                        CommandRecieved(this, command as IMmoCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void SendCommand<T>(IMmoCommand<T> command)
        {
            if (command.CommandDestination == Destination.Server)
            {
                return; // Don't send server only commands to a client.
            }

            if (_connection == null || _connection.IsOpen() == false)// || _connection.Available == 0)
            {
                return;
            }

            Console.WriteLine("Sending Message to Client: " + command.ToString());

            var bytes = command.ToBytes();
            var networkData = new NetworkData()
            {
                Buffer = bytes,
                Length = bytes.Length,
                RemoteHost = _connection.RemoteHost
            };
            try
            {
                _connection.Send(networkData);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("EXCEPTION Sending Command to Client: " + ex.ToString());
            }
        }
        public void SendCommand(IMmoCommand command)
        {
            //if (command.CommandDestination == Destination.Server)
            //{
            //    return; // Don't send server only commands to a client.
            //}

            if (_connection == null || _connection.IsOpen() == false)// || _connection.Available == 0)
            {
                return;
            }

            var bytes = command.ToBytes();
            var networkData = new NetworkData()
            {
                Buffer = bytes,
                Length = bytes.Length,
                RemoteHost = _connection.RemoteHost
            };
            try
            {
                _connection.Send(networkData);
            }
            catch (SocketException ex)
            {

            }
        }
        public void WriteString(string value)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            var networkData = new NetworkData()
            {
                Buffer = bytes,
                Length = bytes.Length,
                RemoteHost = _connection.RemoteHost
            };

            _connection.Send(networkData);
        }

        
    }
}
