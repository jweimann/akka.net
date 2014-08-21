﻿using Akka.Actor;
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
            Behaviors.Clear();
            Behaviors.Add(new MoveToLocationBehavior(this, position));
            _destination = position;
            _path = new List<Vector3>() { position };
            SendPathToClients(position);
            return;

            if (PositionIsChanged(ref position) == false)
            {
                return;
            }
            _destination = position;
            _path = new List<Vector3>() { position };
            _entity.MessageTeam(new SetPathOnClientCommand() { Path = _path, UnitId = this._entity.Id });
        }

        public void Stop()
        {
            Behaviors.Clear();
            SendPathToClients(GetPosition());
        }

        private void SendPathToClients(Core.Structs.Vector3 position)
        {
            _entity.MessageTeam(new SetPathOnClientCommand() { Path = _path, UnitId = this._entity.Id });
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

        public async void Tick(double deltaTime)
        {
            //Console.WriteLine("VehicleTick " + DateTime.Now.TimeOfDay.ToString() + " DeltaTime " + deltaTime);
            for (int i = 0; i < this.Behaviors.Count; i++)
            {
                this.Behaviors[i].Tick(deltaTime); // Behaviors need to remove selves or all be on an AI component, mixing is causing conflicts where they fight back and forth
            }
           

            if (PathIsSet())
            {
                MoveAlongPath(deltaTime);
            }

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

        private async Task<bool> TargetIsAlive()
        {
            return await _targetEntity.Ask<bool>(EntityRequest.GetIsAlive);
        }

        private void MoveAlongPath(double deltaTime)
        {
            float distance = Vector3.Distance(_entity.Position, _path[0]);
            if (distance > GetMoveThreshold())
            {
                Vector3 direction = (_path[0] - _entity.Position).Normalized();
                _entity.Position += direction * _speed * deltaTime;
            }
            else
            {
                _path.RemoveAt(0); // Reached this point, remove it.
            }
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
            var destinationPosition = Vector3.LerpByDistance(targetPosition, this._entity.Position, GetMoveThreshold());
            MoveToPosition(destinationPosition);
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
    }
}