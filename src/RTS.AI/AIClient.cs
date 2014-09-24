using Akka.Actor;
using RTS.Commands.Interfaces;
using RTS.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.AI
{
    /// <summary>
    /// THIS WILL NOT WORK DISTRIBUTED.  NEED TO CHANGE THE AICLIENT TO SOMETHING THAT CAN EXIST ON MORE THAN 1 SYSTEM AT ONCE
    /// Add an AICLIENTCONTROLLER for the player side and send a message?  Add an actor?
    /// </summary>
    public class AIClient : IPlayerConnection
    {
        private AIBot _bot;
        public AIClient()
        {
        }

        public void SetBot(AIBot bot) { _bot = bot; }

        public void SendCommand<T>(IMmoCommand<T> command)
        {
            _bot.HandleCommand(command);
        }



        public event CommandRecievedEventHandler CommandRecieved;

        internal void SendCommandToPlayer(IMmoCommand cmd)
        {
            if (CommandRecieved != null)
                CommandRecieved(this, cmd);
        }
    }
}
