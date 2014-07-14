using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Movement
{
    public interface IMovementActor
    {
        Vector3 Location { get; }
        Vector3 Direction { get; }
        IPhysicsZone PhysicsZone { get; }
        void SetPhysicsZone(IPhysicsZone physicsZone);
    }
}
