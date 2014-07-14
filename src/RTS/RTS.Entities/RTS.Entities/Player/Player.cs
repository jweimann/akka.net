using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Player;
using RTS.Entities.Interfaces.Teams;
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
                if (((MmoCommand<IEntityComponent>)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IEntityComponent>);
                }
                else if (((MmoCommand<IEntityComponent>)command).TellServer)
                {
                    _team.Tell(command);
                }
            }
            if (command is IEntityControllerCommand)
            {
                if (((MmoCommand<IEntityController>)command).TellClient)
                {
                    _client.SendCommand(command as MmoCommand<IEntityController>);
                }
                else if (((MmoCommand<IEntityController>)command).TellServer)
                {
                    _team.Tell(command);
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
