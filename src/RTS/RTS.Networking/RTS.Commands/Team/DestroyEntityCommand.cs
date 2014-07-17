using RTS.Entities.Interfaces.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Team
{
    [Serializable]
    public class DestroyEntityCommand : MmoCommand<ITeam>
    {
        public long EntityId { get; set; }
        public override void Execute(ITeam target)
        {
            target.DestroyEntity(this.EntityId);
        }

        public override bool CanExecute(ITeam target)
        {
            return true; ;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.DestroyEntity; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.ServerAndClient; }
        }
    }
}
