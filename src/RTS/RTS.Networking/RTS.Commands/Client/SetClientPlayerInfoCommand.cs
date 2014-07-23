using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Server
{
    [Serializable]
    public class SetClientPlayerInfoCommand : MmoCommand<IPlayer>, IPlayerCommand
    {
        public long TeamId { get; set; }
        public string PlayerName { get; set;}

        public override void Execute(IPlayer target)
        {
            target.SetTeamId(this.TeamId);
        }

        public override bool CanExecute(IPlayer target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SetClientPlayerInfo; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Client; }
        }

    }
}
