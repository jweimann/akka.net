using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Interfaces;
using RTS.Core.Structs;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Movement;
using RTS.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Movement
{
    public class Mover : EntityComponent<IMover>, IMover
    {
        public Bounds Bounds { get; set; }
        public Vector3 Position { get; protected set; }
        public Vector3 Velocity { get; protected set; }
        public Vector3 Destination { get; set; }

        protected bool _dirty;

        public override void HandleMessage(object message)
        {
            IMmoCommand<IMover> command = message as IMmoCommand<IMover>;
            if (command != null)
            {
                if (command.CanExecute(this))
                {
                    command.Execute(this);
                }
            }
        }

        public override void Update(double deltaTime)
        {
            if (_dirty)
            {
                 _dirty = false;
            }
        }

        public void SetPosition(Core.Structs.Vector3 vector3)
        {
            if (this.Position != vector3)
            {
                this.Position = vector3;
    
                _dirty = true;
            }
        }

        public void SetVelocity(Core.Structs.Vector3 vector3)
        {
            if (this.Velocity != vector3)
            {
                this.Velocity = vector3;
                _dirty = true;
            }
        }

        public void SetDestination(Vector3 destination)
        {
            this.Destination = destination;
        }
    }
}
