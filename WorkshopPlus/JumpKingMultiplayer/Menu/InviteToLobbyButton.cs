using JumpKingMultiplayer.Models;
using Steamworks;
using JumpKingMultiplayer.Helpers;

namespace JumpKingMultiplayer.Menu
{
    public class InviteToLobbyButton : TextButtonToggle
    {
        public InviteToLobbyButton() : base(ModEntry.Preferences.LobbySettings.OpenToJoin)
        {
        }

        protected override bool CanChange()
        {
            return MultiplayerManager.IsOnline();
        }

        protected override string GetName() => "Invite friends";

        protected override void OnToggle()
        {
            SteamFriends.ActivateGameOverlayInviteDialog(MultiplayerManager.instance.LobbyId.Value);
        }
    }
}
