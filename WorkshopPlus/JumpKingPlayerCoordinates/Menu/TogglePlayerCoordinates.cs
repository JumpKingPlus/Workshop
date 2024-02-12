using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlayerCoordinates.Menu
{
    public class TogglePlayerCoordinates : ITextToggle
    {
        public TogglePlayerCoordinates() : base(JumpKingPlayerCoordinates.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Display Coordinates";

        protected override void OnToggle()
        {
            JumpKingPlayerCoordinates.Preferences.IsEnabled = !JumpKingPlayerCoordinates.Preferences.IsEnabled;
        }
    }
}
