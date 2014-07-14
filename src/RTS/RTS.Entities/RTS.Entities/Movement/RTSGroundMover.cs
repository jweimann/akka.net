using RTS.Core.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Movement
{
    public class RTSGroundMover : Mover
    {
        public float Speed { get; set; }

        public RTSGroundMover (SpawnEntityData data)
        {
            this.Position = data.Position;
            _dirty = true;
        }
        public override void PreStart()
        {
            this.Speed = 1f;
            base.PreStart();
        }
        public override void Update(double deltaTime)
        {
            MoveTowardDestination();
            base.Update(deltaTime);
        }

        private void MoveTowardDestination()
        {
            if (this.Position != this.Destination)
            {
                Vector3 direction = this.Destination - this.Position;
                this.Position += direction * this.Speed;
            }
        }

    }
}
