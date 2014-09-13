using Akka.Actor;
using JW.Behavior.Interfaces;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JW.Behavior
{
    public class Brain : IBrain
    {
        private List<JW.Behavior.Interfaces.IBehavior> _behaviors = new List<JW.Behavior.Interfaces.IBehavior>(); // NEW Behavior system

        public IEntity Entity { private set; get; }
        public IVehicle Vehicle { get; private set; }
        public IActorContext Context { get; private set; }

        public Brain(IEntity entity, IVehicle vehicle, IActorContext context)
        {
            this.Entity = entity;
            this.Vehicle = vehicle;
            this.Context = context;
        }

        public void AddBehavior(JW.Behavior.Interfaces.IBehavior behavior)
        {
            _behaviors.RemoveAll(p => p.GetType() == behavior.GetType());

            behavior.Completed = () =>
            {
                _behaviors.Remove(behavior);
                if (behavior.Then != null)
                {
                    AddBehavior(behavior.Then);
                }
            };

            
            _behaviors.Add(behavior);
        }


        public void Tick(double deltaTime)
        {
            for (int i = 0; i < _behaviors.Count; i++)
            {
                _behaviors[i].Tick(deltaTime);
            }
        }
    }
}
