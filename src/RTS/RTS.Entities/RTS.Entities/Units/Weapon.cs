using Akka.Actor;
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
        private float _attackRange = 8f;
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
            return Vector3.Distance(targetPosition, this._entity.Position) <= _attackRange;
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
            //throw new NotImplementedException();
        }

        public void Update(double deltaTime)
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
                    _spawnEntityData = await _targetEntity.Ask<SpawnEntityData>(EntityRequest.GetSpawnData);

                    if (this.InRange(_spawnEntityData.Position))
                    {
                        _reloadTimer = _fireRate;
                        Console.WriteLine("Fired!!!!!!");
                    }
                }
            }
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
            long targetTeam = await actorRef.Ask<long>(EntityRequest.GetTeam);
            if (targetTeam == _entity.GetSpawnEntityData().TeamId)
            {
                return; // Don't target allies with a weapon.
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
    }
}
