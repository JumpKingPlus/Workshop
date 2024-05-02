using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates.Menu
{
    public class ToggleSavestates : ITextToggle
    {
        public ToggleSavestates() : base(JumpKingSaveStates.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "SaveStates";

        protected override void OnToggle()
        {
            JumpKingSaveStates.Preferences.IsEnabled = !JumpKingSaveStates.Preferences.IsEnabled;
        }
    }
}
