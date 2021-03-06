﻿using Akka.Actor;
using RTS.Core.Structs;
using RTS.Entities.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Factories
{
    public class TeamFactory: IActorFactory
    {
        private ActorSystem _context;
        private static long _teamId;
        public TeamFactory(ActorSystem context, int teamId = 0)
        {
            _context = context;
            _teamId = teamId;
        }
    
        public ActorRef GetEntity(out long id)
        {
            long teamId = _teamId++;
            Props props = new Props(Deploy.Local, typeof(Team), new List<object> { _context, teamId });
            ActorRef entity = _context.ActorOf(props, "Team" + teamId);

            id = teamId;

            return entity;
        }
    }
}
