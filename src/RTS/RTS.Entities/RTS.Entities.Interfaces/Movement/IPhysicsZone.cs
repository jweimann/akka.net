using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Movement
{
    public interface IPhysicsZone
    {
        Bounds Bounds { get; }
        List<IMovementActor> MovementActors { get; }
    }
}
