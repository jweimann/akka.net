using RTS.Commands.Interfaces;
using RTS.Entities;
using RTS.Entities.Interfaces.EntityComponents;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Units
{
    [Serializable]
    public class SetTargetCommand : MmoCommand<IEntityTargeter>, IEntityTargeterCommand, IEntityComponentCommand//, IVehicleCommand
    {
        //public List<long> EntityIds { get; set; }
        public long TargetEntityId { get; set; }

        public override void Execute(IEntityTargeter target)
        {
            target.SetTarget(TargetEntityId);
        }

        public override bool CanExecute(IEntityTargeter target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.AttackUnit; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
        }


        public void Execute(Entities.Interfaces.Control.IEntityController target)
        {
            throw new NotImplementedException();
        }

        public bool CanExecute(Entities.Interfaces.Control.IEntityController target)
        {
            return true;
        }


        public void Execute(IVehicle target)
        {
            throw new NotImplementedException();
        }

        public bool CanExecute(IVehicle target)
        {
            return false;
        }



        public long EntityId { get; set; }
    }
}
