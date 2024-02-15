using BehaviorTree;
using EntityComponent;
using EntityComponent.BT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates.Menu
{
    internal class CustomBindDefault : EntityBTNode
    {
        public CustomBindDefault(Entity p_entity) : base(p_entity)
        {
        }

        protected override BTresult MyRun(TickData p_data)
        {
            JumpKingSaveStates.Preferences.KeyBindings = new Preferences().KeyBindings;
            return BTresult.Success;
        }
    }
}
