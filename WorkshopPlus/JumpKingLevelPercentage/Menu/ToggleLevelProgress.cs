using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLevelPercentage.Menu
{
    public class ToggleLevelProgress : ITextToggle
    {
        public ToggleLevelProgress() : base(JumpKingLevelPercentage.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Display Level Progression";

        protected override void OnToggle()
        {
            JumpKingLevelPercentage.Preferences.IsEnabled = !JumpKingLevelPercentage.Preferences.IsEnabled;
        }
    }
}
