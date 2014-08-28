using Akka.Actor;
using Helios.Net;
using RTS.ActorRequests;
using RTS.Commands.Pathing;
using RTS.Core.Structs;
using RTS.Networking.Helios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Pathfinding
{
    public class PathingClientConnectionActor : UntypedActor
    {
        private RTSHeliosNetworkClient _connection;
        private int _nextRequestId;
        private Dictionary<int, ActorRef> _clientRequests;
        public PathingClientConnectionActor(RTSHeliosNetworkClient connection)
        {
            _clientRequests = new Dictionary<int, ActorRef>();
            _connection = connection;
            _connection.CommandRecieved += _connection_CommandRecieved;
        }

        void _connection_CommandRecieved(object sender, Commands.MmoCommand command)
        {
            if (command is ResolvePathCommand)
            {
                ResolvePathCommand request = (ResolvePathCommand)command;
                var requester = _clientRequests[request.RequestId];
                requester.Tell(request.Path);
            }
        }

        protected override void PreStart()
        {
            ActorSelection pathingActor = Context.System.ActorSelection("user/PathingActor");
            pathingActor.Tell(this.Self.Path.ToString());
        }
        
        protected override void OnReceive(object message)
        {
           if (message is GetPathRequest)
           {
               GetPathRequest request = (GetPathRequest)message;
               RequestPathCommand requestPathCommand = GenerateCommand(request);
               _connection.SendCommand(requestPathCommand);
               
               _clientRequests.Add(requestPathCommand.RequestId, Sender);

               //List<Vector3> path = new List<Vector3>() { request.Destination };
               //Sender.Tell(path);
           }
        }

        private RequestPathCommand GenerateCommand(GetPathRequest request)
        {
            RequestPathCommand command = new RequestPathCommand()
            {
                Destination = request.Destination,
                StartLocation = request.StartPosition
            };
            command.RequestId = _nextRequestId++;
            return command;
        }
    }
}
