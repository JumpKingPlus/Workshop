using JumpKing.PauseMenu.BT.Actions;
using JumpKingMultiplayer.Menu;

namespace JumpKingMultiplayer.Menu.DisplayOptions
{
    public class ToggleProximityPlayers : ITextToggle
    {
        public ToggleProximityPlayers() 
            : base(ModEntry.Preferences.IsProximityPlayersEnabled)
        {
        }

        protected override string GetName() => "Display Proximity Players";

        protected override void OnToggle()
        {
            ModEntry.Preferences.IsProximityPlayersEnabled = !ModEntry.Preferences.IsProximityPlayersEnabled;
        }
    }
}
