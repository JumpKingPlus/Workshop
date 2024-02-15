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
    public class CustomSaveBind : EntityBTNode
    {
        public CustomSaveBind(Entity p_entity) : base(p_entity)
        {
        }

        protected override BTresult MyRun(TickData p_data)
        {
            JumpKingSaveStates.Preferences.ForceUpdate();
            return BTresult.Success;
        }
    }
}
