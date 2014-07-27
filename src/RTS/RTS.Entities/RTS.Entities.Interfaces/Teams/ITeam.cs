using RTS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Teams
{
    public interface ITeam
    {
        Dictionary<long, object> EntityActors { get; set; }

        void FinishBuildEntity(UnitType unitType, Core.Structs.Vector3 vector3);

        void SetPlayer(object player);

        void HandlePlayerJoined(object PlayerActor);

        void DestroyEntity(long p);

    }
}
