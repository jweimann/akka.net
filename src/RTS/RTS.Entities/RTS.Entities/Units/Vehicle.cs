using Akka.Actor;
using BehaviorTreeLibrary;
using RTS.Commands;
using RTS.Commands.Client;
using RTS.Commands.Interfaces;
using RTS.Commands.Units;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Behaviors;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.EntityComponents;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Units
{
    public class Vehicle : IVehicle
    {
        private IEntity _entity;
        private List<Vector3> _path;
        private float _moveThreshhold = 0.25f;
        private float _buildRangeThreshhold = 1.50f; // Distance from point to build at it
        private float _attackRange = 25f;
        private float _speed = 5f;
        private Vector3 _destination;

        List<Behavior> Behaviors = new List<Behavior>();

        public Vehicle()
        {
            _path = new List<Vector3>();
        }
        public void MoveToPosition(Core.Structs.Vector3 position)
        {
            if (position == _destination)
                return;

            if (Vector3.Distance(position, _destination) < _moveThreshhold)
                return;

            if (Behaviors.Any(t=> t is MoveToLocationBehavior && 
                (
                    ((MoveToLocationBehavior)t).Destination.HasValue == true &&
                    ((MoveToLocationBehavior)t).Destination.Value.WithoutHeight() == position.WithoutHeight())) // Ignore height
                )
            {
                return; // Destination already set.
            }

            foreach(var existingBehavior in Behaviors)
            {
                if (existingBehavior is MoveToLocationBehavior)
                {
                    var behaviorDest = ((MoveToLocationBehavior)existingBehavior).RequestedDestination;
                    var dist = Vector3.Distance(behaviorDest.WithoutHeight(), position.WithoutHeight());
                    if (dist < _moveThreshhold)
                        return;
                }
            }

            Behaviors.RemoveAll(t=> t is MoveToLocationBehavior);
            var behavior = new MoveToLocationBehavior(this, position, _entity.GetActorContext());
            behavior.Initialize();
            Behaviors.Add(behavior);
            _destination = position;
        }

        public void Stop()
        {
            Behaviors.Clear();
            SendPathToClients(new List<Vector3>() { GetPosition() });
        }

        public void SendPathToClients(List<Core.Structs.Vector3> path)
        {
            _entity.MessageTeam(new SetPathOnClientCommand() { Path = path, UnitId = this._entity.Id });
        }

        private bool PositionIsChanged(ref Core.Structs.Vector3 position)
        {
            return _destination != position;
        }

        public void MessageComponents(object message)
        {
            _entity.MessageComponents(message);
        }

        public void SetEntity(Interfaces.IEntity entity)
        {
            _entity = entity;
        }

        public void HandleMessage(object message)
        {
            if (message is IMmoCommand<IVehicle>)
            {
                if ((message as IMmoCommand<IVehicle>).CanExecute(this))
                {
                    (message as IMmoCommand<IVehicle>).Execute(this);
                }
            }
            if (message is IMmoCommand<IEntityTargeter>)
            {
                if ((message as IMmoCommand<IEntityTargeter>).CanExecute(this))
                {
                    (message as IMmoCommand<IEntityTargeter>).Execute(this);
                }
            }
        }

        private double _targettingTimer = 0f;
        public async void Tick(double deltaTime)
        {
            //Console.WriteLine("VehicleTick " + DateTime.Now.TimeOfDay.ToString() + " DeltaTime " + deltaTime);
            for (int i = 0; i < this.Behaviors.Count; i++)
            {
                try
                {
                    this.Behaviors[i].Tick(deltaTime); // Behaviors need to remove selves or all be on an AI component, mixing is causing conflicts where they fight back and forth
                } catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.ToString());
                }
            }

            _targettingTimer += deltaTime;
            if (_targettingTimer >= 1.0)
            {
                _targettingTimer = 0f;
                if (TargetIsSet())
                {
                    if (await TargetIsAlive() == false)
                    {
                        _targetEntityId = 0;
                    }
                    else
                    {
                        await MoveToTargetedPosition();
                    }
                }
            }
        }

        private async Task<bool> TargetIsAlive()
        {
            return await _targetEntity.Ask<bool>(EntityRequest.GetIsAlive);
        }

        private bool PathIsSet()
        {
            return _path.Count > 0;
        }

        private bool TargetIsSet()
        {
            return _targetEntityId != 0;
        }

        private async Task MoveToTargetedPosition()
        {
            Vector3 targetPosition = await GetTargetEntityPosition();
            //var destinationPosition = Vector3.LerpByDistance(targetPosition, this._entity.Position, GetMoveThreshold());
            //Console.WriteLine(String.Format("MOVETOTARGETEDPOSITION - Pos: {0} TargetPos: {1}",
            //    _entity.Position.ToRoundedString(),
            //    targetPosition.ToRoundedString()));

            if (targetPosition.IsNan() == false)
            {
                MoveToPosition(targetPosition);
            }
        }

        private float GetMoveThreshold()
        {
            if (_targetEntityTeam != this._entity.GetSpawnEntityData().TeamId)
            {
                return _attackRange;
            }
            else
            {
                return _moveThreshhold;
            }
        }

        private async Task<Vector3> GetTargetEntityPosition()
        {
            Vector3 position = await _targetEntity.Ask<Vector3>(new GetPositionCommand());
            //Console.WriteLine("Retrieved Position for Entity " + _targetEntityId + " Position " + position.ToString() + " MyPosition " + _entity.Position.ToString());
            return position;
        }

        public void PreStart()
        {
            //throw new NotImplementedException();
        }

        #region IEntityTargeter

        private long _targetEntityId;
        private ActorRef _targetEntity;
        private long _targetEntityTeam;

        public async void SetTarget(long entityId)
        {
            if (entityId == this._entity.Id)
            {
                return; // Dont' target self...
            }
            var actorRef = await GetEntityById(entityId);
            _targetEntityId = entityId;
            _targetEntity = actorRef;
            _targetEntityTeam = await actorRef.Ask<long>(EntityRequest.GetTeam);
        }


        public void ClearTarget()
        {
            _targetEntityId = 0;
        }
        private async Task<ActorRef> GetEntityById(long entityId)
        {
            IActorContext context = _entity.GetActorContext() as IActorContext;
            ActorSelection targetActorSelection = context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Entity" + entityId);
            var actorRef = await targetActorSelection.ResolveOne(TimeSpan.FromSeconds(1));
            return actorRef;
        }

        #endregion


        public void SetPath(List<Vector3> list)
        {
            _path = list;
        }

        #region Behavior Support

        internal Vector3 GetPosition()
        {
            return _entity.Position;
        }

        internal float GetSpeed()
        {
            return _speed;
        }

        internal void SetPosition(Vector3 currentPosition)
        {
            if (currentPosition.IsNan())
            {
                throw new ArgumentOutOfRangeException("Position is NaN");
            }
            if (currentPosition.y > 3f)
            {
                //Console.WriteLine("RaisedOffGround " + currentPosition.y);
            }
            _entity.Position = currentPosition;
            //SendPathToClients(_destination);
        }

        internal float GetMoveThreshhold()
        {
            return _moveThreshhold;
        }

        #endregion

        internal void SendCommandToTeam(Commands.Buildings.FinishBuildEntityCommand finishBuildEntityCommand)
        {
            _entity.MessageTeam(finishBuildEntityCommand);
        }

        internal float GetBuildRangeThreshhold()
        {
            return _buildRangeThreshhold;
        }
    }
}
