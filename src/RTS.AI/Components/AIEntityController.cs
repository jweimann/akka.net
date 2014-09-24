using JW.Behavior;
using RTS.AI.Behaviors;
using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.AI.Components
{
    public class AIEntityController : IEntityController
    {
        private Dictionary<long, IEntity> _entities = new Dictionary<long, IEntity>();
        private DateTime _lastUpdate;
        private AIBot _bot;

        public AIEntityController(AIBot bot)
        {
            _lastUpdate = DateTime.Now;
            _bot = bot;
        }
        public void AddEntity(long entityId, string name, Core.Structs.Vector3 position, object sender)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(long EntityId)
        {
            _entities[EntityId].IsAlive = false;
        }

        public void Update()
        {
            double deltaTime = (DateTime.Now - _lastUpdate).TotalMilliseconds;
            _lastUpdate = DateTime.Now;

            List<long> keys = new List<long>(_entities.Keys);

            foreach (var key in keys)
            {
                var entity = _entities[key];
                entity.Brain.Tick(deltaTime);
            }
        }

        public Dictionary<long, IEntity> GetEntities()
        {
            return _entities;
        }

        public void SetBounds(Core.Structs.Bounds Bounds)
        {
            throw new NotImplementedException();
        }

        public void MoveEntity(object sendMovementToAOICommand)
        {
            throw new NotImplementedException();
        }

        public void SendEntitiesInAreaListToEntity(object sender)
        {
            throw new NotImplementedException();
        }

        public int GetId()
        {
            throw new NotImplementedException();
        }

        public void SpawnEntity(Core.Enums.UnitType unitType, Core.Structs.Vector3 position, long entityId, long teamId, List<RTS.DataStructures.Stat> stats)
        {
            var aiEntity = new AIEntity(unitType, entityId, position, teamId, this);
            _entities.Add(entityId, aiEntity);
            
            if (teamId == this._bot.Team)
                aiEntity.Brain.AddBehavior(new BHAIRandomAttack(aiEntity.Brain as Brain, this));
        }

        public void SetUnitPath(long unitId, List<Core.Structs.Vector3> path)
        {
            //throw new NotImplementedException();
        }

        public void HandleRequest(object request)
        {
            throw new NotImplementedException();
        }

        internal IEntity RandomBuilding()
        {
            if (_bot.Team == 0)
                return null;

            foreach (var entity in _entities.Values)
            {
                var aiEntity = entity as AIEntity;
                if (aiEntity.UnitType == UnitType.BaseStation &&
                    aiEntity.TeamId == _bot.Team)
                    return entity;
            }
            return null;
        }

        internal void SendCommandToPlayer(IMmoCommand message)
        {
            _bot.SendCommandToPlayer(message);
        }
    }
}
