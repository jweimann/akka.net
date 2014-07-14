using Akka.Actor;
using RTS.Commands;
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
    public class NpcMover : Mover
    {
        private Random _random;
        private double _lastTime;
        private DateTime _lastDirty = DateTime.MinValue;

        private bool _hasUpdatedSinceSpawn = false;
        private DateTime _spawnTime = DateTime.Now;

        public NpcMover()
        {
            if (_random == null)
            {
                _random = new Random();
                System.Threading.Thread.Sleep(10);
            }
            _dirty = true;
        }
        public NpcMover(SpawnEntityData data)
        {
            this.Position = data.Position;
            if (_random == null)
            {
                _random = new Random();
                System.Threading.Thread.Sleep(10);
            }
            _dirty = true;
        }
        public override void PreStart()
        {
            SetVelocity(new Vector3(1f, 0, 0));
        }
        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            long now = DateTime.Now.Ticks;
            if (_lastTime == 0) _lastTime = now;
            //double deltaTime = (double)(now - _lastTime) / ((double)TimeSpan.TicksPerMillisecond * 1000.0);
            _lastTime = now;

            //if (Velocity == Vector3.zero)
                TryTurn();
            //SetVelocity(new Vector3(1.0f, 0f, 1.0f));
            
            SetPosition(this.Position + (this.Velocity * deltaTime));

            if (AreaOfInterest != null)
            {
                if (_dirty || (DateTime.Now - _lastDirty).TotalSeconds > 10 || _hasUpdatedSinceSpawn == false)
                {
                    if (_hasUpdatedSinceSpawn == false)
                    {
                        Console.WriteLine(String.Format("First movement sent EntityId: {0}  Position: {1}  SpawnedAt: {2}  Delay: {3}", _entity.Id, this.Position.ToString(), _spawnTime.ToString(), (DateTime.Now - _spawnTime).ToString()));
                    }

                    _dirty = false;
                    _lastDirty = DateTime.Now;
                    _hasUpdatedSinceSpawn = true;
                    //UpdateAreaOfInterest();
                }
            }
        }
        public override void HandleMessage(object message)
        {
            
        }

        //private void UpdateAreaOfInterest()
        //{
        //    if (this.AreaOfInterest == null)
        //    {
        //        GetAreaOfInterest(this.Position);
        //        return;
        //    }

        //    ActorRef sender = this._entity.GetActorRef() as ActorRef;
        //    this.AreaOfInterest.Tell(new MoveRequest() { Velocity = this.Velocity, Dirty = true, EntityId = this._entity.Id, Position = this.Position }, sender);
        //}

        private bool TryTurn()
        {
            int randValue = _random.Next(50);
            //Console.WriteLine("Rolled " + randValue);
            if (randValue == 1)
            {
                PickRandomDirection();
                return true;
            }
            return false;
        }
        private void PickRandomDirection()
        {
            SetVelocity(new Vector3(_random.Next(-5, 6), _random.Next(-5, 6) * 0, _random.Next(-5, 6)));

            //Console.WriteLine(String.Format("Entity: {0}  Turning to: {1}", this.Self.ToString(), _direction.ToString()));
        }

        internal void TurnAround()
        {
            SetVelocity(this.Velocity * -1.0);
        }
    }
}
