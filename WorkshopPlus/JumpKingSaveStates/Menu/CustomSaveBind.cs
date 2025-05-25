using BehaviorTree;
using EntityComponent;
using EntityComponent.BT;

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
