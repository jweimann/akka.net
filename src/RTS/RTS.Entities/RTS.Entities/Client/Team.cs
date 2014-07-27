using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Buildings;
using RTS.Commands.Client;
using RTS.Commands.Interfaces;
using RTS.Commands.Server;
using RTS.Commands.Team;
using RTS.Commands.Units;
using RTS.Commands.Weapons;
using RTS.ContentRepository;
using RTS.Core.Enums;
using RTS.Core.Structs;
using RTS.DataStructures;
using RTS.Entities.Buildings;
using RTS.Entities.Factories;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Stats;
using RTS.Entities.Interfaces.Teams;
using RTS.Entities.Interfaces.UnitTypes;
using RTS.Entities.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Client
{
    public class Team : UntypedActor, ITeam, IEntityController
    {
        private Akka.Actor.ActorSystem _context;
        private bool _initialized;
        private long _teamId;
        private BuildingFactory _buildingFactory;
        private UnitFactory _unitFactory;
        private ActorRef _playerActor { get; set; }
        public Dictionary<long, object> EntityActors { get; set; }

        public Team(Akka.Actor.ActorSystem context, long teamId)
        {
            EntityActors = new Dictionary<long, object>();
            this._context = context;
            this._teamId = teamId;
            _buildingFactory = new BuildingFactory(_context);
            _unitFactory = new UnitFactory(_context);
        }

        private void SpawnBuilding(UnitType unitType, string name, Vector3 position, long teamId)
        {
            long entityId;
            var building = _buildingFactory.GetEntity(new SpawnEntityData() { Name = unitType.ToString(), Position = position, TeamId = this._teamId }, out entityId);

            SpawnEntityCommand command = new SpawnEntityCommand() { Name = unitType.ToString(), Position = position, EntityId = entityId, TeamId = teamId };
            var selection = _context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Player*");

            selection.Tell(command);

            EntityActors.Add(entityId, building);
        }

        private void SpawnUnit(UnitDefinition unitDefinition, Vector3 position, long teamId)
        {
            long entityId;
            var unit = _unitFactory.GetEntity(new SpawnEntityData() { UnitType = unitDefinition.UnitType, Position = position, TeamActor = this.Self, TeamId = this._teamId }, out entityId);

            SpawnEntityCommand command = new SpawnEntityCommand() { Name = unitDefinition.UnitType.ToString(), Position = position, EntityId = entityId, TeamId = teamId };
            var selection = _context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Player*");

            selection.Tell(command);

            EntityActors.Add(entityId, unit);
        }

        public void HandleMessage(object message)
        {
            Console.WriteLine("Team recieved message" + message.ToString());

            HandleEntityRequest(message);

            if (message is IMmoCommand<ITeam>)
            {
                if ((message as IMmoCommand<ITeam>).CanExecute(this))
                {
                    (message as IMmoCommand<ITeam>).Execute(this);
                }
            }
            else if (message is BuildEntityCommand)
            {
                var msg = message as BuildEntityCommand;
                if (EntityActors.ContainsKey(msg.BuildingEntityId) == false)
                {
                    return; // Not my vehicle.  TODO: Log an error/cheat attempt?
                }
                var buildingActor = EntityActors[msg.BuildingEntityId] as ActorRef; //TODO: Will throw if you try to use the wrong team, needs to do checks here or before to make sure the entity is mine.

                buildingActor.Tell(message);
            }
            else if (message is IMmoCommand<IEntityController>)
            {
                if ((message as IMmoCommand<IEntityController>).CanExecute(this))
                {
                    if ((message as IMmoCommand<IEntityController>).CommandDestination != Destination.Server)
                    {
                        var selection = _context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Player*");

                        selection.Tell(message);
                        //_playerActor.Tell(message);
                    }
                    //(message as IMmoCommand<IEntityController>).Execute(this); //TODO: this is an infinite loop with spawnentitycommand, after seperating from player, find a clean way for this to work.
                }
            }
            
            if (message is IVehicleCommand && ((IVehicleCommand)message).TellServer)
            {
                var msg = message as IVehicleCommand;
                foreach (var entityId in msg.EntityIds)
                {
                    if (EntityActors.ContainsKey(entityId))
                    {
                        var vehicleActor = EntityActors[entityId] as ActorRef;
                        vehicleActor.Tell(msg);
                    }
                }
            }
            if (message is UpdateStatsCommand || message is FireWeaponCommand || message is IEntityComponentCommand)
            {
                if ((message as MmoCommand).CommandDestination != Destination.Server)
                {
                    SendCommandToAllPlayers(message);
                }
                else
                {
                    var msg = message as IEntityComponentCommand;
                    if (this.EntityActors.ContainsKey(msg.EntityId))
                    {
                        var entityActor = this.EntityActors[msg.EntityId] as ActorRef;
                        if (entityActor != null)
                        {
                            entityActor.Tell(message);
                        }
                    }
                }
            }
          
        }

        private void SendCommandToAllPlayers(object message)
        {
            var selection = _context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Player*");
            selection.Tell(message);
        }

  

        public void Update()
        {
            if (_initialized == false)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            Random rand = new Random();
            Vector3 spawnPoint = new Vector3(rand.Next(-100, 100), 0, rand.Next(-100, 100));
            SpawnBuilding(UnitType.TruckDepot, "TruckDepot", spawnPoint, this._teamId);
            _initialized = true;
        }

    

        public void AddEntity(long entityId, string name, Vector3 position, object sender)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(long EntityId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<long, Interfaces.Movement.IMover> GetEntities()
        {
            throw new NotImplementedException();
        }

        public void SetBounds(Bounds Bounds)
        {
            throw new NotImplementedException();
        }

        public void MoveEntity(object sendMovementToAOICommand)
        {
            throw new NotImplementedException();
        }

        public void SendEntitiesInAreaListToEntity(object sender)
        {
            throw new NotImplementedException();
        }

        public int GetId()
        {
            throw new NotImplementedException();
        }

        public void SpawnEntity(string name, Vector3 position, long entityId, long teamId)
        {
            SpawnBuilding(UnitType.TruckDepot, name, position, teamId);
        }

        public void HandleRequest(object request)
        {
            throw new NotImplementedException();
        }

        UnitDefinitionRepository _repository = new UnitDefinitionRepository();
        public void FinishBuildEntity(UnitType unitType, Vector3 position)
        {
            var definition = _repository.Get(unitType);
            SpawnUnit(definition, position, this._teamId);
            //switch (name)
            //{
            //    case "Truck":
            //        SpawnUnit(UnitType.Truck, name, position, this._teamId);
            //        break;
            //    case "stugIII":
            //        SpawnUnit(UnitType.StugIII, name, position, this._teamId);
            //        break;
            //    case "TruckDepot":
            //        SpawnBuilding(UnitType.TruckDepot, name, position, this._teamId);
            //        break;
            //}
        }


        public void SetEntity(Interfaces.IEntity entity)
        {
            throw new NotImplementedException();
        }


        public void SetPlayer(object player)
        {
            this._playerActor = player as ActorRef;

            var teams = Context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Team*");
            teams.Tell(new PlayerConnectedCommand() { PlayerActor = player });
        }

        #region UnTypedActor
        protected override void OnReceive(object message)
        {
            HandleMessage(message);
        }

        protected override void PreStart()
        {
            base.PreStart();

            //Initialize(); // Player not set yet can't call until then while units are still associated with player.  Once moved to tearerrm this can be re-implemented.
            _context.Scheduler.Schedule(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100), () => Update());
        }

        #endregion






        public void SetUnitPath(long unitId, List<Vector3> path)
        {
            throw new NotImplementedException();
        }


        public async void HandlePlayerJoined(object PlayerActor)
        {
            ActorRef joinedPlayerActor = PlayerActor as ActorRef;

            foreach (var key in this.EntityActors.Keys)
            {
                ActorRef unitActor = this.EntityActors[key] as ActorRef;
                SpawnEntityData spawnEntityData = await unitActor.Ask<SpawnEntityData>(EntityRequest.GetSpawnData);
                SpawnEntityCommand command = new SpawnEntityCommand() { Name = spawnEntityData.Name, Position = spawnEntityData.Position, EntityId = spawnEntityData.EntityId };
                //var selection = _context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Player*");

                joinedPlayerActor.Tell(command);
            }
            
        }




        public async void DestroyEntity(long entityId)
        {
            if (this.EntityActors.ContainsKey(entityId) == false)
            {
                Console.WriteLine("Tried to destroy an entity that this Team doesn't control.  It may have already been destroyed previously.");
                return;
            }

            ActorRef actor = this.EntityActors[entityId] as ActorRef;
            bool stopped = await actor.GracefulStop(TimeSpan.FromSeconds(1));
            this.EntityActors.Remove(entityId);
            
            SendCommandToAllPlayers(new DestroyEntityCommand() { EntityId = entityId });
        }


        private void HandleEntityRequest(object message)
        {
            if (message is EntityRequest)
            {
                switch ((EntityRequest)message)
                {
                    case EntityRequest.GetTeam:
                        Sender.Tell(this._teamId);
                        break;
                }
            }
        }

    }
}