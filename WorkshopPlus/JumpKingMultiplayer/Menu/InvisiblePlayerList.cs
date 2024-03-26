using BehaviorTree;
using JumpKing.PauseMenu;

namespace JumpKingMultiplayer.Menu
{
    internal class InvisiblePlayerList : PlayerList
    {
        private GuiFormat leader_format;

        public InvisiblePlayerList(GuiFormat leader_format, GuiFormat list)
            : base(list)
        {
            this.leader_format = leader_format;
        }

        protected override BTresult MyRun(TickData p_data)
        {
            throw new System.NotImplementedException();
        }
    }
}