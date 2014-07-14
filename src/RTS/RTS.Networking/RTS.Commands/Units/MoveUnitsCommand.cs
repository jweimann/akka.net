using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Units
{
    [Serializable]
    public class MoveUnitsCommand : MmoCommand, IVehicleCommand
    {
        public List<long> EntityIds { get; set; }
        public Vector3 Position { get; set; }

        public void Execute(IVehicle target)
        {
            target.ClearTarget();
            target.MoveToPosition(this.Position);
        }

        public bool CanExecute(IVehicle target)
        {
            return true;
        }

        public Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.MoveUnits; }
        }

        Core.Enums.Destination IMmoCommand<IVehicle>.CommandDestination
        {
            get { return Core.Enums.Destination.ServerAndClient; }
        }

        public object Sender
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int AoiId
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        Core.Enums.Destination IMmoCommand<IEntityController>.CommandDestination
        {
            get { return Core.Enums.Destination.ServerAndClient; }
        }

        public void Execute(IEntityController target)
        {
            foreach (long entityId in this.EntityIds)
            {
                target.SetUnitPath(entityId, new List<Vector3>() { this.Position });
            }
        }

        public bool CanExecute(IEntityController target)
        {
            return false;
        }
    }
}
