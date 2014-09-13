using RTS.Core.Enums;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Stats
{
    public class ModifyStatCommand : MmoCommand<IStats>
    {
        public StatId StatId { get; set; }
        public int Amount { get; set; }
        public int Max { get; set; }


        public override void Execute(IStats target)
        {
            int currentValue = target.GetStatValue(this.StatId);
            int newValue = currentValue + this.Amount;
            target.SetStat(this.StatId, newValue, this.Max);
        }

        public override bool CanExecute(IStats target)
        {
            return true;
        }

        public override CommandId CommandId
        {
            get { return Core.Enums.CommandId.ModifyStat; }
        }

        public override Destination CommandDestination
        {
            get { return Destination.Server; }
        }
    }
}
