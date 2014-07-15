using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Client
{
    [Serializable]
    public class SetPathOnClientCommand : MmoCommand<IEntityController>, IEntityControllerCommand
    {
        public long UnitId { get; set; }
        public List<Vector3> Path { get; set; }
        public override void Execute(IEntityController target)
        {
            target.SetUnitPath(this.UnitId, this.Path);
        }

        public override bool CanExecute(IEntityController target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.SetPathOnClient; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Client; }
        }
    }
}
