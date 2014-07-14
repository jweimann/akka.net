using Akka.Actor;
using Helios.Net;
using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Factories
{
    public class NPCEntityFactory : IEntityFactory
    {
        private ActorSystem _context;
        private static Int64 _nextEntityId = 1000;

        public NPCEntityFactory(ActorSystem context)
        {
            _context = context;
        }

        public ActorRef GetEntity()
        {
            SpawnEntityData data = new SpawnEntityData();
            return GetEntity(data);
        }

        public ActorRef GetEntity(SpawnEntityData data)
        {
            List<IEntityComponent> args = new List<IEntityComponent>();

            args.Add(new Stats.Stats());
            //args.Add(new Movement.NpcMover(data));
            args.Add(new Movement.RTSGroundMover(data));

            Int64 entityId = _nextEntityId++;

            Props props = new Props(Deploy.Local, typeof(Entity), new List<object> { entityId, args });
            ActorRef entity = _context.ActorOf(props, "NPC_" + entityId);
            return entity;
        }

    
    }
}
