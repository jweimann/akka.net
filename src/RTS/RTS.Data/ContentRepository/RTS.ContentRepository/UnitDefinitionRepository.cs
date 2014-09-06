using RTS.Core.Enums;
using RTS.DataStructures;
using RTS.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.ContentRepository
{
    public class UnitDefinitionRepository : IRepository<UnitDefinition, UnitType>
    {
        Dictionary<UnitType, UnitDefinition> _unitDefinitions;

        public UnitDefinitionRepository()
        {
            _unitDefinitions = new Dictionary<UnitType, UnitDefinition>();
            _unitDefinitions.Add(UnitType.BaseStation, new UnitDefinition() { BuildTime = 2, Name = "Base Station", Cost = 1, StartingHP = 1000, UnitType = UnitType.BaseStation, SpawnLocationRequired = true, ResourceAmount = 10, ResourceInterval = 3.0, CanBuild = new List<UnitType>() { UnitType.Engineer } });
            _unitDefinitions.Add(UnitType.TruckDepot, new UnitDefinition() { BuildTime = 2, Name = "TruckDepot", Cost = 250, StartingHP = 500, UnitType = UnitType.TruckDepot, SpawnLocationRequired = true, ResourceAmount = 10, ResourceInterval = 3.0, CanBuild = new List<UnitType>() { UnitType.Truck, UnitType.StugIII, UnitType.M730A1_L } });
            _unitDefinitions.Add(UnitType.Truck, new UnitDefinition() { BuildTime = 1, Name = "Truck", Cost = 100, StartingHP = 1000, UnitType = UnitType.Truck, SpawnLocationRequired = false });
            _unitDefinitions.Add(UnitType.StugIII, new UnitDefinition() { BuildTime = 3, Name = "StugIII", Cost = 150, StartingHP = 250, UnitType = UnitType.StugIII, SpawnLocationRequired = false });
            _unitDefinitions.Add(UnitType.M730A1_L, new UnitDefinition() { BuildTime = 3, Name = "M730A1_L", Cost = 250, StartingHP = 150, UnitType = UnitType.M730A1_L, SpawnLocationRequired = false });
            _unitDefinitions.Add(UnitType.Harvester, new UnitDefinition() { BuildTime = 3, Name = "Harvester", Cost = 125, StartingHP = 400, UnitType = UnitType.Harvester, SpawnLocationRequired = true, ResourceAmount = 5, ResourceInterval = 2.0 });
            _unitDefinitions.Add(UnitType.Engineer, new UnitDefinition() { BuildTime = 3, Name = "Engineer", Cost = 300, StartingHP = 400, UnitType = UnitType.Engineer, SpawnLocationRequired = false, CanBuild = new List<UnitType>() { UnitType.Harvester, UnitType.TruckDepot } });
        }
        public UnitDefinition Get(UnitType Id)
        {
            return _unitDefinitions[Id];
        }


        public UnitDefinition Get(string Name)
        {
            return _unitDefinitions.FirstOrDefault(t => t.Value.Name == Name).Value;
        }
    }
}
