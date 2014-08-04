using Akka.Actor;
using RTS.Core.Structs;
using RTS.Entities.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Factories
{
    public class BuildingFactory : IEntityFactory
    {
        private ActorSystem _context;
        private static Int64 _nextEntityId = 1000;
        public BuildingFactory(ActorSystem context)
        {
            _context = context;
        }
        public ActorRef GetEntity(ActorRef teamActor, out long entityId)
        {
            SpawnEntityData data = new SpawnEntityData() { TeamActor = teamActor };
            return GetEntity(data, out entityId);
        }

        public ActorRef GetEntity(SpawnEntityData data, out long entityId)
        {
            entityId = _nextEntityId++;

            List<IEntityComponent> args = new List<IEntityComponent>();

            List<IEntityComponent> components = new List<IEntityComponent>();
            components.Add(new Building());
            Props props = new Props(Deploy.Local, typeof(Entity), new List<object> { entityId, components, data });
            ActorRef entity = _context.ActorOf(props, "Building_" + entityId);

            return entity;
        }
    }
}
