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
    public abstract class BehaviorBase : IBehavior
    {
        private bool _initialized = false;

        protected double deltaTime;
        protected Brain _brain;

        protected IVehicle Vehicle { get { return _brain.Vehicle; } }
        protected IEntity Entity { get { return _brain.Entity; } }
        protected IActorContext Context { get { return _brain.Context; } }

        public Action Initialize;
        public Action Update;
        public Action Completed { get; set; }
        public IBehavior Then { get; set; }

        public BehaviorBase(Brain brain)
        {
            _brain = brain;
        }
        
        public void Tick(double deltaTime)
        {
            this.deltaTime = deltaTime;
            if (!_initialized && Initialize != null)
            {
                _initialized = true;
                Initialize();
            }

            Update();
        }
    }
}
