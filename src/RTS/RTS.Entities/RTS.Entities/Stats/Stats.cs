using RTS.Commands;
using RTS.Commands.Interfaces;
using RTS.Core.Enums;
using RTS.DataStructures;
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
    public class Stats : IStats
    {
        private bool _dirty;
        private IEntity _entity;
        private Dictionary<StatId, int> _stats = new Dictionary<StatId, int>();
        private Dictionary<StatId, int> _statsMax = new Dictionary<StatId, int>();
        public int HP { get; private set; }
        
        public Stats(UnitDefinition unitDefinition)
        {
            HP = unitDefinition.StartingHP;
            _stats.Add(StatId.HP, HP);
            _stats.Add(StatId.Mana, 45);

            _statsMax.Add(StatId.HP, HP);
            _statsMax.Add(StatId.Mana, 45);

            _dirty = true;
        }
        public void HandleMessage(object message)
        {
            IMmoCommand<IStats> command = message as IMmoCommand<IStats>;
            if (command != null)
            {
                if (command.CanExecute(this))
                {
                    command.Execute(this);
                }
                //DamageEntityCommand damageEntityCommand = command as DamageEntityCommand;
                //if (damageEntityCommand != null)
                //{
                //    if (damageEntityCommand.EntityId == _entity.Id)
                //    {
                //        if (command.CanExecute(this))
                //        {
                //            command.Execute(this);
                //        }
                //    }
                //}
            }
        }

        public void Tick(double deltaTime)
        {
            if (_dirty)
            {
                _dirty = false;
                _entity.MessageTeam(new UpdateStatsCommand() { StatIds = new StatId[] { StatId.HP }, Values = new int[] { this.GetStat(StatId.HP) }, Max = new int[] { this.GetStatMax(StatId.HP) }, EntityId = _entity.Id });

                //AreaOfInterest.Tell(new UpdateStatsCommand() { StatIds = new StatId[] { StatId.HP }, Values = new int[] { this.HP }, EntityId = _entity.Id });
            }
        }

        private int GetStatMax(StatId statId)
        {
            return _statsMax[statId];
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


        public void SetStat(StatId statId, int value, int max)
        {
            if (_stats.ContainsKey(statId) == false)
                return;

            _stats[statId] = value;
            _statsMax[statId] = max;

            CheckHP();

            _dirty = true;
        }

        private void CheckHP()
        {
            if (GetStat(StatId.HP) <= 0)
            {
                _entity.Destroy();
            }
        }

        //public UpdateStatsCommand GetUpdateStatsCommand()
        //{
        //    UpdateStatsCommand command = new UpdateStatsCommand() { EntityId = this._entity.Id, StatIds = _stats.Keys.ToArray(), Values = _stats.Values.ToArray() };
        //    return command;
        //}

        public void MessageComponents(object message)
        {
            //throw new NotImplementedException();
        }

        public void SetEntity(IEntity entity)
        {
            _entity = entity;
        }

        public void PreStart()
        {
            //throw new NotImplementedException();
        }


        public int GetStat(StatId statId)
        {
            if (_stats.ContainsKey(statId) == false)
                return 0;

            return _stats[statId];
        }
    }
}
