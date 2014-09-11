using Akka.Actor;
using Helios.Net;
using RTS.ActorRequests;
using RTS.Core.Structs;
using RTS.Networking.Helios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Pathfinding
{
    public class PathingActor : UntypedActor
    {
        List<ActorSelection> _clientConnections = new List<ActorSelection>();

        public PathingActor()
        {
            //this.Self.Path.ToString()
        }
        protected override void OnReceive(object message)
        {
            if (message is GetPathRequest)
            {
                GetPathRequest request = (GetPathRequest)message;
                var sender = Sender;
                RespondWithPath(request, sender);
            }
            if (message is string)
            {
                string path = message as string;
                ActorSelection selection = Context.ActorSelection(path);
                _clientConnections.Add(selection);
            }
        }

        private async void RespondWithPath(GetPathRequest request,  ActorRef sender)
        {
            List<Vector3> path = await RequestPathFromClient(request);
            sender.Tell(path);
        }

        private async Task<List<Vector3>> RequestPathFromClient(GetPathRequest request)
        {
            List<Vector3> path = null;
            // ask last temporarily until i get the cleanup or roundrobin right.

            return await _clientConnections.Last().Ask<List<Vector3>>(request);

            foreach(var connection in _clientConnections)
            {
                path = await connection.Ask<List<Vector3>>(request);
                if (path != null && path.Count > 0)
                    break;
            }
            if (path == null || path.Count == 0)
            {
                //throw new Exception("Unable to get a path to destination " + request.Destination);
            }
            return path;
        }
    }
}
