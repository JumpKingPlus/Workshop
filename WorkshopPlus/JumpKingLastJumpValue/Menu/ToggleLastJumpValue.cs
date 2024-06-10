using JumpKing.PauseMenu.BT.Actions;

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
