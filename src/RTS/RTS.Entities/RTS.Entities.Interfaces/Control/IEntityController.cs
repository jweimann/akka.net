using RTS.Core.Structs;
using RTS.DataStructures;
using RTS.Entities.Interfaces.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Control
{
    public interface IEntityController : IController
    {
        //void HandleRequest(object request);
        void AddEntity(Int64 entityId, string name, Vector3 position, object sender);
        void RemoveEntity(long EntityId);
        void Update();
        Dictionary<Int64, IMover> GetEntities();

        void SetBounds(Bounds Bounds);

        void MoveEntity(object sendMovementToAOICommand);

        void SendEntitiesInAreaListToEntity(object sender);
        int GetId();

        void SpawnEntity(string name, Vector3 position, long entityId, long teamId, List<Stat> stats);

        void SetUnitPath(long unitId, List<Vector3> path);

    }
}
