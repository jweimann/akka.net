using RTS.Core.Structs;
using RTS.Entities.Interfaces.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.UnitTypes
{
    public interface IVehicle : IEntityTargeter
    {
        void MoveToPosition(Vector3 position);

        void SetPath(List<Vector3> list);
    }
}
