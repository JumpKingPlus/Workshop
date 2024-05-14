using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingTAS
{
    public class ToggleTAS : ITextToggle
    {
        public ToggleTAS() : base(ModEntry.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "TAS";

        protected override void OnToggle()
        {
            ModEntry.Preferences.IsEnabled = !ModEntry.Preferences.IsEnabled;
        }
    }
}
