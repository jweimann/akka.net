using Akka.Actor;
using BehaviorTreeLibrary;
using RTS.ActorRequests;
using RTS.Core.Structs;
using RTS.Entities.Units;
using RTS.Pathfinding;
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
        private IActorContext _context;
        private int _currentPathNode;
        private List<Vector3> _path;

        public Vector3 Destination { get { return _destination; } }
        public MoveToLocationBehavior(Vehicle vehicle, Vector3 destination, object context)
        {
            _vehicle = vehicle;
            _destination = destination;
            _context = context as IActorContext;
            this.SetInitialize(Initialize);
        }

        public async void Initialize()
        {
            _path = await GetPathToDestination(_destination);
            _currentPathNode = 0;

            _vehicle.SendPathToClients(_path);

            _startPosition = _vehicle.GetPosition();
            _totalDistance = Vector3.Distance(_startPosition, _destination);

            Add<Condition>().CanRun = NotReachedDestination;
            Add<Behavior>().Update = Move;

            //Add<Condition>().CanRun = ReachedWaypoint;
            //Add<Behavior>().Update = NextWaypoint;

            Add<Condition>().CanRun = ReachedDestination;
            Add<Behavior>().Update = Stop;
        }

        private async Task<List<Vector3>> GetPathToDestination(Vector3 destination)
        {
            List<Vector3> path;
            ActorSelection pathingActor = _context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/PathingActor");
            
            var request = new GetPathRequest(_vehicle.GetPosition(), destination);
            path = await pathingActor.Ask<List<Vector3>>(request);
            
            return path;
        }

        private BehaviorTreeLibrary.Status Move()
        {
            if (_path == null || _path.Count == 0)
                return BehaviorTreeLibrary.Status.BhInvalid;

            double deltaTime = this.DeltaTime;
            Vector3 currentPosition = _vehicle.GetPosition();
            
            float speed = _vehicle.GetSpeed();
            Vector3 direction = (NextPosition() - currentPosition).Normalized();

            float dist = Vector3.Distance(_vehicle.GetPosition(), NextPosition());

            Vector3 moveAmount = direction * speed * deltaTime;
            Vector3 speedPerSecond = moveAmount * (1 / deltaTime);

            float distMoved = Vector3.Distance(currentPosition, _startPosition);
            string pctText = String.Format(" Moved {0} of {1}", distMoved, _totalDistance);
            //Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + " / " + deltaTime + pctText);// + " Moving from " + currentPosition.ToString() + " to " + _destination.ToString() + " MoveAmount=" + moveAmount.ToString() + " Dist=" + dist + " SPS=" + speedPerSecond);
            currentPosition += moveAmount;

            _vehicle.SetPosition(currentPosition);

            if (ReachedWaypoint())
            {
                NextWaypoint();
            }

            return BehaviorTreeLibrary.Status.BhSuccess;
        }

        private Vector3 NextPosition()
        {
            return _path[_currentPathNode];
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
        private bool ReachedWaypoint()
        {
            float dist = Vector3.Distance(_vehicle.GetPosition(), NextPosition());
            return dist <= _vehicle.GetMoveThreshhold();
        }
        private BehaviorTreeLibrary.Status NextWaypoint()
        {
            _currentPathNode++;
            return BehaviorTreeLibrary.Status.BhSuccess;
        }

        private BehaviorTreeLibrary.Status Stop()
        {
            float dist2 = Vector3.Distance(_vehicle.GetPosition(), _destination);
            Console.WriteLine("Reached Destination.  Distance " + dist2.ToString());
            return BehaviorTreeLibrary.Status.BhSuccess;
        }
    }
}
