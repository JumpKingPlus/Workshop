using JumpKing.PauseMenu.BT.Actions;
using System;
using JumpKingMultiplayer.Extensions;
using JumpKingMultiplayer.Menu;

namespace JumpKingMultiplayer.Menu.DisplayOptions
{
    public class GhostPlayerDisplayOption : IOptions
    {
        public GhostPlayerDisplayOption()
            : base(Enum.GetNames(typeof(EGhostPlayerDisplayType)).Length, (int)ModEntry.Preferences.GhostPlayerDisplayType, EdgeMode.Clamp)
        {
        }

        protected override bool CanChange() => true;

        protected override string CurrentOptionName()
        {
            return ((EGhostPlayerDisplayType)CurrentOption).GetDescription();
        }

        protected override void OnOptionChange(int option)
        {
            ModEntry.Preferences.GhostPlayerDisplayType = (EGhostPlayerDisplayType)option;
        }
    }
}
