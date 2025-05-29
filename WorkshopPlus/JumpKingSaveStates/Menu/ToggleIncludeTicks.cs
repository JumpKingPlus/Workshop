using JumpKing.PauseMenu.BT.Actions;

namespace JumpKingSaveStates.Menu
{
    public class ToggleIncludeTicks : ITextToggle
    {
        public ToggleIncludeTicks() : base(JumpKingSaveStates.Preferences.IncludeTicks)
        {
        }

        // Don't know if "ticks" or "time" is better. I think it's easier to understand
        // for users to just say "time" although "ticks" is technically correcter.
        protected override string GetName() => "Include time";

        protected override void OnToggle()
        {
            JumpKingSaveStates.Preferences.IncludeTicks = !JumpKingSaveStates.Preferences.IncludeTicks;
        }
    }
}
