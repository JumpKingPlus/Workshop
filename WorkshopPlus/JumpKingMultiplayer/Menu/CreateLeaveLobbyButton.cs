using BehaviorTree;
using JumpKing;
using JumpKing.PauseMenu.BT;
using JumpKing.PauseMenu.BT.Actions;
using JumpKing.Util;
using JumpKingMultiplayer.Models;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer.Menu
{
    public abstract class TextButtonToggle : ITextToggle
    {
        protected TextButtonToggle(bool p_start_value) : base(p_start_value)
        {
        }

        public override void Draw(int x, int y, bool selected)
        {
            TextHelper.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                GetName(),
                new Vector2(x, y),
                CanChange() ? Color.White : Color.Gray,
                new Vector2(0f, 0f));
        }

        public override Point GetSize()
        {
            return Game1.instance.contentManager.font.MenuFont.MeasureString(GetName()).ToPoint();
        }
    }

    public class CreateLeaveLobbyButton : TextButtonToggle
    {
        public CreateLeaveLobbyButton() : base(MultiplayerManager.IsOnline())
        {
        }

        protected override string GetName()
        {
            return this.toggle ? "Leave Lobby" : "Create Lobby";
        }

        protected override void OnToggle()
        {
            MultiplayerManager.ToggleOnline();
        }
    }

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

    public class ToggleOpenLobby : ITextToggle
    {
        public ToggleOpenLobby() : base(ModEntry.Preferences.LobbySettings.OpenToJoin)
        {
        }

        protected override bool CanChange()
        {
            return MultiplayerManager.IsOnline() && SteamMatchmaking.GetLobbyOwner(MultiplayerManager.instance.LobbyId.Value) == SteamUser.GetSteamID();
        }

        protected override string GetName()
        {
            if (MultiplayerManager.IsOnline() &&
                SteamMatchmaking.GetLobbyOwner(MultiplayerManager.instance.LobbyId.Value) != SteamUser.GetSteamID())
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
