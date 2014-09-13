using RTS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Entities.Interfaces.Stats
{
    public interface IStats : IEntityComponent
    {
        void TakeDamage(int damage);
        void SetStat(StatId statId, int value, int max);
        int GetStatValue(StatId statId);
    }
}
