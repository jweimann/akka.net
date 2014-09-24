using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.UnitTypes;
using RTS.DataStructures;
using Akka.Actor;
using RTS.Core.Enums;
using RTS.Commands.Buildings;
using RTS.Entities.Interfaces;
using JW.Behavior;
using JW.Behavior.Interfaces;
using RTS.Commands;
using RTS.Commands.Units;
using RTS.Entities.Interfaces.Control;

namespace RTS.AI.Behaviors
{
    public class BHAIRandomAttack : BehaviorBase
    {
        private IEntity _entity;
        private DateTime _lastTargetAquire;
        private IEntity _target;
        private IEntityController _entityController;
        public BHAIRandomAttack(Brain brain, IEntityController entityController)
            : base(brain)
        {
            _entity = _brain.Entity;
            _entityController = entityController;

            Initialize = () => { Console.WriteLine("BHAIRandomAttack Initialize"); };
            Update = () => { FindAndAttack(); };
        }

        private void FindAndAttack()
        {
            //if (_target != null && _target.IsAlive)
            //    return;

            if ((DateTime.Now - _lastTargetAquire).TotalSeconds < 15)
                return;

            _lastTargetAquire = DateTime.Now;

            var possibleTargets = _entityController.GetEntities().Values.Where(t => t.TeamId != this._entity.TeamId).ToList();
            if (possibleTargets.Count == 0)
                return;

            Random rng = new Random();
            _target = possibleTargets[rng.Next(0, possibleTargets.Count)];

            SetTargetCommand command = new SetTargetCommand()
            {
                EntityId = _entity.Id,
                TargetEntityId = _target.Id
            };

            _entity.MessageTeam(command);

            //Completed();
        }

  
    }
}
