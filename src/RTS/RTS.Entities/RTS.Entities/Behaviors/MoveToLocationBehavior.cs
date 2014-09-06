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
        private Vector3 _destination; // Only used for setting the path.  Do not use for anything other than initialization.
        private Vector3 _startPosition;
        private float _totalDistance;
        private IActorContext _context;
        private int _currentPathNode;
        private List<Vector3> _path;

        public MoveToLocationBehavior()
        {

        }

        public Vector3 RequestedDestination { get; private set; }
        public Vector3? Destination
        {
            get
            {
                if (_path == null || _path.Count == 0)
                    return null;

                return _path[_path.Count - 1];
            }
        }
        
        private bool HasPath() { return this.Destination != null; }
        public MoveToLocationBehavior(Vehicle vehicle, Vector3 destination, object context)
        {
            this.RequestedDestination = destination;
            _vehicle = vehicle;
            SetDestinationAndTemporaryPath(destination);
            _context = context as IActorContext;
            this.SetInitialize(Initialize);
        }

        private void SetDestinationAndTemporaryPath(Vector3 destination)
        {
            _destination = destination;
            _path = new List<Vector3>() { destination };
        }

        public async void Initialize()
        {
            _path = await GetPathToDestination(_destination);
            if (HasPath() == false)
            {
                Console.WriteLine(String.Format("Unable to find a path from: {0} to: {1}",_vehicle.GetPosition(), _destination.ToRoundedString()));
                return;
            }
            _currentPathNode = 0;

            _vehicle.SendPathToClients(_path);

            _startPosition = _vehicle.GetPosition();
            _totalDistance = Vector3.Distance(_startPosition, (Vector3)this.Destination);

            //Add<Condition>().CanRun = NotReachedDestination;
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
                return BehaviorTreeLibrary.Status.BhSuccess;

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
            //Console.WriteLine(String.Format("DIRECTIONDEBUG - Direction: {0} MoveAmount: {1} PreviousPos: {2} NewPos: {3}",
            //    direction.ToRoundedString(),
            //    moveAmount.ToRoundedString(),
            //    _vehicle.GetPosition().ToRoundedString(),
            //    currentPosition.ToRoundedString()));

            _vehicle.SetPosition(currentPosition);

            if (ReachedWaypoint())
            {
                NextWaypoint();
            }

            return BehaviorTreeLibrary.Status.BhSuccess;
        }

        private Vector3 NextPosition()
        {
            if (_path.Count <= _currentPathNode)
            {
                return _path[_path.Count - 1];
            }
            return _path[_currentPathNode];
        }

        private bool NotReachedDestination()
        {
            return !ReachedDestination();
        }
        private bool ReachedDestination()
        {
            if (HasPath() == false)
                return true;

            DebugDistanceToConsole();

            float dist = Vector3.Distance(_vehicle.GetPosition(), (Vector3)this.Destination);
            return dist <= _vehicle.GetMoveThreshhold();
        }
        private void DebugDistanceToConsole()
        {
            if (this.Destination.HasValue == false)
            {
                Console.WriteLine("MOVEDEBUG - No Destination");
                return;
            }
            var position = _vehicle.GetPosition();
            float dist = Vector3.Distance(position, (Vector3)this.Destination);
            float moveThreshhold = _vehicle.GetMoveThreshhold();
            //Console.WriteLine(String.Format("MOVEDEBUG - Dist: {0} MoveThreshhold: {1} Position: {2} Destination: {3}",
            //    dist.ToString("N0"),
            //    moveThreshhold.ToString("N0"),
            //    position.ToRoundedString(),
            //    Destination.Value.ToRoundedString()));

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
            if (this.Destination.HasValue)
            {
                float dist2 = Vector3.Distance(_vehicle.GetPosition(), (Vector3)this.Destination);
                Console.WriteLine("Reached Destination.  Distance " + dist2.ToString());
            }
            _path = null;
            return BehaviorTreeLibrary.Status.BhSuccess;
        }
    }
}
