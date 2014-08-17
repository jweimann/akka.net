using Akka.Actor;
using RTS.Commands.Resources;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.UnitTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Buildings
{
    public class ResourceGenerator : IResourceGenerator
    {
        private IEntity _entity;
        private double _timer;
        public int Amount { get; set; }
        public double Interval { get; set; }

        public void MessageComponents(object message)
        {
            //throw new NotImplementedException();
        }

        public void SetEntity(Interfaces.IEntity entity)
        {
            _entity = entity;
        }

        public void HandleMessage(object message)
        {
            //throw new NotImplementedException();
        }

        public void Tick(double deltaTime)
        {
            if (Amount == 0)
                return;

            _timer += deltaTime;
            if (_timer >= Interval)
            {
                _timer -= Interval;
                AddMoney(this.Amount);
            }
        }

        private void AddMoney(int p)
        {
            ModifyResourcesCommand cmd = new ModifyResourcesCommand() { Amount = p };
            ActorRef entityActor = _entity.GetActorRef() as ActorRef;
            _entity.MessagePlayer(cmd);
            //entityActor.Tell(cmd);
        }

        public void PreStart()
        {
            _timer = 0.0;
        }
    }
}
