using Akka.Actor;
using RTS.AI.Components;
using RTS.Commands;
using RTS.Commands.Buildings;
using RTS.Commands.Interfaces;
using RTS.Commands.Server;
using RTS.Commands.Team;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Interfaces.Player;
using RTS.Entities.Interfaces.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.AI
{
    public class AIBot : UntypedActor, IPlayer
    {
        /// <summary>
        /// THIS WILL NOT WORK DISTRIBUTED.  NEED TO CHANGE THE AICLIENT TO SOMETHING THAT CAN EXIST ON MORE THAN 1 SYSTEM AT ONCE
        /// </summary>
        
        private AIClient _client;
        private AIEntityController _entityController;

        public long Team { get { return _teamId; } }
        
        public AIBot(AIClient client)
        {
            _client = client;
            _client.SetBot(this);
            
            _entityController = new AIEntityController(this);
        }
        
        public void Update()
        {
            TryBuild();
            _entityController.Update();
        }
        public void SendCommandToPlayer(IMmoCommand command)
        {
            _client.SendCommandToPlayer(command);
        }
        
        private void TryBuild()
        {
            if (_money > 300)
            {
                var building = PickBuilding();
                if (building != null)
                {
                    BuildEntityCommand cmd = new BuildEntityCommand() { BuildingEntityId = building.Id, UnitTypeId = Core.Enums.UnitType.Truck };
                    _client.SendCommandToPlayer(cmd);
                }
            }
        }

        private IEntity PickBuilding()
        {
            return _entityController.RandomBuilding();

            //IEntity building = ((Player.Player)_player).AskTeam("PickBuilding") as IEntity;
            //return building;
        }

        protected override void OnReceive(object message)
        {
            throw new NotImplementedException();
        }

        internal void HandleCommand<T>(IMmoCommand<T> command)
        {
            CommandMatch.Match(command)
                .WithClient<IMmoCommand<IPlayer>>(() => ((IMmoCommand<IPlayer>)command).Execute(this))
                .WithClient<IMmoCommand<IEntityController>>(() => ((IMmoCommand<IEntityController>)command).Execute(_entityController))
                .WithClient<ITeam>(() => HandleTeamCommand(command));
            
        }

        private void HandleTeamCommand(IMmoCommand command)
        {
            if (command is DestroyEntityCommand)
            {
                var cmd = (DestroyEntityCommand)command;
                _entityController.RemoveEntity(cmd.EntityId);
            }
        }

        protected override void PreStart()
        {
            base.PreStart();
            Context.System.Scheduler.Schedule(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000), () => Update());
        }

    

        /// <summary>
        /// TODO: Move this all to components.
        /// </summary>
        #region IPlayer Implementation

        private long _teamId;
        private int _money;

        public void HandleCommand(object command)
        {
            throw new NotImplementedException();
        }

        public void MessageComponents(object message)
        {
            throw new NotImplementedException();
        }

        public void SetTeam(object team) {  }

        public void SetTeamId(long teamId) { _teamId = teamId; }

        public void SetMoney(int money) { _money = money; }

        public void HandlePlayerDisconnected(object PlayerActor)
        {
            throw new NotImplementedException();
        }

        public int GetMoney() { return _money; }
        
        #endregion

        
    }
}
