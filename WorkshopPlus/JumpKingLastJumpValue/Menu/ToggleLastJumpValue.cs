using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLastJumpValue.Menu
{
    public class ToggleLastJumpValue : ITextToggle
    {
        public ToggleLastJumpValue() : base(JumpKingLastJumpValue.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Display Jump%";

        protected override void OnToggle()
        {
            JumpKingLastJumpValue.Preferences.IsEnabled = !JumpKingLastJumpValue.Preferences.IsEnabled;
        }
    }
}
