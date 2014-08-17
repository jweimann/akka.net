using BehaviorTreeLibrary;
using RTS.Core.Structs;
using RTS.Entities.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Behaviors
{
    public class MoveToLocationBehavior : Sequence
    {
        private Vehicle _vehicle;
        private Vector3 _destination;
        private Vector3 _startPosition;
        private float _totalDistance;
        public MoveToLocationBehavior(Vehicle vehicle, Vector3 destination)
        {
            _vehicle = vehicle;
            _destination = destination;
            _startPosition = _vehicle.GetPosition();
            _totalDistance = Vector3.Distance(_startPosition, _destination);

            Add<Condition>().CanRun = NotReachedDestination;
            Add<Behavior>().Update = Move;
            Add<Condition>().CanRun = ReachedDestination;
            Add<Behavior>().Update = Stop;
            //var selector = Add<Selector>();
            //var sequence = selector.Add<Sequence>();
            //sequence.Add<Condition>().CanRun = NearFood;
            //sequence.Add<Behavior>().Update = () =>
            //                                      {
            //                                          Debug.Log("Eating");
            //                                          hunger = 0;
            //                                          _targetFood = null;
            //                                          GameObject.Destroy(_dwarf.Target);
            //                                          _dwarf.Target = _dwarf.gameObject;
            //                                          _dwarf.ResetTarget();
            //                                          return Status.BhSuccess;
            //                                      };
        }

        Status Move()
        {
            double deltaTime = this.DeltaTime;
            Vector3 currentPosition = _vehicle.GetPosition();
            
            float speed = _vehicle.GetSpeed();
            Vector3 direction = (_destination - currentPosition).Normalized();

            float dist = Vector3.Distance(_vehicle.GetPosition(), _destination);

            Vector3 moveAmount = direction * speed * deltaTime;
            Vector3 speedPerSecond = moveAmount * (1 / deltaTime);

            float distMoved = Vector3.Distance(currentPosition, _startPosition);
            string pctText = String.Format(" Moved {0} of {1}", distMoved, _totalDistance);
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + " / " + deltaTime + pctText);// + " Moving from " + currentPosition.ToString() + " to " + _destination.ToString() + " MoveAmount=" + moveAmount.ToString() + " Dist=" + dist + " SPS=" + speedPerSecond);
            currentPosition += moveAmount;

            _vehicle.SetPosition(currentPosition);

            

            return BehaviorTreeLibrary.Status.BhSuccess;
        }

        private bool NotReachedDestination()
        {
            return !ReachedDestination();
        }
        private bool ReachedDestination()
        {
            float dist = Vector3.Distance(_vehicle.GetPosition(), _destination);
            return dist <= _vehicle.GetMoveThreshhold();
        }

        Status Stop()
        {
            float dist2 = Vector3.Distance(_vehicle.GetPosition(), _destination);
            Console.WriteLine("Reached Destination.  Distance " + dist2.ToString());
            return BehaviorTreeLibrary.Status.BhSuccess;
        }
    }
}
