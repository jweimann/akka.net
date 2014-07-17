using RTS.Core.Structs;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Movement;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces
{
    public interface IEntity
    {
        List<IEntityComponent> Components { get; }
        void MessageComponents(object message);
        void AddComponent(IEntityComponent component);
        Int64 Id { get; }
        Vector3 Position { get; set; }
        object GetActorRef();
        object GetActorContext();
        void MessageTeam(object message);
        SpawnEntityData GetSpawnEntityData();
        void Destroy();
    }
}
