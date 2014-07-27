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
    public class MoveUnitsCommand : MmoCommand<IVehicle>, IVehicleCommand
    {
        //public List<long> EntityIds { get; set; }
        public Vector3 Position { get; set; }

        public override void Execute(IVehicle target)
        {
            target.ClearTarget();
            target.MoveToPosition(this.Position);
        }

        public override bool CanExecute(IVehicle target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.MoveUnits; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
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

        public List<long> EntityIds { get; set; }

        Core.Enums.CommandId IMmoCommand<IVehicle>.CommandId
        {
            get { return Core.Enums.CommandId.MoveUnits; }
        }

        Core.Enums.Destination IMmoCommand<IVehicle>.CommandDestination
        {
            get { return Core.Enums.Destination.ServerAndClient; }
        }

        void IMmoCommand<IVehicle>.Execute(IVehicle target)
        {
            foreach (long entityId in this.EntityIds)
            {
                target.ClearTarget();
                target.MoveToPosition(this.Position);
            }
        }

        bool IMmoCommand<IVehicle>.CanExecute(IVehicle target)
        {
            return true;
        }

        public long EntityId { get; set; }
    }
}
