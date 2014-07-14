using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Movement
{
    public interface IMover
    {
        Vector3 Position { get; }
        Vector3 Velocity { get; }
        void SetPosition(Core.Structs.Vector3 vector3);
        void SetVelocity(Core.Structs.Vector3 vector3);
        void SetDestination(Vector3 vector3);
    }
}
