using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Teams
{
    public interface ITeam
    {
        Dictionary<long, object> EntityActors { get; set; }

        void FinishBuildEntity(string name, Core.Structs.Vector3 vector3);

        void SetPlayer(object player);

        void HandlePlayerJoined(object PlayerActor);

        void DestroyEntity(long p);

    }
}
