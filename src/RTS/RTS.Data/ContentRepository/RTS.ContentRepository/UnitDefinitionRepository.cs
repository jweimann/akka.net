using RTS.Core.Enums;
using RTS.DataStructures;
using RTS.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.ContentRepository
{
    public class UnitDefinitionRepository : IRepository<UnitDefinition, UnitType>
    {
        Dictionary<UnitType, UnitDefinition> _unitDefinitions;

        public UnitDefinitionRepository()
        {
            _unitDefinitions = new Dictionary<UnitType, UnitDefinition>();
            _unitDefinitions.Add(UnitType.TruckDepot, new UnitDefinition() { BuildTime = 2, Name = "TruckDepot", StartingHP = 500, UnitType = UnitType.TruckDepot, SpawnLocationRequired = true });
            _unitDefinitions.Add(UnitType.Truck, new UnitDefinition() { BuildTime = 1, Name = "Truck", StartingHP = 100, UnitType = UnitType.Truck, SpawnLocationRequired = false });
            _unitDefinitions.Add(UnitType.StugIII, new UnitDefinition() { BuildTime = 3, Name = "StugIII", StartingHP = 250, UnitType = UnitType.StugIII, SpawnLocationRequired = false });
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
