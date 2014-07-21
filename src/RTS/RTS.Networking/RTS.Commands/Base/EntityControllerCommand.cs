using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Base
{
    [Serializable]
    public abstract class EntityControllerCommand : IEntityControllerCommand
    {
        public object Sender { get; set; }
        public int AoiId { get; set; }

        public abstract void Execute(IEntityController target);

        public abstract bool CanExecute(IEntityController target);

        public abstract Core.Enums.CommandId CommandId { get; }
        public override string ToString()
        {
            return this.GetType().ToString() + " AoiId: " + AoiId;
        }
        public Destination CommandDestination { get; set; }


        public abstract bool TellClient { get; }
        public abstract bool TellServer { get; }
    }
}
