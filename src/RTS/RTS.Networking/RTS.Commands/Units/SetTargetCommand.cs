using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.EntityComponents;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Units
{
    [Serializable]
    public class SetTargetCommand : MmoCommand, IEntityTargeterCommand, IVehicleCommand
    {
        public List<long> EntityIds { get; set; }
        public long TargetEntityId { get; set; }

        public void Execute(IEntityTargeter target)
        {
            target.SetTarget(TargetEntityId);
        }

        public bool CanExecute(IEntityTargeter target)
        {
            return true;
        }

        public Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.AttackUnit; }
        }

        public Core.Enums.Destination CommandDestination
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
    }
}
