﻿using Akka.Actor;
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
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.EntityComponents;
using RTS.Entities.Interfaces.Player;
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

        private void SpawnUnit(UnitDefinition unitDefinition, Vector3 position, long teamId)
        {
            long entityId;

            ActorRef unit = null;
            Stats.Stats stats = null;

            switch(unitDefinition.UnitType)
            {
                case UnitType.TruckDepot:
                case UnitType.BaseStation:
                case UnitType.Harvester:
                    unit = _buildingFactory.GetEntity(new SpawnEntityData() { UnitType = unitDefinition.UnitType, Name = unitDefinition.Name, Position = position, TeamId = this._teamId }, out entityId, out stats);
                    break;
                default:
                    unit = _unitFactory.GetEntity(new SpawnEntityData() { UnitType = unitDefinition.UnitType, Name = unitDefinition.Name, Position = position, TeamActor = this.Self, TeamId = this._teamId }, out entityId, out stats);
                    break;
            }

            
            SpawnEntityCommand command = new SpawnEntityCommand(position, unitDefinition.UnitType, entityId, teamId, stats.AllValues);
            var selection = _context.ActorSelection("akka.tcp://MyServer@localhost:2020/user/Player*");

            selection.Tell(command);

            EntityActors.Add(entityId, unit);
        }

        private Func<object, Dictionary<long, object>, bool> HandleBuildEntityCommand = new Func<object, Dictionary<long, object>, bool>((message, entityActors) =>
            {
                var msg = message as BuildEntityCommand;
                if (entityActors.ContainsKey(msg.BuildingEntityId) == false)
                {
                    //System.Diagnostics.Debugger.Break();
                    //throw new ArgumentOutOfRangeException("Player tried to build from an entity on another team.");
                    //return; // Not my vehicle.  TODO: Log an error/cheat attempt?
                    Console.WriteLine("Player tried to build from an entity on another team.");
                    return false;
                }
                else
                {
                    var buildingActor = entityActors[msg.BuildingEntityId] as ActorRef; //TODO: Will throw if you try to use the wrong team, needs to do checks here or before to make sure the entity is mine.

                    buildingActor.Tell(message);
                    return true;
                }
            });

   
        private void DebugSendBuildingCommandToPlayer(object message)
        {
            SendCommandToAllPlayers(message);
        }

        public void HandleMessage(object message)
        {
            //Console.WriteLine("Team recieved message" + message.ToString());

            HandleEntityRequest(message);

            //Console.WriteLine("DEBUG - TEAM RECIEVED MESSAGE: " + message.GetType().ToString());

            CommandMatch.Match(message)
                .WithServer<ITeam>(() => (message as IMmoCommand<ITeam>).Execute(this))
                .WithServer<MoveUnitsCommand>(() => HandleMoveUnitsCommand(message))
                .WithServer<IBuilding>(() => HandleBuildEntityCommand(message, EntityActors))
                .WithServer<IVehicle>(() => SendMessageToEntities(message))
                .WithServer<IStats>(() => SendMessageToEntities(message))
                .WithServer<IWeapon>(() => SendMessageToEntities(message))
                .WithServer<IEntityTargeter>(() => SendMessageToEntities(message))
                .WithServer<IPlayer>(() => SendMessageToPlayerActor(message))

                .WithClient<IEntityController>(() => SendCommandToAllPlayers(message))
                .WithClient<IStats>(() => SendCommandToAllPlayers(message))
                .WithClient<IWeapon>(() => SendCommandToAllPlayers(message))
                .WithClient<IBuilding>(() => DebugSendBuildingCommandToPlayer(message)) // should this only go to the owner not all?  might need to update percents on everyone for visuals?
                ;

          
        }

        private void SendMessageToPlayerActor(object message)
        {
            if (_playerActor == null)
                Console.WriteLine("ERROR _playerActor NULL");
            else
                _playerActor.Tell(message);
        }

        private void SendMessageToEntities(object message)
        {
            var msg = message as IEntityComponentCommand;
            MessageActorById(msg.EntityId, msg);
        }

        private void MessageActorById(long id, IEntityComponentCommand command)
        {
            if (this.EntityActors.ContainsKey(id))
            {
                var entityActor = this.EntityActors[id] as ActorRef;
                if (entityActor != null)
                {
                    entityActor.Tell(command);
                }
            }
        }

        private void HandleMoveUnitsCommand(object message)
        {
            MoveUnitsCommand command = message as MoveUnitsCommand;
            foreach(var entityId in command.EntityIds)
            {
                MessageActorById(entityId, command);
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
            Vector3 spawnPoint = GetNextSpawnLocation();// new Vector3(rand.Next(-100, 100), 0, rand.Next(-100, 100));
            UnitDefinition definition = _repository.Get(UnitType.BaseStation);
            SpawnUnit(definition, spawnPoint, this._teamId);
            _initialized = true;
        }

        private List<Vector3> _spawnLocations = new List<Vector3>() { 
            new Vector3(-338.6f, 20.5f, -275.35f), 
            new Vector3(369.25f, 20.5f, -277.45f),
            new Vector3(318.5f, 20.5f, -389.35f), 
            new Vector3(-327.0f, 20.5f, -328.5f)
        };

        private static int _nextSpawnLocationIndex = 0; //TODO: Make this real
        private Vector3 GetNextSpawnLocation()
        {
            Vector3 spawnLoc = _spawnLocations[_nextSpawnLocationIndex];
            _nextSpawnLocationIndex++;
            if (_nextSpawnLocationIndex >= _spawnLocations.Count)
                _nextSpawnLocationIndex = 0;

            Random rng = new Random();

            spawnLoc += new Vector3(rng.Next(-200, 200), 0, rng.Next(-200, 200));

            return spawnLoc;
        }

        public void AddEntity(long entityId, string name, Vector3 position, object sender)
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity(long EntityId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<long, IEntity> GetEntities()
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

        public void SpawnEntity(UnitType unitType, Vector3 position, long entityId, long teamId, List<Stat> stats)
        {
            // Stats not used here
            var definition = _repository.Get(UnitType.BaseStation);
            SpawnUnit(definition, position, teamId);
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

            List<long> keys = new List<long>(this.EntityActors.Keys);

            foreach (var key in keys)
            {
                ActorRef unitActor = this.EntityActors[key] as ActorRef;
                SpawnEntityData spawnEntityData = await unitActor.Ask<SpawnEntityData>(EntityRequest.GetSpawnData);
                List<Stat> tempStats = (List<Stat>)spawnEntityData.Stats;
                string statsString = String.Empty;
                foreach (var stat in tempStats)
                {
                    statsString += String.Format("{0}={1}/{2} ", stat.Name, stat.Value, stat.Max);
                }
                Console.WriteLine(String.Format("Sending Existing Entity. {0} Stats: {1}", spawnEntityData.ToString(), statsString));
                SpawnEntityCommand command = new SpawnEntityCommand(spawnEntityData.Position, spawnEntityData.UnitType, spawnEntityData.EntityId, spawnEntityData.TeamId, tempStats);// { Name = spawnEntityData.Name, Position = spawnEntityData.Position, EntityId = spawnEntityData.EntityId };
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
            try
            {
                bool stopped = await actor.GracefulStop(TimeSpan.FromSeconds(1));
            } catch (Exception ex)
            {

            }
            this.EntityActors.Remove(entityId);
            
            SendCommandToAllPlayers(new DestroyEntityCommand() { EntityId = entityId });
        }


        private async void HandleEntityRequest(object message)
        {
            if (message is EntityRequest)
            {
                switch ((EntityRequest)message)
                {
                    case EntityRequest.GetTeam:
                        Sender.Tell(this._teamId);
                        break;
                    case EntityRequest.GetRandomBuilding:
                        var firstBuildingActorRef = EntityActors.Where(t => t.Key < 5000).FirstOrDefault().Value as ActorRef;
                        var spawnEntityData = await firstBuildingActorRef.Ask<SpawnEntityData>(EntityRequest.GetSpawnData);
                        Sender.Tell(spawnEntityData); // Have entitiy actor reply or do something else???
                        break;
                }
            }
        }

    }
}