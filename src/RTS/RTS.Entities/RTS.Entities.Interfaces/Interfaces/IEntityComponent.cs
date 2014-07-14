using RTS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities
{
    public interface IEntityComponent
    {
        void MessageComponents(object message);
        void SetEntity(IEntity entity);
        void HandleMessage(object message);
        void Update(double deltaTime);
        void PreStart();
    }
}
