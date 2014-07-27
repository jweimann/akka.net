﻿using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Buildings;
using RTS.Commands.Client;
using RTS.Commands.Interfaces;
using RTS.Commands.Server;
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
        private const int STARTING_MONEY = 500;

        private RTSHeliosNetworkClient _client;
        private List<IPlayerComponent> _components;
        private ActorRef _team;
        private long _teamId; // Not set on server atm.
        private int _money;
        
        public Player(RTSHeliosNetworkClient client, List<IPlayerComponent> components)
        {
            _client = client;
            _components = components;
            _money = STARTING_MONEY;
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
            _money += 10;
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
            if (command is MmoCommand<IPlayer>)
            {
                if ((command as MmoCommand<IPlayer>).CanExecute(this))
                {
                    (command as MmoCommand<IPlayer>).Execute(this);
                }
                if ((command as MmoCommand<IPlayer>).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IPlayer>);
                }
            }
            if (command is IEntityTargeterCommand)
            {
                if (((MmoCommand<IEntityTargeter>)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IEntityTargeter>);
                }
                else if (((MmoCommand<IEntityTargeter>)command).TellServer)
                {
                    _team.Tell(command);
                }
            }
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
            if (command is IEntityControllerCommand)
            {
                if (((IEntityControllerCommand)command).TellClient)
                {
                    _client.SendCommand(command as IEntityControllerCommand);
                }
                if (((IEntityControllerCommand)command).TellServer)
                {
                    _team.Tell(command);
                }
            }
            if (command is IVehicleCommand)
            {
                if (((MmoCommand)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IVehicle>);
                }
                if (((MmoCommand)command).TellServer)
                {
                    _team.Tell(command);
                }
            }
            if (command is UpdateStatsCommand)
            {
                if (((MmoCommand)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IStats>);
                }
                if (((MmoCommand)command).TellServer)
                {
                    _team.Tell(command);
                }
            }
            if (command is MmoCommand<IWeapon>)
            {
                if (((MmoCommand)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IWeapon>);
                }
                if (((MmoCommand)command).TellServer)
                {
                    _team.Tell(command);
                }
            }
            if (command is MmoCommand<ITeam>)
            {
                if (((MmoCommand)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<ITeam>);
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
            switch (unitType)
            {
                case Core.Enums.UnitType.Truck:
                    return 100;
                case Core.Enums.UnitType.StugIII:
                    return 150;
                case Core.Enums.UnitType.TruckDepot:
                    return 250;
                default:
                    return 0;
            }
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


        public void HandlePlayerDisconnected(object PlayerActor)
        {
            //_components.FirstOrDefault(t=> t is Ic)
        }
    }
}
