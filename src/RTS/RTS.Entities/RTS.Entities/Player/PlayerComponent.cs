using RTS.Entities.Interfaces.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Player
{
    public abstract class PlayerComponent : IPlayerComponent
    {
        public abstract void MessageComponents(object message);

        public abstract void SetPlayer(IPlayer entity);

        public abstract void HandleMessage(object message);

        public abstract void Update();

        public abstract void PreStart();
    }
}
