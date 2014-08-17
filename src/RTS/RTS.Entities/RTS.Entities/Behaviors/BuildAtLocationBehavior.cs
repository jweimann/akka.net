﻿using BehaviorTreeLibrary;
using RTS.Commands.Buildings;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Behaviors
{
    public class BuildAtLocationBehavior : Sequence
    {
        private Vehicle _vehicle;
        private Vector3 _destination;
        private UnitType _unitType;
        public BuildAtLocationBehavior(Vehicle vehicle, Vector3 destination, UnitType unitType)
        {
            _vehicle = vehicle;
            _destination = destination;
            _unitType = unitType;

            this.AddCondition(ReachedDestination)
                .AddBehavior(BuildEntity)
                .AddBehavior(StopVehilcle);
            //Add<Condition>().CanRun = ReachedDestination;
            //Add<Behavior>().Update = BuildEntity;
        }
     
        private bool ReachedDestination()
        {
            float dist = Vector3.Distance(_vehicle.GetPosition(), _destination);
            return dist <= _vehicle.GetMoveThreshhold();
        }

        private Status BuildEntity()
        {
            float dist2 = Vector3.Distance(_vehicle.GetPosition(), _destination);
            Console.WriteLine("Reached Destination.  Distance " + dist2.ToString());
            _vehicle.SendCommandToTeam(new FinishBuildEntityCommand() { UnitType = _unitType, Position = _destination });

            return BehaviorTreeLibrary.Status.BhSuccess;
        }

        private Status StopVehilcle()
        {
            _vehicle.Stop();
            return BehaviorTreeLibrary.Status.BhSuccess;
        }
    }
}
