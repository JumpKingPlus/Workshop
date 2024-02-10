using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLastJumpValue.Menu
{
    public class LastJumpValueOption : IOptions
    {
        public LastJumpValueOption() 
            : base(Enum.GetNames(typeof(ELastJumpDisplayType)).Length, (int)ELastJumpDisplayType.Percentage, EdgeMode.Wrap)
        {
            CurrentOption = (int)JumpKingLastJumpValue.DisplayType;
        }

        protected override bool CanChange() => JumpKingLastJumpValue.IsEnabled;

        protected override string CurrentOptionName() => ((ELastJumpDisplayType)CurrentOption).ToString();

        protected override void OnOptionChange(int option)
        {
            JumpKingLastJumpValue.DisplayType = (ELastJumpDisplayType)option;
        }
    }
}
