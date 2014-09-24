using Akka.Actor;
using Helios.Net;
using RTS.Networking.Helios;
using RTS.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Factories
{
    public interface IPlayerFactory
    {
        ActorRef GetPlayer(IPlayerConnection client);
    }
}
