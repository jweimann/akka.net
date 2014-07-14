using Akka.Actor;
using RTS.Commands;
using RTS.Commands.Interfaces;
using RTS.Core.Constants;
using RTS.Entities.Controllers;
using RTS.Entities.Interfaces.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Movement
{
    public class AOI : UntypedActor
    {
        private IEntityController _entityController;
        

        protected override void PreStart()
        {
            _entityController = new AOIEntityController();
            Context.System.Scheduler.Schedule(TimeSpan.FromMilliseconds(Constants.AOI_UPDATE_INTERVAL), TimeSpan.FromMilliseconds(Constants.AOI_UPDATE_INTERVAL), () => Update());
        }

        private void Update()
        {
            _entityController.Update();
        }
        protected override void OnReceive(object message)
        {
            IEntityControllerCommand command = message as IEntityControllerCommand;

            if (command == null)
            {
                Console.WriteLine("ERROR " + message.ToString());
                return;
            }

            command.Execute(_entityController);
        }

    }
}
