using BehaviorTree;
using EntityComponent;
using EntityComponent.BT;

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
