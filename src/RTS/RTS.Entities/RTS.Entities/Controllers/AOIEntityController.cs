using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Server;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Interfaces;
using RTS.Entities.Requests;
using RTS.Entities.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Controllers
{
    public class AOIEntityController : IEntityController
    {
        private const double UPDATE_SURROUNDING_AREAS_TIMER = 1.200f;
        private const int MAX_MOVE_REQUESTS_QUEUED = 1000;
        private const int MAX_MOVE_REQUESTS_PER_MESSAGE = 50;

        private Dictionary<Int64, EntityInfo> _entities = new Dictionary<long, EntityInfo>();
        private Bounds _bounds;
        public void HandleRequest(object request)
        {
            throw new NotImplementedException();
        }
        public void Update()
        {
            lock (this)
            {
                for (int i = 0; i < _moveRequestIndex; i += MAX_MOVE_REQUESTS_PER_MESSAGE)
                {
                    int count = _moveRequestIndex - i;
                    if (count > MAX_MOVE_REQUESTS_PER_MESSAGE) count = MAX_MOVE_REQUESTS_PER_MESSAGE;
                    BatchMoveRequest message = CreateMoveRequest(i, count);

                    foreach (var entity in _entities)
                    {
                        if (_bounds.Contains(entity.Value.Position))
                        {
                            var actorWithLocation = entity.Value.ActorRef;
                            actorWithLocation.Tell(message);
                        }
                    }
                }

                _moveRequestIndex = 0;
            }
        }
        public void AddEntity(long entityId, string name, Core.Structs.Vector3 position, object sender)
        {
            Console.WriteLine(String.Format("AddEntity  AOI: {0}  EntityId: {1}", _bounds.ToString(), entityId));
            if (_entities.ContainsKey(entityId))
            {
                //throw new Exception("Entity already in collection");
            }
            else
            {
                _entities.Add(entityId, new EntityInfo() { ActorRef = sender as ActorRef, Id = entityId, Position = position });
            }
            MessageEntities(new AOIDetailsCommand() { AoiId = GetId(), Center = this._bounds.Center, Extents = this._bounds.Extents });
            //(sender as ActorRef).Tell(new AOIDetailsCommand() { AoiId = GetId(), Center = this._bounds.Center, Extents = this._bounds.Extents, Sender = this });
        }
        public void RemoveEntity(long entityId)
        {
            Console.WriteLine(String.Format("RemoveEntity  AOI: {0}  EntityId: {1}", _bounds.ToString(), entityId));
            if (_entities.ContainsKey(entityId) == false)
            {
                //throw new Exception("Entity not in collection");
            }
            else
            {
                MessageEntities(new LeaveAreaOfInterestCommand() { EntityId = entityId, Position = _entities[entityId].Position });
                _entities.Remove(entityId);
            }
        }

        #region Not Implemented On Server
        public Dictionary<long, Interfaces.Movement.IMover> GetEntities()
        {
            throw new NotImplementedException("This is only supported on the client");
        }
        #endregion


        public void SetBounds(Bounds bounds)
        {
            _bounds = bounds;
        }

        private void MessageEntities(object message)
        {
            foreach (var entity in _entities)
            {
                if (message is IEntityRequest)
                {
                    if (entity.Key == ((IEntityRequest)message).EntityId)
                    {
                        continue;
                    }
                }
                var actorWithLocation = entity.Value.ActorRef;
                actorWithLocation.Tell(message);
            }
        }

        SendMovementToAOICommand[] _moveRequestsThisFrame = new SendMovementToAOICommand[MAX_MOVE_REQUESTS_QUEUED];
        private int _moveRequestIndex;
        private BatchMoveRequest CreateMoveRequest(int startingIndex, int count)
        {
            BatchMoveRequest message = new BatchMoveRequest() { EntityId = new Int64[count], Velocity = new Vector3[count], Position = new Vector3[count] };
            for (int i = 0; i < count; i++)
            {
                message.Position[i] = _moveRequestsThisFrame[startingIndex + i].Position;
                message.Velocity[i] = _moveRequestsThisFrame[startingIndex + i].Velocity;
                message.EntityId[i] = _moveRequestsThisFrame[startingIndex + i].EntityId;
            }
            return message;
        }


        public void MoveEntity(object sendMovementToAOICommand)
        {
            SendMovementToAOICommand command = sendMovementToAOICommand as SendMovementToAOICommand;
            _moveRequestsThisFrame[_moveRequestIndex] = command;
            lock (this)
            {
                _moveRequestIndex++;
            }
            _entities[command.EntityId].Position = command.Position;
        }


        public void SendEntitiesInAreaListToEntity(object target)
        {
            ActorRef actor = target as ActorRef;

            var command = new EntitiesInAreaListCommand(_entities.Keys.Count);

            for (int i = 0; i < _entities.Keys.Count; i++)
            {
                long key = _entities.Keys.ElementAt(i);
                command.EntityIds[i] = key;
                command.Locations[i] = _entities[key].Position;
                command.Names[i] = "Entity " + key;
            }

            actor.Tell(command);
        }


        public int GetId()
        {
            return (int)_bounds.Center.x * 1024 + (int)_bounds.Center.y * 2048 + (int)_bounds.Center.z * 4096;
        }


        public void SpawnEntity(string name, Vector3 position, long entityId)
        {
            throw new NotImplementedException();
        }


        public void SetUnitPath(long unitId, List<Vector3> path)
        {
            throw new NotImplementedException();
        }

    }
}
