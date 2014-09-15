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
using JW.Behavior;
using JW.Behavior.Interfaces;

namespace RTS.Entities.Behaviors
{
    public class BHMoveToLocation : BehaviorBase
    {
        private MovementPath _path;
        private bool _isRequestingPath;
        private float _threshhold;
        public BHMoveToLocation(Brain brain, Vector3 position, float threshhold) : base (brain)
        {
            _threshhold = threshhold;

            Initialize = () => { Console.WriteLine("MoveToLocationBehavior Initialize"); };
            Update = () => { Move(position, threshhold); };
        }

        private async void Move(Vector3 position, float threshhold)
        {
            if (!HasPath()) // Refactor this down to 2 lines..
            {
                if (_isRequestingPath == false)
                {
                    _isRequestingPath = true;
                    _path = await GetPathToDestination(position);
                    _isRequestingPath = false;
                    if (_path != null)
                    {
                        this.Vehicle.SetPosition(_path.NextPosition());
                        this.Vehicle.SendPathToClients(_path);
                    }
                }
                return;
            }

            if (_path.IsValid == false)
            {
                _path = null; // Add a remove handler here and subscribe with the behavior list.

                if (Completed != null)
                    Completed();

                this.Vehicle.SendStopPathToClients();

                return;
            }

            this.Vehicle.MoveBy(MoveAmount());

            if (ReachedWaypoint())
                _path.IncrementPoint();
        }

        private Vector3 DirectionToNextPoint()
        {
            return (_path.NextPosition() - this.Vehicle.GetPosition()).Normalized();
        }
        private Vector3 MoveAmount()
        {
            return (DirectionToNextPoint() * this.Vehicle.GetSpeed() * deltaTime);
        }
        private bool ReachedWaypoint()
        {
            float dist = Vector3.Distance(this.Vehicle.GetPosition(), _path.NextPosition());
            return dist <= _threshhold;
        }
     
        private async Task<MovementPath> GetPathToDestination(Vector3 destination)
        {
            List<Vector3> points;
            ActorSelection pathingActor = this.Context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/PathingActor");

            var request = new GetPathRequest(this.Vehicle.GetPosition(), destination);
            points = await pathingActor.Ask<List<Vector3>>(request);

            if (points == null)
                return null;

            var movementPath = new MovementPath(points);
            return movementPath;
        }

        private bool HasPath()
        {
            return _path != null;
        }
    }
}
