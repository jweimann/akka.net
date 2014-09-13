using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTS.Core.Structs;
using RTS.Entities.Units;
using RTS.Entities.Interfaces.UnitTypes;
using RTS.DataStructures;
using Akka.Actor;
using RTS.ActorRequests;
using RTS.Core.Enums;
using RTS.Commands.Buildings;
using RTS.Entities.Interfaces;
using JW.Behavior;
using JW.Behavior.Interfaces;

namespace RTS.Entities.Behaviors
{
    public class BHBuildAtLocation : BehaviorBase
    {
        private IEntity _entity;
        public BHBuildAtLocation(Brain brain, Vector3 position, UnitType unitType) : base(brain)
        {
            _entity = _brain.Entity;

            Initialize = () => { Console.WriteLine("BHBuildAtLocation Initialize"); };
            Update = () => { Build(position, unitType); };
        }

        private void Build(Vector3 position, UnitType unitType)
        {
            Console.WriteLine("Building At Destination.");
            _entity.MessageTeam(new FinishBuildEntityCommand() { UnitType = unitType, Position = position });

            Completed();
        }

  
    }
}
