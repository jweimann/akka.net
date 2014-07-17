using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Core.Enums
{
    public enum CommandId : byte
    {
        Move,
        BatchMove,
        DamageEntity,
        UpdateProperty,
        UpdateStats,
        SpawnEntity,
        RequestEntityStats,
        RequestEntities,
        EntitiesInAreaList,
        JoinAreaOfInterestCommand,
        SetBounds,
        SendMovementToAOI,
        AOIDetailsCommand,
        SetDestination,
        BuildEntity,
        FinishBuildEntity,
        MoveUnits,
        SetPlayer,
        SetTeam,
        SetPathOnClient,
        AttackUnit,
        PlayerJoined,
        ModifyStat,
        DestroyEntity,
    }
}
