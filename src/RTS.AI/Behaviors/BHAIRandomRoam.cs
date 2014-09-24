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

namespace RTS.AI.Behaviors
{
    public class BHAIRandomRoam : BehaviorBase
    {
        private IEntity _entity;
        private DateTime _lastRoam;
        public BHAIRandomRoam(Brain brain)
            : base(brain)
        {
            _entity = _brain.Entity;

            Initialize = () => { Console.WriteLine("BHAIRandomRoam Initialize"); };
            Update = () => { Roam(); };
        }

        private void Roam()
        {
            if ((DateTime.Now - _lastRoam).TotalSeconds < 15)
                return;

            _lastRoam = DateTime.Now;

            //Console.WriteLine("Roaming.");

            Random rng = new Random();

            Vector3 randomPosition = new Vector3(rng.Next(-100, 100), 0, rng.Next(-100, 100)) + _entity.Position;

            MoveUnitsCommand command = new MoveUnitsCommand()
            {
                EntityIds = new List<long>() { _entity.Id },
                Position = randomPosition
            };

            _entity.MessageTeam(command);

            //Completed();
        }

  
    }
}
