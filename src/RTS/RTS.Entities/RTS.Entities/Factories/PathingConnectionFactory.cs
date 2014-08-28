using Akka.Actor;
using RTS.Networking.Helios;
using RTS.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Factories
{
    public class PathingConnectionFactory : IPathingConnectionFactory
    {
        private ActorSystem _context;

        public PathingConnectionFactory(ActorSystem context)
        {
            _context = context;
        }
        public Akka.Actor.ActorRef GetActor(RTSHeliosNetworkClient client)
        {
            Props props = new Props(Deploy.Local, typeof(PathingClientConnectionActor), new List<object> { client });
            ActorRef entity = _context.ActorOf(props, "PathingClientConnectionActor_" + client.Connection.RemoteHost.Host);
            return entity;
        }
    }
}