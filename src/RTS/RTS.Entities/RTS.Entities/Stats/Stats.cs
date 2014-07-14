using RTS.Commands;
using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.Entities.Interfaces;
using RTS.Entities.Interfaces.Stats;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTS.Entities.Stats
{
    public class Stats : EntityComponent<IStats>, IStats
    {
        private bool _dirty;
        private Dictionary<StatId, int> _stats = new Dictionary<StatId, int>();
        public int HP { get; private set; }
        
        public Stats()
        {
            HP = 127;
            _stats.Add(StatId.HP, HP);
            _stats.Add(StatId.Mana, 45);
        }
        public override void HandleMessage(object message)
        {
            IMmoCommand<IStats> command = message as IMmoCommand<IStats>;
            if (command != null)
            {
                DamageEntityCommand damageEntityCommand = command as DamageEntityCommand;
                if (damageEntityCommand != null)
                {
                    if (damageEntityCommand.EntityId == _entity.Id)
                    {
                        if (command.CanExecute(this))
                        {
                            command.Execute(this);
                        }
                    }
                }
            }
        }

        public override void Update(double deltaTime)
        {
            if (_dirty)
            {
                _dirty = false;
                AreaOfInterest.Tell(new UpdateStatsCommand() { StatIds = new StatId[] { StatId.HP }, Values = new int[] { this.HP }, EntityId = _entity.Id });
            }
        }

        public void TakeDamage(int damage)
        {
            if (damage > 0)
            {
                this.HP -= damage;
                _dirty = true;
                Console.WriteLine(String.Format("Entity Took Damage  EntityId: {0}  Damage: {1}  HP: {2}", _entity.Id, damage, this.HP));
            }
        }


        public void SetStat(StatId statId, int value)
        {
            throw new NotImplementedException(); // Only used on client atm.. will update a dictionary of stats eventually.
        }

        public UpdateStatsCommand GetUpdateStatsCommand()
        {
            UpdateStatsCommand command = new UpdateStatsCommand() { EntityId = this._entity.Id, StatIds = _stats.Keys.ToArray(), Values = _stats.Values.ToArray() };
            return command;
        }
    }
}
