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
using JumpKingMultiplayer.Menu.DisplayFrames;
using JumpKingMultiplayer.Helpers;
using JumpKingMultiplayer.Menu.Lists;
using JumpKingMultiplayer.Models.Infos;
using JumpKingMultiplayer.Models;

namespace JumpKingMultiplayer.Models.Infos
{
    internal class InviteInfo : Info
    {
        public CSteamID? id = null;
        public CSteamID? lobbyId = null;

        public InviteInfo() : base() { }

        protected override void Initialize()
        {
            frame = new RunnableDisplayFrame(format, BTresult.Running, Color.DarkGreen, alpha);
            var text_format = format;
            text_format.anchor_bounds.Width = 160;

            if (id.HasValue)
            {
                frame.AddChild(
                    TextInfo.CreateOneFittedInfo(
                        text_format,
                        CreateLabel(id.Value),
                        Color.White * alpha,
                        Game1.instance.contentManager.font.MenuFontSmall
                    )
                );
            }

            frame.AddChild(new TextInfo("[F1] Accept", Color.DarkGreen));
            frame.AddChild(new TextInfo("[F2] Decline", Color.White));

            frame.Initialize();
        }

        public string CreateLabel(Steamworks.CSteamID id)
        {
            var name = Steamworks.SteamFriends.GetFriendPersonaName(id);
            if (!Game1.instance.contentManager.font.MenuFontSmall.CanUseFontProperly(name))
            {
                name = "Jumper";
            }
            return $"{name} invited you to their game.";
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

        public override void Run(TickData data)
        {
            frame.Run(data);
        }

        public override void Draw()
        {
            if (!id.HasValue) { return; }
            //var label = CreateLabel(id.Value);
            //TextHelper.DrawString(Game1.instance.contentManager.font.LocationFont, label, new Vector2(8f, 330f), new Color(255, 215, 0, 130), Vector2.Zero);
            //TextHelper.DrawString(Game1.instance.contentManager.font.LocationFont, "[F1] Accept   [F2] Decline", new Vector2(8f, 343f), new Color(255, 0, 0, 130), Vector2.Zero);
            frame.Draw();
        }
    }
}
