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
        void MoveToPosition(Vector3 position, float threshhold);

        void SetPath(List<Vector3> list);

        void SetPosition(Vector3 position);

        Vector3 GetPosition();

        void SendPathToClients(DataStructures.MovementPath path);
        float GetSpeed();

        void MoveBy(Vector3 vector3);

        void SendCommandToTeam(object command);

        void SendStopPathToClients();
    }
}
