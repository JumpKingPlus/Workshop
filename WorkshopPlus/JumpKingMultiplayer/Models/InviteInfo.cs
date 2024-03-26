using JumpKing.Util;
using JumpKing;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKingMultiplayer.Menu;
using BehaviorTree;
using JumpKing.PauseMenu.BT;
using JumpKing.PauseMenu;

namespace JumpKingMultiplayer.Models
{
    internal class InviteInfo
    {
        RunnableDisplayFrame invite_frame;
        float invite_alpha = 0.7f;
        GuiFormat format = new GuiFormat
        {
            all_padding = 12,
            all_margin = 15,
            element_margin = 5,
            anchor_bounds = new Rectangle(0, 0, JumpGame.GAME_RECT.Width, JumpGame.GAME_RECT.Height),
            anchor = new Vector2(1f, 0f),
        };

        public InviteInfo()
        {
            Initialize();
        }

        public void Initialize()
        {
            invite_frame = new RunnableDisplayFrame(format, BTresult.Running, Color.DarkGreen, invite_alpha);
            var text_format = format;
            text_format.anchor_bounds.Width = 160;

            if (id.HasValue)
            {
                invite_frame.AddChild(
                    TextInfo.CreateOneFittedInfo(
                        text_format,
                        CreateLabel(id.Value),
                        Color.White * invite_alpha,
                        Game1.instance.contentManager.font.MenuFontSmall
                    )
                );
            }

            invite_frame.AddChild(new TextInfo("[F1] Accept", Color.DarkGreen));
            invite_frame.AddChild(new TextInfo("[F2] Decline", Color.White));

            invite_frame.Initialize();
        }

        public CSteamID? id = null;
        public CSteamID? lobbyId = null;

        public string CreateLabel(Steamworks.CSteamID id)
        {
            var name = Steamworks.SteamFriends.GetFriendPersonaName(id);
            return $"{name} Invited you to their game.";
        }

        public void AddInvite(CSteamID userId, CSteamID lobbyId)
        {
            this.id = userId;
            this.lobbyId = lobbyId;
            Initialize();
        }

        public void Accept()
        {
            if (!id.HasValue) { return; }
            MultiplayerManager.instance.JoinLobby(lobbyId.Value);
            this.id = null;
            this.lobbyId = null;
        }

        public void Decline()
        {
            if (!id.HasValue) { return; }
            this.id = null;
            this.lobbyId = null;
        }

        public void Tick()
        {
            if (!id.HasValue) { return; }

            var keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Contains(Keys.F1))
            {
                Accept();
            }
            else if (keys.Contains(Keys.F2))
            {
                Decline();
            }
        }

        public void Run(TickData data)
        {
            invite_frame.Run(data);
        }

        public void Draw()
        {
            if (!id.HasValue) { return; }
            //var label = CreateLabel(id.Value);
            //TextHelper.DrawString(Game1.instance.contentManager.font.LocationFont, label, new Vector2(8f, 330f), new Color(255, 215, 0, 130), Vector2.Zero);
            //TextHelper.DrawString(Game1.instance.contentManager.font.LocationFont, "[F1] Accept   [F2] Decline", new Vector2(8f, 343f), new Color(255, 0, 0, 130), Vector2.Zero);
            invite_frame.Draw();
        }
    }
}
