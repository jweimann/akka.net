using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.EntityComponents
{
    public interface IWeapon : IEntityTargeter
    {
        bool ReadyToFire();
        bool InRange(Vector3 targetPosition);
    }
}
