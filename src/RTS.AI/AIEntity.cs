using JW.Behavior;
using RTS.AI.Behaviors;
using RTS.AI.Components;
using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.AI
{
    public class AIEntity : IEntity
    {
        private AIEntityController _entityController;
        public UnitType UnitType { get; private set; }
        public long Id { get; private set; }
        public Core.Structs.Vector3 Position { get; set; }
        public JW.Behavior.Interfaces.IBrain Brain { get; private set; }
        public long TeamId { get; private set; }
        public bool IsAlive { get; set; }
        public AIEntity(UnitType unitType, long id, Vector3 position, long team, AIEntityController entityController)
        {
            this.UnitType = unitType;
            this.Id = id;
            this.Brain = new Brain(this, null, null);
            this._entityController = entityController;
            this.Position = position;
            this.TeamId = team;
            this.IsAlive = true;
        }

        public List<Entities.IEntityComponent> Components
        {
            get { throw new NotImplementedException(); }
        }

        public void MessageComponents(object message)
        {
            throw new NotImplementedException();
        }

        public void AddComponent(Entities.IEntityComponent component)
        {
            throw new NotImplementedException();
        }

        public object GetActorRef()
        {
            throw new NotImplementedException();
        }

        public object GetActorContext()
        {
            throw new NotImplementedException();
        }

        public void MessageTeam(object message)
        {
            IMmoCommand command = message as IMmoCommand;
            _entityController.SendCommandToPlayer(command);
        }

        public void MessagePlayer(object message)
        {
            throw new NotImplementedException();
        }

        public Core.Structs.SpawnEntityData GetSpawnEntityData()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        
    }
}
