using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Buildings;
using RTS.Commands.Client;
using RTS.Commands.Interfaces;
using RTS.Commands.Server;
using RTS.ContentRepository;
using RTS.Core.Enums;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.EntityComponents;
using RTS.Entities.Interfaces.Player;
using RTS.Entities.Interfaces.Stats;
using RTS.Entities.Interfaces.Teams;
using RTS.Entities.Interfaces.UnitTypes;
using RTS.Networking.Helios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Player
{
    public class Player : UntypedActor, IPlayer
    {
        private const int STARTING_MONEY = 1500;

        private RTSHeliosNetworkClient _client;
        private List<IPlayerComponent> _components;
        private ActorRef _team;
        private long _teamId; // Not set on server atm.
        private int _money;
        private UnitDefinitionRepository _repository;
        
        public Player(RTSHeliosNetworkClient client, List<IPlayerComponent> components)
        {
            _client = client;
            _components = components;
            _money = STARTING_MONEY;
            _repository = new UnitDefinitionRepository();
            foreach (var component in _components)
            {
                component.SetPlayer(this);
            }
        }
        protected override void PreStart()
        {
            SendInfoToClient();
            Context.System.Scheduler.Schedule(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), () => Update());
            base.PreStart();
        }

        private void Update()
        {
            //_money += 10;
            SendInfoToClient();
        }

        private void SendInfoToClient()
        {
            _client.SendCommand(new SetClientPlayerInfoCommand() { TeamId = _teamId, Money = _money });
        }
        protected override void OnReceive(object message)
        {
            HandleCommand(message);
        }

        public void HandleCommand(object command)
        {

            CommandMatch.Match(command)
                .WithServer<IEntityController>(() => _team.Tell(command))
                .WithServer<IEntityTargeter>(() => _team.Tell(command))
                //.WithServer<IBuilding>(() => _team.Tell(command))
                .WithServer<IWeapon>(() => _team.Tell(command))
                .WithServer<IPlayer>(() => ((MmoCommand<IPlayer>)command).Execute(this))
                .WithServer<IStats>(() => _team.Tell(command))
                .WithServer<IVehicle>(() => _team.Tell(command))
                .WithServer<IWeapon>(() => _team.Tell(command))

                .WithClient<IEntityController>(() => _client.SendCommand(command as MmoCommand<IEntityController>))
                .WithClient<IPlayer>(() => _client.SendCommand(command as MmoCommand<IPlayer>))
                .WithClient<IEntityTargeter>(() => _client.SendCommand(command as MmoCommand<IEntityTargeter>))
                .WithClient<IStats>(() => _client.SendCommand(command as MmoCommand<IStats>))
                .WithClient<IVehicle>(() => _client.SendCommand(command as MmoCommand<IVehicle>))
                .WithClient<IWeapon>(() => _client.SendCommand(command as MmoCommand<IWeapon>))
                .WithClient<ITeam>(() => _client.SendCommand(command as MmoCommand<ITeam>))
                .WithClient<IBuilding>(() => _client.SendCommand(command as MmoCommand<IBuilding>))
                ;

            if (command is IBuildingCommand)
            {
                if (((MmoCommand<IBuilding>)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IBuilding>);
                }
                if (((MmoCommand<IBuilding>)command).TellServer)
                {
                    if (IsBuildEntityCommand(command))
                    {
                        if (CanAffordBuild(command))
                        {
                            _money -= GetUnitCost(((BuildEntityCommand)command).UnitTypeId);
                            _team.Tell(command);
                        }
                    }
                    else
                    {
                        _team.Tell(command);
                    }
                }
            }
        }

        private bool CanAffordBuild(object command)
        {
            BuildEntityCommand cmd = command as BuildEntityCommand;
            return this._money > GetUnitCost(cmd.UnitTypeId);
        }

        private int GetUnitCost(Core.Enums.UnitType unitType)
        {
            return _repository.Get(unitType).Cost;
        }

        private bool IsBuildEntityCommand(object command)
        {
            return command is BuildEntityCommand;
        }

        public void MessageComponents(object message)
        {
            foreach (var component in _components)
            {
                component.HandleMessage(message);
            }
        }

        public async void SetTeam(object team)
        {
            _team = team as ActorRef;
            _teamId = await _team.Ask<long>(EntityRequest.GetTeam);
        }

        public void SetTeamId(long teamId)
        {
            _teamId = teamId;
        }

        public void SetMoney(int money)
        {
            _money = money;
        }
        public int GetMoney()
        {
            return _money;
        }

        public void HandlePlayerDisconnected(object PlayerActor)
        {
            //_components.FirstOrDefault(t=> t is Ic)
        }


      
    }
}
