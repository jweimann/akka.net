using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Units
{
    [Serializable]
    public class SetPathCommand : MmoCommand<IVehicle>, IVehicleCommand
    {
        public List<long> EntityIds { get; set; }
        public long EntityId { get; set; }
        public List<Vector3> Path { get; set; }

        public override void Execute(IVehicle target)
        {
            target.SetPath(this.Path);
        }

        public override bool CanExecute(IVehicle target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SetPathOnServer; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
        }
    }
}
