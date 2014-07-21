using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Interfaces;
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
        private RTSHeliosNetworkClient _client;
        private List<IPlayerComponent> _components;
        private ActorRef _team;
        public Player(RTSHeliosNetworkClient client, List<IPlayerComponent> components)
        {
            _client = client;
            _components = components;
            foreach (var component in _components)
            {
                component.SetPlayer(this);
            }
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
                    _team.Tell(command);
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

        public void MessageComponents(object message)
        {
            foreach (var component in _components)
            {
                component.HandleMessage(message);
            }
        }

        public void SetTeam(object team)
        {
            _team = team as ActorRef;
        }
    }
}
