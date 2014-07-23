using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Player
{
    public interface IPlayer
    {
        void HandleCommand(object command);
        void MessageComponents(object message);
        void SetTeam(object team);

        void SetTeamId(long p);
    }
}
