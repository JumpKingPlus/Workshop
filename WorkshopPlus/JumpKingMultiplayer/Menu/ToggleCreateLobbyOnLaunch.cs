using JumpKing.PauseMenu.BT.Actions;

namespace JumpKingMultiplayer.Menu
{
    public class ToggleCreateLobbyOnLaunch : ITextToggle
    {
        public ToggleCreateLobbyOnLaunch() 
            : base(ModEntry.Preferences.LobbySettings.CreateLobbyOnLaunch)
        {
        }

        protected override string GetName() => "Create lobby on launch";

        protected override void OnToggle()
        {
            // struct moment
            var lobby = ModEntry.Preferences.LobbySettings;
            lobby.CreateLobbyOnLaunch = !lobby.CreateLobbyOnLaunch;
            ModEntry.Preferences.LobbySettings = lobby;
        }
    }
}
