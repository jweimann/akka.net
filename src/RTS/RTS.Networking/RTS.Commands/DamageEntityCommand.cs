using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class DamageEntityCommand : MmoCommand<IStats>
    {
        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.DamageEntity; }
        }
        public Int64 EntityId { get; set; }
        public Int64 OriginatorEntityId { get; set; }
        public int Damage { get; set; }
        public override void Execute(IStats target)
        {
            target.TakeDamage(this.Damage);
        }
        public override bool CanExecute(IStats target)
        {
            return target != null;
        }

        public override Core.Enums.Destination CommandDestination { get { return Core.Enums.Destination.ServerAndClient; } }
    }
}
