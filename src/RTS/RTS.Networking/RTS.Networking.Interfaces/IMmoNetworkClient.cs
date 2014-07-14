using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace RTS.Networking.Interfaces
{
    public interface IMmoNetworkClient
    {
        void SetClient(TcpClient client);
        void WriteVector2(Vector2 value);
        void WriteInt32(int value);
        void WriteString(string value);
        void WriteToClient(string message);
    }
}
