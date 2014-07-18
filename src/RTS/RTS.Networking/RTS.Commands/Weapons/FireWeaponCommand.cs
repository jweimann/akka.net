using RTS.Commands.Interfaces;
using RTS.Entities.Interfaces.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS.Commands.Weapons
{
    [Serializable]
    public class FireWeaponCommand : MmoCommand<IWeapon>, IEntityComponentCommand
    {
        public long EntityId { get; set; } // Probably change this to an actual weaponID or something that specifies which weapon it is on the entity.
        public long TargetEntityId { get; set; }
        public override void Execute(IWeapon target)
        {
            target.FireWeapon(EntityId, TargetEntityId);
        }

        public override bool CanExecute(IWeapon target)
        {
            return true;
        }

        public override Core.Enums.CommandId CommandId
        {
            get { return Core.Enums.CommandId.FireWeapon; }
        }

        public override Core.Enums.Destination CommandDestination
        {
            get { return Core.Enums.Destination.Client; }
        }
    }
}
