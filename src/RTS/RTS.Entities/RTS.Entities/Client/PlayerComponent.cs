using RTS.Entities.Interfaces.Player;
using RTS.Entities.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Client
{
    public abstract class PlayerComponent<T> : PlayerComponent
    {
        protected IPlayer _player;
        public override void MessageComponents(object message)
        {
            if (_player != null)
            {
                _player.MessageComponents(message);
            }
        }

        public override void SetPlayer(IPlayer player)
        {
            _player = player;
        }

        //public abstract void HandleMessage(object message);
        //public abstract void Update();
        //public virtual void PreStart() { }
    }
}
