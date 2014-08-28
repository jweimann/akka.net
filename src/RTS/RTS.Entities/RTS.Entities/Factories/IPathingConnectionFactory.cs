using Akka.Actor;
using Helios.Net;
using RTS.Networking.Helios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Factories
{
    public interface IPathingConnectionFactory
    {
        ActorRef GetActor(RTSHeliosNetworkClient client);
    }
}
