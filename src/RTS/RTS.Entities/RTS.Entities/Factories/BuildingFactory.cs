﻿using Akka.Actor;
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
        private static Int64 _nextEntityId = 1000; //STATIC - Move to a generator service.

        private ActorSystem _context;
        private ContentRepository.UnitDefinitionRepository _repository;
        public BuildingFactory(ActorSystem context)
        {
            _context = context;
            _repository = new ContentRepository.UnitDefinitionRepository();
            
            Random rng = new Random();
        }
        public ActorRef GetEntity(ActorRef teamActor, out long entityId, out Stats.Stats stats)
        {
            SpawnEntityData data = new SpawnEntityData() { TeamActor = teamActor };
            return GetEntity(data, out entityId, out stats);
        }

        public ActorRef GetEntity(SpawnEntityData data, out long entityId, out Stats.Stats stats)
        {
            entityId = _nextEntityId++;

            var definition = _repository.Get(data.UnitType);
            stats = new Stats.Stats(definition);

            List<IEntityComponent> args = new List<IEntityComponent>();

            List<IEntityComponent> components = new List<IEntityComponent>();
            components.Add(new Building());
            components.Add(new ResourceGenerator() { Amount = definition.ResourceAmount, Interval = definition.ResourceInterval });
            components.Add(stats);

            Props props = new Props(Deploy.Local, typeof(Entity), new List<object> { entityId, components, data });
            ActorRef entity = _context.ActorOf(props, "Entity" + entityId);

            return entity;
        }
    }
}
