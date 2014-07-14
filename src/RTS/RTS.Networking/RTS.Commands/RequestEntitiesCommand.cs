using RTS.Commands.Base;
using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands
{
    [Serializable]
    public class RequestEntitiesCommand : EntityControllerCommand
    {
        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.RequestEntities; }
        }

        public override void Execute(Entities.Interfaces.Control.IEntityController target)
        {
            target.SendEntitiesInAreaListToEntity(Sender);
        }

        public override bool CanExecute(Entities.Interfaces.Control.IEntityController target)
        {
            return (target != null && Sender != null);
        }
    }
}
