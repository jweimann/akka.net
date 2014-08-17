using Akka.Actor;
using Helios.Net;
using Helios.Ops.Executors;
using Helios.Reactor;
using Helios.Reactor.Bootstrap;
using Helios.Topology;
using RTS.Commands.Server;
using RTS.Core.Structs;
using RTS.Entities.Factories;
using RTS.Entities.Movement;
using RTS.Entities.Requests;
using RTS.Networking;
using RTS.Networking.Helios;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Client
{
    public class ClientProxyListenerActor : TypedActor
    {
        private ActorSelection _clientProxyCollectionActor;
        private List<MmoNetworkClient> _clients = new List<MmoNetworkClient>();
        private Dictionary<IConnection, ActorRef> _connections = new Dictionary<IConnection, ActorRef>();
        private IPlayerFactory _factory;
        private TeamFactory _teamFactory;

        private IReactor _server;
        private IServerFactory _bootstrapper;
        public ClientProxyListenerActor()
        {
            _clientProxyCollectionActor = Context.System.ActorSelection("akka.tcp://MyServer@localhost:8081/user/ClientProxyCollection"); ;
        }
        protected override void PreStart()
        {
            _factory = new PlayerFactory(Context.System);
            _teamFactory = new TeamFactory(Context.System);

            var executor = new TryCatchExecutor(exception => Console.WriteLine("Unhandled exception: {0}", exception));

            _bootstrapper =
                new ServerBootstrap()
                    .WorkerThreads(2)
                    .Executor(executor) 
                    .SetTransport(TransportType.Tcp)
                    .Build();
            _server = _bootstrapper.NewReactor(NodeBuilder.BuildNode().Host(IPAddress.Any).WithPort(7331));
            _server.OnConnection += (address, connection) =>
            {
                AcceptClient(connection);
                Console.WriteLine("Connected: {0}", address);
            };
            _server.OnDisconnection += (reason, address) =>
                {
                    Console.WriteLine("Disconnected: {0}; Reason: {1}", address.RemoteHost, reason.Type);
                    DisconnectClient(address);
                };
            _server.Start();

            base.PreStart();
        }

        private void DisconnectClient(IConnection address)
        {
            return;
            if (_connections.ContainsKey(address))
            {
                var players = Context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Player*");
                players.Tell(new PlayerDisconnectedCommand() { PlayerActor = _connections[address] });
                _connections.Remove(address);
            }
        }

        private void AcceptClient(IConnection connection)
        {
            var player = _factory.GetPlayer(connection);
            long teamId;
            var team = _teamFactory.GetEntity(out teamId);
            team.Tell(new SetPlayerCommand() { PlayerActor = player }); // Tell the team what it's player actor is
            player.Tell(new SetTeamCommand() { TeamActor = team }); // Tell the player what it's team actor is

            _connections.Add(connection, player);
        }

    }
}
