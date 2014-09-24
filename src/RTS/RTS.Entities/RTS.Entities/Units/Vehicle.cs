using Akka.Actor;
using BehaviorTreeLibrary;
using JW.Behavior;
using RTS.Commands;
using RTS.Commands.Client;
using RTS.Commands.Interfaces;
using RTS.Commands.Units;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.DataStructures;
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
        private float _speed = 15f;
        private Vector3 _destination;

        List<Behavior> Behaviors = new List<Behavior>(); // OLD behavior system
        

        public Vehicle()
        {
            _path = new List<Vector3>();
        }

        private void NewMoveToPosition(Vector3 position, float threshhold)
        {
            // Build a behavior manager to handle this shit.
            var b = new BHMoveToLocation(_entity.Brain as Brain, position, threshhold);
            _entity.Brain.AddBehavior(b);

            //b.Completed = () => { _behaviors.Remove(b); };

            //_behaviors.RemoveAll(p => p is BHMoveToLocation);
            //_behaviors.Add(b);
        }

        public void MoveToPosition(Core.Structs.Vector3 position, float threshhold)
        {
            if (Vector3.Distance(position, GetPosition()) <= threshhold)
            {
                return; // Already in range
            }
            NewMoveToPosition(position, threshhold);
            return;

            if (position == _destination)
                return;

            if (Vector3.Distance(position, _destination) < threshhold)
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
                    if (dist < threshhold)
                        return;
                }
            }

            Behaviors.RemoveAll(t=> t is MoveToLocationBehavior);
            var behavior = new MoveToLocationBehavior(this, position, _entity.GetActorContext(), threshhold);
            //behavior.Initialize();
            Behaviors.Add(behavior);
            _destination = position;
        }

        public void Stop()
        {
            Behaviors.Clear();
            SendPathToClients(new MovementPath(new List<Vector3>() { GetPosition() }));
        }

        public void SendPathToClients(MovementPath path)
        {
            _entity.MessageTeam(new SetPathOnClientCommand() { Path = path.Points, UnitId = this._entity.Id });
        }
        public void SendPathToClients(List<Core.Structs.Vector3> path)
        {
            throw new NotImplementedException("DEPRICIATED");
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
            // OLD BEHAVIOR STUFF
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

           // Vector3 directionBetweenPoints = targetPosition - _entity.Position;
           // directionBetweenPoints = directionBetweenPoints.Normalized();
           //
           // var weapon = _entity.Components.Where(t => t is Weapon) as Weapon;
           // 
           // var destinationPosition = targetPosition + (directionBetweenPoints * weapon.AttackRange);

        //    Console.WriteLine(String.Format("MOVETOTARGETEDPOSITION - Pos: {0} TargetPos: {1}",
        //        _entity.Position.ToRoundedString(),
        //        targetPosition.ToRoundedString()));

            float weaponRange = 30f;

            if (targetPosition.IsNan() == false)
            {
                MoveToPosition(targetPosition, weaponRange);
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
            if (actorRef != null)
            {
                _targetEntityId = entityId;
                _targetEntity = actorRef;
                _targetEntityTeam = await actorRef.Ask<long>(EntityRequest.GetTeam);
            }
            else
            {
                Console.WriteLine("Vehicle unable to find target.  EntityId: " + entityId);
            }
        }


        public void ClearTarget()
        {
            _targetEntityId = 0;
        }
        private async Task<ActorRef> GetEntityById(long entityId)
        {
            IActorContext context = _entity.GetActorContext() as IActorContext;
            ActorSelection targetActorSelection = context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Entity" + entityId);
            try
            {
                var actorRef = await targetActorSelection.ResolveOne(TimeSpan.FromSeconds(1));
                return actorRef;
            } catch (ActorNotFoundException)
            {
                return null;
            }
        }

        #endregion


        public void SetPath(List<Vector3> list)
        {
            _path = list;
        }

        #region Behavior Support

        public Vector3 GetPosition()
        {
            return _entity.Position;
        }

        public float GetSpeed()
        {
            return _speed;
        }

        public void SetPosition(Vector3 currentPosition)
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

        public void SendCommandToTeam(object command)
        {
            _entity.MessageTeam(command);
        }

        internal float GetBuildRangeThreshhold()
        {
            return _buildRangeThreshhold;
        }


        public void MoveBy(Vector3 vector3)
        {
            if (vector3.IsNan() == false)
                SetPosition(this.GetPosition() + vector3);
        }


        public void SendStopPathToClients()
        {
            SendPathToClients(new MovementPath(new List<Vector3>() { this.GetPosition() }));
        }
    }
}
