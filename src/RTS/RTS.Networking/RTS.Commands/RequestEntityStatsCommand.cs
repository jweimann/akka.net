using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    public class RequestEntityStatsCommand : MmoCommand<IStats>
    {
        public Int64 EntityId { get; set; }
        public override void Execute(IStats target)
        {
            throw new NotImplementedException();
        }

        public override bool CanExecute(IStats target)
        {
            throw new NotImplementedException();
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.RequestEntityStats; }
        }



        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.ServerAndClient; }
        }
    }
}
