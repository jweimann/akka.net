using Akka.Actor;
using RTS.Commands.Interfaces;
using RTS.Commands.Stats;
using RTS.Commands.Units;
using RTS.Commands.Weapons;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Units
{
    public class Weapon : IWeapon
    {
        //private ActorRef _targetActorRef; // Using _targetEntity not sure why this was still here.
        private float _attackRange = 65f;
        private float _reloadTimer = 0f;
        private float _fireRate = 1f;
        private IEntity _entity;
        private SpawnEntityData _spawnEntityData;
   
        public bool ReadyToFire()
        {
            return _reloadTimer <= 0f;
        }

        public bool InRange(Core.Structs.Vector3 targetPosition)
        {
            float dist = Vector3.Distance(targetPosition, this._entity.Position);
            return dist <= _attackRange;
        }

        public void MessageComponents(object message)
        {
            throw new NotImplementedException();
        }

        public void SetEntity(IEntity entity)
        {
            _entity = entity;
        }

        public void HandleMessage(object message)
        {
            if (message is IMmoCommand<IWeapon>)
            {
                if ((message as IMmoCommand<IWeapon>).CanExecute(this))
                {
                    (message as IMmoCommand<IWeapon>).Execute(this);
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

        public void Tick(double deltaTime)
        {
            _reloadTimer -= (float)deltaTime;
            TryFire(deltaTime);
        }

        private async void TryFire(double deltaTime)
        {
            if (_targetEntity != null)
            {
                if (this.ReadyToFire())
                {
                    bool isAlive = await GetTargetIsAlive();
                    if (isAlive == false)
                    {
                        _targetEntity = null;
                        return;
                    }

                    var targetPosition = await GetTargetEntityPosition();
                    
                    if (this.InRange(targetPosition))
                    { 
                        _reloadTimer = _fireRate;
                        _entity.MessageTeam(new FireWeaponCommand() { TargetEntityId = _targetEntityId, EntityId = this._entity.Id });
                        _targetEntity.Tell(new ModifyStatCommand() { Amount = this._damage, StatId = StatId.HP });
                        Console.WriteLine("Fired!!!!!!");
                    }
                }
            }
        }

        private async Task<bool> GetTargetIsAlive()
        {
            bool isAlive = await _targetEntity.Ask<Boolean>(EntityRequest.GetIsAlive);
            return isAlive;
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
        private int _damage = -10;
        public async void SetTarget(long entityId)
        {
            if (_entity.Id == entityId)
                return;

            Console.WriteLine("SetTarget EntityId: " + entityId);
            var actorRef = await GetEntityById(entityId);
            long targetTeam = await actorRef.Ask<long>(EntityRequest.GetTeam);
            if (targetTeam == _entity.GetSpawnEntityData().TeamId)
            {
                //return; // Don't target allies with a weapon. FriendlyFire Friendly Fire
            }
            _targetEntityId = entityId;
            _targetEntity = actorRef;
            _targetEntityTeam = targetTeam;
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


        public void FireWeapon(long WeaponEntityId, long TargetEntityId) // Client Command?
        {
            throw new NotImplementedException();
        }
    }
}
