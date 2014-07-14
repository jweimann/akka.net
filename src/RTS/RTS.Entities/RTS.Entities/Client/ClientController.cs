using RTS.Commands;
using RTS.Commands.Server;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Control;
using RTS.Entities.Requests;
using RTS.Networking.Helios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Client
{
    public class ClientController : PlayerComponent<IController>, IController
    {
        private RTSHeliosNetworkClient _client;

        public ClientController(RTSHeliosNetworkClient client)
        {
            _client = client;
            _client.CommandRecieved += CommandRecieved;
            //Console.WriteLine(String.Format("ClientController added to Connection: {0}  EntityId: {1}", _client.Connection.RemoteHost.ToString(), _entity.Id));
        }

        void CommandRecieved(object sender, Commands.MmoCommand command)
        {
            _player.HandleCommand(command);
        }
        //void CommandRecieved2(object sender, Commands.MmoCommand command)
        //{
        //    if (command is DamageEntityCommand)
        //    {
        //        var damageEntityCommand = command as DamageEntityCommand;
        //        damageEntityCommand.OriginatorEntityId = _entity.Id;
        //        if (damageEntityCommand.EntityId != _entity.Id)
        //        {
        //            AreaOfInterest.Tell(damageEntityCommand);
        //        }
        //        else
        //        {
        //            MessageComponents(command);
        //        }
        //    }
        //    else if (command is SpawnEntityCommand)
        //    {
        //        AreaOfInterest.Tell(command);
        //        return;
        //    }
        //    else
        //    {
        //        MessageComponents(command);
        //    }
        //}


        public override void HandleMessage(object message)
        {
            HandleRequest(message);
        }

        public void HandleRequest(object request)
        {

        }

        public void HandleRequest2(object request)
        {
            MoveRequest moveRequest = request as MoveRequest;
            if (moveRequest != null)
            {
                _client.SendCommand(new MoveCommand() { EntityId = moveRequest.EntityId, Velocity = moveRequest.Velocity, Position = moveRequest.Position });
            }

            BatchMoveRequest batchMoveRequest = request as BatchMoveRequest;
            if (batchMoveRequest != null)
            {
                _client.SendCommand(new BatchMoveCommand() { EntityId = batchMoveRequest.EntityId, Velocity = batchMoveRequest.Velocity, Position = batchMoveRequest.Position });
            }

            UpdateStatsCommand command = request as UpdateStatsCommand;
            if (command != null)
            {
                _client.SendCommand(command);
            }

            EntitiesInAreaListCommand entitiesCommand = request as EntitiesInAreaListCommand;
            if (entitiesCommand != null)
            {
                _client.SendCommand(entitiesCommand);
            }

            LeaveAreaOfInterestCommand leaveCommand = request as LeaveAreaOfInterestCommand;
            if (leaveCommand != null)
            {
                _client.SendCommand(leaveCommand);
            }

            AOIDetailsCommand aoiCommand = request as AOIDetailsCommand;
            if (aoiCommand != null)
            {
                _client.SendCommand(aoiCommand);
            }
        }

        public override void Update()
        {

        }

        public override void PreStart()
        {
        }
    }
}
