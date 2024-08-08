using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingDiscordRPC.Menu
{
    public class ToggleDiscordRPC : ITextToggle
    {
        public ToggleDiscordRPC() : base(ModEntry.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Display Discord RPC";

        protected override void OnToggle()
        {
            var value = !ModEntry.Preferences.IsEnabled;
            ModEntry.Preferences.IsEnabled = value;

            if (value)
            {
                ModEntry.Client.Start();
            }
            else
            {
                ModEntry.Client.Stop();
            }
        }
    }
}
