using Akka.Actor;
using RTS.Entities.Interfaces.Player;
using RTS.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Factories
{
    public class AIBotFactory : IPlayerFactory
    {
        private ActorSystem _context;
        private Int64 _nextEntityId = 500;

        public AIBotFactory(ActorSystem context)
        {
            _context = context;
        }
        public ActorRef GetPlayer(IPlayerConnection client)
        {
            List<IPlayerComponent> args = new List<IPlayerComponent>();

            //args.Add(new AIBot());

            Int64 entityId = _nextEntityId++;

            Props props = new Props(Deploy.Local, typeof(Player.Player), new List<object> { null, args });
            ActorRef entity = _context.ActorOf(props, "Player" + entityId);
            return entity;
        }
    }
}
