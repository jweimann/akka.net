using RTS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.DataStructures
{
    [Serializable]
    public class Stat
    {
        public StatId Id { get; set; }
        public int Max { get; set; }
        public int Value { get; set; }
        public string Name { get { return Id.ToString(); } }
        public float Pct
        {
            get
            {
                if (Max == 0 || Value == 0)
                    return 0;
                return (float)Value / (float)Max;
            }
        }

        public Stat(StatId id, int max)
        {
            this.Id = id;
            this.Max = max;
            this.Value = this.Max;
        }
    }
}
