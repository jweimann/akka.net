using Akka.Actor;
using RTS.Core.Structs;
using RTS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace RTS.Entities
{
    public abstract class EntityComponent<T> : IEntityComponent
    {
        protected IEntity _entity;
        protected ActorSelection AreaOfInterest { get { return _entity == null ? null : ((Entity)_entity).AreaOfInterest; } }
     
        protected void MessageComponents(object message)
        {
            if (_entity != null)
            {
                _entity.MessageComponents(message);
            }
        }

        public void SetEntity(IEntity entity)
        {
            _entity = entity;
        }

        public abstract void HandleMessage(object message);
        public abstract void Tick(double deltaTime);
        public virtual void PreStart() { }

        void IEntityComponent.MessageComponents(object message)
        {
            throw new NotImplementedException();
        }
        
    }
}
