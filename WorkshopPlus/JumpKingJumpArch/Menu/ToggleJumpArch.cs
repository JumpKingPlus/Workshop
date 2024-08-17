using JumpKing.PauseMenu.BT.Actions;

namespace JumpKingJumpArch.Menu
{
    public class ToggleJumpArch : ITextToggle
    {
        public ToggleJumpArch() : base(ModEntry.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Toggle Jump Preview";

        protected override void OnToggle()
        {
            ModEntry.Preferences.IsEnabled = !ModEntry.Preferences.IsEnabled;
        }
    }
}