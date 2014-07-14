using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class UpdateStatsCommand : MmoCommand<IStats>
    {
        public override CommandId CommandId
        {
            get { return CommandId.UpdateStats; }
        }
        public Int64 EntityId { get; set; }
        public StatId[] StatIds { get; set; }
        public int[] Values { get; set; } // Need to figure out how I want to deal with different data types

        public override void Execute(IStats target)
        {
            for (int i = 0; i < StatIds.Length; i++)
            {
                target.SetStat(StatIds[i], Values[i]);
            }
        }
        public override bool CanExecute(IStats target)
        {
            return (target != null);
        }
        public override string ToString()
        {
            return String.Format("UpdateStatsCommand  EntityId: {0}  StatCount: {1}", EntityId, StatIds.Length);
        }

        public override Destination CommandDestination
        {
            get { return Destination.ServerAndClient; }
        }
    }
}
