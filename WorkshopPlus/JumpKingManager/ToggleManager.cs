using JumpKing.PauseMenu.BT.Actions;

namespace JumpKingManager
{
    public class ToggleManager : ITextToggle
    {
        public ToggleManager() : base(ModEntry.Preferences.IsEnabled)
        {
        }

        protected override string GetName() => "Manager";

        protected override void OnToggle()
        {
            ModEntry.Preferences.IsEnabled = !ModEntry.Preferences.IsEnabled;
        }
    }
}