﻿using Akka.Actor;
using RTS.Commands.Buildings;
using RTS.Core.Structs;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Teams;
using RTS.Entities.Interfaces.UnitTypes;
using RTS.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTS.Core.Enums;

namespace RTS.Entities.Buildings
{
    public class Building : IBuilding
    {
        private IEntity _entity;
        private Dictionary<DateTime, Tuple<UnitType, Vector3?>> _buildQueue;

        public Building()
        {
            _buildQueue = new Dictionary<DateTime, Tuple<UnitType, Vector3?>>();
        }
        public void BuildEntity(UnitDefinition unitDefinition, Vector3? position)
        {
            _buildQueue.Add(DateTime.Now + TimeSpan.FromSeconds(unitDefinition.BuildTime), new Tuple<UnitType, Vector3?>(unitDefinition.UnitType, position));
        }

        public List<UnitDefinition> BuildableEntities
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool CanBuild(UnitDefinition unitDefinition)
        {
            return true;
        }

        public void MessageComponents(object message)
        {
            throw new NotImplementedException();
        }

        public void SetEntity(IEntity entity)
        {
            this._entity = entity;
        }

        public void HandleMessage(object message)
        {
            if (message is BuildEntityCommand)
            {
                if ((message as BuildEntityCommand).CanExecute((IBuilding)this))
                {
                    (message as BuildEntityCommand).Execute(this);
                }
            }
        }

        public void Tick(double deltaTime)
        {
            List<DateTime> keysToRemove = new List<DateTime>();
            foreach (var key in _buildQueue.Keys)
            {
                if (key <= DateTime.Now)
                {
                    SendBuildEntityToTeam(_buildQueue[key]);
                    keysToRemove.Add(key);
                }
            }
            foreach (var keyToRemove in keysToRemove)
            {
                _buildQueue.Remove(keyToRemove);
            }
        }

        private void SendBuildEntityToTeam(Tuple<UnitType, Vector3?> buildInfo)
        {
            ActorRef entityActor = _entity.GetActorRef() as ActorRef;
            var command = new FinishBuildEntityCommand()
            {
                UnitType = buildInfo.Item1,
                Position = _entity.Position + new Vector3(10, 0, 0)    
            };
            if (buildInfo.Item2 != null)
            {
                command.Position = (Vector3)buildInfo.Item2;
            }
            entityActor.Tell(command);
            
        }

        public void PreStart()
        {
            //throw new NotImplementedException();
        }
    }
}
