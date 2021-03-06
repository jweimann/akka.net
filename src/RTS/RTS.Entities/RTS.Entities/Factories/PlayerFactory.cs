﻿using Akka.Actor;
using Helios.Net;
using RTS.Entities.Client;
using RTS.Entities.Interfaces.Player;
using RTS.Networking.Helios;
using RTS.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Factories
{
    public class PlayerFactory : IPlayerFactory
    {
        private ActorSystem _context;
        private Int64 _nextEntityId = 250;

        public PlayerFactory(ActorSystem context)
        {
            _context = context;
        }
        public ActorRef GetPlayer(IPlayerConnection client)
        {
            List<IPlayerComponent> args = new List<IPlayerComponent>();

            args.Add(new ClientController(client));
            //args.Add(new Team(_context));

            Int64 entityId = _nextEntityId++;

            Props props = new Props(Deploy.Local, typeof(Player.Player), new List<object> { client, args });
            ActorRef entity = _context.ActorOf(props, "Player" + entityId);
            return entity;
        }
    }
}