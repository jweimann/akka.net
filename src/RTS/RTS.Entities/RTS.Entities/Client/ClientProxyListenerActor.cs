﻿using Akka.Actor;
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
        private List<IConnection> _connections = new List<IConnection>();
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
                };
            _server.Start();

            base.PreStart();
        }

        private void AcceptClient(IConnection connection)
        {
            _connections.Add(connection);

            var player = _factory.GetEntity(connection);
            long teamId;
            var team = _teamFactory.GetEntity(out teamId);
            team.Tell(new SetPlayerCommand() { PlayerActor = player }); // Tell the team what it's player actor is
            player.Tell(new SetTeamCommand() { TeamActor = team }); // Tell the player what it's team actor is
            player.Tell(new SetClientPlayerInfoCommand() { TeamId = teamId });


            //_clientProxyCollectionActor.Tell(new AcceptClientConnectionRequest() { Connection = connection });
        }




        /*

        public static void Receive(NetworkData data, IConnection channel)
        {
            var rawCommand = Encoding.UTF8.GetString(data.Buffer);
            var commands = rawCommand.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)); //we use the pipe to separate commands
            foreach (var command in commands)
            {
                //Console.WriteLine("Received: {0}", command);
                if (command.ToLowerInvariant() == "gettime")
                {
                    var time = Encoding.UTF8.GetBytes(DateTime.Now.ToLongTimeString());
                    channel.Send(new NetworkData() { Buffer = time, Length = time.Length, RemoteHost = channel.RemoteHost });
                    //Console.WriteLine("Sent time to {0}", channel.Node);
                }
                else
                {
                    Console.WriteLine("Invalid command: {0}", command);
                    var invalid = Encoding.UTF8.GetBytes("Unrecognized command");
                    channel.Send(new NetworkData() { Buffer = invalid, Length = invalid.Length, RemoteHost = channel.RemoteHost });
                }
            }
        }

     */

     

    }
}
