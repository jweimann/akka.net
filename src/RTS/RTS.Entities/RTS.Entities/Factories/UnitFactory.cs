using Akka.Actor;
using RTS.ContentRepository;
using RTS.Core.Structs;
using RTS.DataStructures;
using RTS.Entities.Structs;
using RTS.Entities.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Factories
{
    public class UnitFactory: IEntityFactory
    {
        private ActorSystem _context;
        private UnitDefinitionRepository _repository;
        private static Int64 _nextEntityId = 10000;
        public UnitFactory(ActorSystem context)
        {
            _context = context;
            _repository = new UnitDefinitionRepository();
            //_nextEntityId += DateTime.Now.Ticks;
        }
        public ActorRef GetEntity(ActorRef teamActor, out long entityId, out Stats.Stats stats)
        {
            SpawnEntityData data = new SpawnEntityData() { TeamActor = teamActor };
            return GetEntity(data, out entityId, out stats);
        }

        public ActorRef GetEntity(SpawnEntityData data, out long entityId, out Stats.Stats stats)
        {
            entityId = _nextEntityId++;

            UnitDefinition unitDefinition = _repository.Get(data.UnitType);
            stats = new Stats.Stats(unitDefinition);

            List<IEntityComponent> args = new List<IEntityComponent>();
            List<IEntityComponent> components = new List<IEntityComponent>();
            components.Add(new Vehicle());
            components.Add(new Weapon());
            components.Add(stats);

            var definition = _repository.Get(data.UnitType);
            if (definition.CanBuild != null)
            {
                components.Add(new Buildings.Building());
            }
            
            Props props = new Props(Deploy.Local, typeof(Entity), new List<object> { entityId, components, data });
            ActorRef entity = _context.ActorOf(props, "Entity" + entityId);

            return entity;
        }
    }
}
