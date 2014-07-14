using Akka.Actor;
using RTS.Core.Structs;
using System;

namespace RTS.Entities.Structs
{
    public class EntityInfo
    {
        public Int64 Id;
        public Vector3 Position;
        public ActorRef ActorRef;
    }
}
