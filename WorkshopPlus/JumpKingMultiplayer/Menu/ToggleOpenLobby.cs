using JumpKing.PauseMenu.BT.Actions;
using JumpKingMultiplayer.Models;
using Steamworks;

namespace JumpKingMultiplayer.Menu
{
    public class ToggleOpenLobby : ITextToggle
    {
        public ToggleOpenLobby() : base(ModEntry.Preferences.LobbySettings.OpenToJoin)
        {
        }

        protected override bool CanChange()
        {
            return MultiplayerManager.IsOnline() && MultiplayerManager.instance.AmILobbyOwner;
        }

        protected override string GetName()
        {
            if (MultiplayerManager.IsOnline() && !MultiplayerManager.instance.AmILobbyOwner)
            {
                return "Open lobby (not lobby owner)";
            }
            return "Open Lobby";
        }

        protected override void OnToggle()
        {
            // struct moment
            var lobby = ModEntry.Preferences.LobbySettings;
            lobby.OpenToJoin = !lobby.OpenToJoin;
            ModEntry.Preferences.LobbySettings = lobby;

            if (lobby.OpenToJoin)
            {
                SteamMatchmaking.SetLobbyType(MultiplayerManager.instance.LobbyId.Value, ELobbyType.k_ELobbyTypePublic);
            }
            else
            {
                SteamMatchmaking.SetLobbyType(MultiplayerManager.instance.LobbyId.Value, ELobbyType.k_ELobbyTypePrivate);
            }
        }
    }
}
