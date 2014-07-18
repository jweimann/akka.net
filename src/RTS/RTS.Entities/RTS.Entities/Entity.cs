using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

using Akka.Actor;
using Helios.Net;

using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Stats;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Movement;
using RTS.Entities.Requests;
using RTS.Core.Structs;
using RTS.Commands;
using RTS.Commands.Interfaces;
using RTS.Commands.Server;
using RTS.Entities.Movement;
using RTS.Core.Constants;
using System.Runtime.Remoting.Contexts;
using RTS.Entities.Interfaces.Teams;
using RTS.Commands.Units;
using RTS.Commands.Client;
using RTS.Core.Enums;
using RTS.Entities.Interfaces.EntityComponents;
using RTS.Commands.Team;

namespace RTS.Entities
{
    public class Entity : UntypedActor, IEntity
    {
        private List<IEntityComponent> _components = new List<IEntityComponent>();
        private double UPDATE_TIMER = 66;
        private Vector3 _currentAreaOfInterestCenter;
        private IActorContext _context;
        private bool _dirty;
        private DateTime _lastUpdateTime;
        private SpawnEntityData _spawnEntityData; // Keeping this with team and other info, maybe just keep this around and not have a bunch of individual vars.
        private ActorRef _entityActorRef;

        public ActorRef TeamActor { get; set; } // TODO: Switch this to be the team?  Make Team an Actor?  Also using ActorRef instead of selection because selection was failing for some unknown reason.
        public Vector3 Position { get; set; }
        public ActorSelection AreaOfInterest { get; set; }
        public Int64 Id { get; protected set; }
        List<IEntityComponent> IEntity.Components { get { return _components; } }
        public Entity(Int64 entityId, List<IEntityComponent> components, SpawnEntityData data)
        {
            this.Id = entityId;
            this.TeamActor = Context.System.ActorSelection("user/Team" + data.TeamId).ResolveOne(TimeSpan.FromSeconds(1)).Result;// data.TeamActor as ActorRef;
            this.Position = data.Position;
            
            this._spawnEntityData = data;
            
            foreach (var component in components)
            {
                AddComponent(component);
            }
        }
        protected override void PreStart()
        {
            _context = Context;
            _entityActorRef = this.Self;
            //_areaOfInterestCollection = Context.System.ActorSelection("akka.tcp://MyServer@localhost:2020/user/AreaOfInterestCollection");
            
            //GetAreaOfInterest(Vector3.zero);
            Context.System.Scheduler.Schedule(TimeSpan.FromMilliseconds(UPDATE_TIMER), TimeSpan.FromMilliseconds(UPDATE_TIMER), () => Update());

            if (GetComponent<NpcMover>() != null)
            {
                //Context.System.Scheduler.ScheduleOnce(TimeSpan.FromSeconds(20), () => TurnAround());
            }

            foreach (var component in _components)
            {
                component.PreStart();
            }
        }

        private void TurnAround()
        {
            GetComponent<NpcMover>().TurnAround();
        }

        private void Update()
        {
            double deltaTime = (DateTime.Now - _lastUpdateTime).TotalSeconds;
            _lastUpdateTime = DateTime.Now;
            foreach (var component in _components)
            {
                component.Update(deltaTime);
            }
        }

        public void AddComponent(IEntityComponent component)
        {
            if (_components.Contains(component) == false)
            {
                _components.Add(component);
                component.SetEntity(this);
            }
            else
            {
                Console.WriteLine(String.Format("Error trying to add duplicate component Type: {0}", component.GetType().ToString()));
            }
            
            component.SetEntity(this);
        }

        private void HandleMessage<T>(IMmoCommand<T> command) where T : IEntityComponent
        {
            //if (command is IMmoCommand<IEntityComponent>)
            {
                IMmoCommand<IEntityComponent> entityComponentCommand = (IMmoCommand<IEntityComponent>)command;// as IMmoCommand<IEntityComponent>; //TODO: Why doesn't this work?  IEntityTargeter is based on IEntityComponent.  Ask MATT :)
                //var entityComponentCommand = command as IMmoCommand<IEntityTargeter>;
                foreach (var component in _components)
                {
                    if (component is IEntityComponent)
                    {
                        if (entityComponentCommand.CanExecute(component as IEntityComponent)) //TODO: Don't want to do this, need to make this work generic..
                        {
                            entityComponentCommand.Execute(component as IEntityComponent);
                        }
                    }
                }
            }
        }

        public void MessageComponents(object message)
        {
            foreach (var component in _components)
            {
                component.HandleMessage(message);
            }
        }
    
        private void ForwardMessageToController(object message)
        {
            var controller = GetComponent<IController>();
            if (controller != null)
            {
                controller.HandleRequest(message);
            }
        }

        private void ForwardMessageToPlayer(object message)
        {
            TeamActor.Tell(message);
        }

        private T GetComponent<T>()
        {
            foreach (var component in _components)
            {
                if (typeof(T).IsAssignableFrom(component.GetType()))
                {
                    return (T)component;
                }
            }
            return default(T);
        }

        public object GetActorRef()
        {
            return _entityActorRef;
        }

        public object GetActorContext()
        {
            return this._context;
        }

        protected override void OnReceive(object message)
        {
            if (message is EntityRequest)
            {
                switch ((EntityRequest)message)
                {
                    case EntityRequest.GetTeam:
                        Sender.Tell(_spawnEntityData.TeamId);
                        break;
                    case EntityRequest.GetSpawnData:
                        {
                            SpawnEntityData data = new SpawnEntityData() { EntityId = this.Id, Name = this._spawnEntityData.Name, Position= this.Position, TeamId = this._spawnEntityData.TeamId };
                            Sender.Tell(data);
                        }
                        break;
                }
            }

            if (message is GetPositionCommand)
            {
                //Console.WriteLine("Sending my Position for Entity " + this.Id + " Position " + this.Position.ToString());
                Sender.Tell(this.Position);
            }

            MmoCommand command = message as MmoCommand;
            if (command == null)
            {
                MessageComponents(message);
            }
            else if (command.GetType().IsAssignableFrom(typeof(IMmoCommand<IController>)))
            {
                ForwardMessageToController(command);
            }
            else if (command is IMmoCommand<ITeam>)
            {
                ForwardMessageToPlayer(command); // TODO: Change this to team and make team an actor?
            }
            //else if (typeof(IMmoCommand<IEntityComponent>).IsAssignableFrom(command.GetType()))
            //{
            //    HandleMessage(command);
            //}
            else if (command.GetType().GetInterfaces().Contains(typeof(IMmoCommand<IEntityTargeter>)))
            {
                MessageComponents(command);
                //HandleMessage(command);
            }
            else
            {
                MessageComponents(command);
            }
        }

        public void MessageTeam(object message)
        {
            TeamActor.Tell(message);
        }


        public SpawnEntityData GetSpawnEntityData()
        {
            return _spawnEntityData;
        }

        public void Destroy()
        {
            TeamActor.Tell(new DestroyEntityCommand() { EntityId = this.Id });
        }


    }
}
