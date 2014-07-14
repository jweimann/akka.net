using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Player
{
    public interface IPlayerComponent
    {
        void MessageComponents(object message);
        void SetPlayer(IPlayer entity);
        void HandleMessage(object message);
        void Update();
        void PreStart();
    }
}
