using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Pathing
{
    [Serializable]
    public class ResolvePathCommand : MmoCommand, IMmoCommand
    {
        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Server; }
        }

        public int RequestId { get; set; }
        public List<Vector3> Path { get; set; }
    }
}
