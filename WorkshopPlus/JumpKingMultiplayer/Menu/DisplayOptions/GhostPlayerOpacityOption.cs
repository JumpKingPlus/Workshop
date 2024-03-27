using JumpKing.PauseMenu.BT.Actions;
using JumpKingMultiplayer.Menu;

namespace JumpKingMultiplayer.Menu.DisplayOptions
{
    public class GhostPlayerOpacityOption : IOptions
    {
        public GhostPlayerOpacityOption() 
            : base(11, (int)(ModEntry.Preferences.GhostPlayerOpacity * 10), EdgeMode.Clamp)
        {
        }

        protected override bool CanChange() => true;

        protected override string CurrentOptionName()
        {
            return $"Ghost opacity: {CurrentOption * 10}%";
        }

        protected override void OnOptionChange(int option)
        {
            ModEntry.Preferences.GhostPlayerOpacity = option * 0.1f;
        }
    }
}
