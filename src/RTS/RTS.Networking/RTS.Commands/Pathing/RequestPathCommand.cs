using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Pathing
{
    [Serializable]
    public class RequestPathCommand : MmoCommand, IMmoCommand
    {
        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Client; }
        }

        public int RequestId { get; set; }
        public Vector3 StartLocation { get; set; }
        public Vector3 Destination { get; set; }
    }
}
