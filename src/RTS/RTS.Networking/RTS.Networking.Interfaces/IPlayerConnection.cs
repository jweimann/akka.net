using RTS.Commands.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Networking.Interfaces
{
    public delegate void CommandRecievedEventHandler(object sender, IMmoCommand command);
    public interface IPlayerConnection
    {
        void SendCommand<T>(IMmoCommand<T> command);
        event CommandRecievedEventHandler CommandRecieved;
    }
}
