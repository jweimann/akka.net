using Akka.Actor;
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
using RTS.Entities.Behaviors;
using RTS.Entities.Units;
using BehaviorTreeLibrary;
using RTS.ContentRepository;
using JW.Behavior;

namespace RTS.Entities.Buildings
{
    public class Building : IBuilding
    {
        private const float BUILD_RANGE = 10.25f;

        private IEntity _entity;
        private Dictionary<DateTime, Tuple<UnitType, Vector3?>> _buildQueue;
        private UnitDefinitionRepository _repository;

        public List<BehaviorTreeLibrary.Behavior> Behaviors = new List<BehaviorTreeLibrary.Behavior>();

        public Building()
        {
            _buildQueue = new Dictionary<DateTime, Tuple<UnitType, Vector3?>>();
            _repository = new UnitDefinitionRepository();
        }

        private void BuildEntityAtLocation(UnitType unitType, Vector3? position)
        {
            var vehicle = _entity.Components.FirstOrDefault(t => t is Vehicle) as Vehicle;

            var moveBehavior = new BHMoveToLocation(_entity.Brain as Brain, (Vector3)position, BUILD_RANGE);
            var buildBehavior = new BHBuildAtLocation(_entity.Brain as Brain, (Vector3)position, unitType);
            moveBehavior.Then = buildBehavior;

            _entity.Brain.AddBehavior(moveBehavior);

            /*
            var buildAtLocationBehavior = new BuildAtLocationBehavior(vehicle, (Vector3)position, unitDefinition.UnitType, _entity.GetActorContext());
            buildAtLocationBehavior.Add<Behavior>().Update = () => ClearBehaviors();
            //buildAtLocationBehavior.Terminate = x => { ClearBehaviors(); };
            Behaviors.Add(buildAtLocationBehavior);

            vehicle.MoveToPosition((Vector3)position, BUILD_RANGE);
             */
        }

        public void BuildEntity(UnitType unitType, Vector3? position)
        {
            
            UnitDefinition unitDefinition = _repository.Get(unitType);

            if (position != null)
            {
                BuildEntityAtLocation(unitType, position);
            }
            else
            {
                StartBuildingEntity(unitDefinition, position);
            }
        }

        private void StartBuildingEntity(UnitDefinition unitDefinition, Vector3? position)
        {
            _buildQueue.Add(DateTime.Now + TimeSpan.FromSeconds(unitDefinition.BuildTime), new Tuple<UnitType, Vector3?>(unitDefinition.UnitType, position));

        }

        private BehaviorTreeLibrary.Status ClearBehaviors()
        {
            Behaviors.Clear();
            return BehaviorTreeLibrary.Status.BhSuccess;
        }

        // Not used on the server right now but is used on client.  Take this off the shared interface if this isn't going to change.
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

        private List<UnitType> BuildableUnitTypes
        {
            get
            {
                return _repository.Get(_entity.GetSpawnEntityData().UnitType).CanBuild;
            }
        }

        public bool CanBuild(UnitDefinition unitDefinition)
        {
            if (BuildableUnitTypes == null)
                return false;

            foreach (var buildable in BuildableUnitTypes)
            {
                if (buildable == unitDefinition.UnitType)
                    return true;
            }
            return false;
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
            for (int i = 0; i < this.Behaviors.Count; i++)
            {
                this.Behaviors[i].Tick(deltaTime);
            }

            List<DateTime> keysToRemove = new List<DateTime>();
            foreach (var buildCompletionTime in _buildQueue.Keys)
            {
                var buildInfo = _buildQueue[buildCompletionTime];
                if (buildCompletionTime <= DateTime.Now)
                {
                    SendBuildEntityToTeam(buildInfo);
                    keysToRemove.Add(buildCompletionTime);
                }
                else
                {
                    SendUpdateBuildProgressToClient(buildCompletionTime, buildInfo);
                }
            }
            foreach (var keyToRemove in keysToRemove)
            {
                _buildQueue.Remove(keyToRemove);
            }
        }

        private void SendUpdateBuildProgressToClient(DateTime buildCompletionTime, Tuple<UnitType, Vector3?> buildInfo)
        {
            //var unitType = buildInfo.Item1;

            var definition = _repository.Get(buildInfo.Item1);

            DateTime startTime = buildCompletionTime - TimeSpan.FromSeconds(definition.BuildTime); // add this to the tuple? make a datastructure?
            DateTime now = DateTime.Now;

            byte percentageComplete = GetValue(now, startTime, buildCompletionTime, 0, 100);
            var cmd = new UpdateBuildProgressCommand(this._entity.Id, buildInfo.Item1, percentageComplete);
            _entity.MessagePlayer(cmd);
        }

        byte Map(long value, long minIn, long maxIn, byte minOut, byte maxOut)
        {
            double pct = (double)maxIn / (double)minIn;
            var Y = (value - minIn) / (maxIn - minIn) * (maxOut - minOut) + minOut;
            return (byte)Y;
        }

        public double GetValue(int value, int min, int max, int originalMin, int originalMax)
        {
            return min + (double)(value - originalMin) * (max - min) / (originalMax - originalMin);
        }

        public byte GetValue(DateTime value, DateTime min, DateTime max, int originalMin, int originalMax)
        {
            var valBased = value.Ticks - min.Ticks;
            var maxBased = max.Ticks - min.Ticks;

            double pct = (double)valBased / (double)maxBased;
            var outVal = (double)originalMax * pct;
            return (byte)outVal;

            //return min + (double)(value - originalMin) * (max - min) / (originalMax - originalMin);
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


        public void UpdateBuildProgress(UnitType unitType, byte percentComplete)
        {
            throw new NotImplementedException("Not yet handled on the server");
        }
    }
}
