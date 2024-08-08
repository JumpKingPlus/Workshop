using JumpKing;
using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingDiscordRPC.Menu
{
    public class PresetOption : IOptions
    {
        public PresetOption() 
            : base(Enum.GetNames(typeof(ERPCPresets)).Length, (int)ERPCPresets.BabeAndLocation, EdgeMode.Clamp)
        {
            CurrentOption = (int)ModEntry.Preferences.Preset;
        }

        protected override bool CanChange() => ModEntry.Preferences.IsEnabled;

        protected override string CurrentOptionName() => ((ERPCPresets)CurrentOption).ToString();

        protected override void OnOptionChange(int option)
        {
            ModEntry.Preferences.Preset = (ERPCPresets)option;
        }
    }
}
