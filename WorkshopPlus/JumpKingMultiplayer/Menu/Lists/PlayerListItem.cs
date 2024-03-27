using BehaviorTree;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;
using JumpKing.Player;
using JumpKing.Util;
using JumpKingMultiplayer.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKingMultiplayer.Extensions;
using JumpKingMultiplayer.Menu;

namespace JumpKingMultiplayer.Menu.Lists
{
    internal class PlayerListItem : TextInfo
    {
        internal IGhostPlayerData playerData;
        Sprite Sprite = Game1.instance.contentManager.Pixel.sprite;
        public PlayerListItem(IGhostPlayerData playerData)
            : base(playerData.SteamName, Color.White)
        {
            Sprite.SetAlpha(0.25f);
            this.playerData = playerData;

            InitializeItems();
        }

        BodyComp bodyComp;
        
        public PlayerListItem(string steamName, CSteamID steamId, BodyComp bodyComp)
            : base(steamName, Color.White)
        {
            Sprite.SetAlpha(0.25f);

            playerData = new GhostPlayer
            {
                SteamName = steamName,
                SteamId = steamId,
                LevelId = PlayerSpriteStateExtensions.GetLevelId(),
                Color = (ModEntry.Preferences.LobbySettings.PersonalColor == 0) 
                    ? Color.White 
                    : GhostPlayer.ColorList[ModEntry.Preferences.LobbySettings.PersonalColor],
                RelativePosition = Vector2.Zero,
                AbsolutePosition = bodyComp.Position
            };
            this.bodyComp = bodyComp;

            InitializeItems(true);
        }

        void InitializeItems(bool is_self = false)
        {
            var color = Color.White;
            float alpha = 1f;
            if (playerData.IsDisposed || !is_self)
            {
                alpha = 0.5f;
                color *= 0.5f;
            }

            // colored crown
            Items[0] = new IconTextInfo(string.Empty, Game1.instance.contentManager.gui.Completed, playerData.Color * alpha, playerData.Color * alpha, yAlign: 4);
            
            // display name
            Items[1] = new TextInfo(playerData.SteamName, color, Game1.instance.contentManager.font.MenuFontSmall);
            
            // owner badge
            if (MultiplayerManager.instance.LobbyId.HasValue)
            {
                if (MultiplayerManager.instance.LobbyOwner == playerData.SteamId)
                {
                    Items[2] = new IconTextInfo("", Sprite.CreateSprite(ModEntry.HostFlag), Color.DarkGreen);
                }
            }

            if (!is_self)
            {
                // screen count
                Items[3] = new TextInfo(playerData.ScreenIndex1.ToString(), color, Game1.instance.contentManager.font.MenuFontSmall);
                
                // difference with player (* -1 needed to invert the Y direction)
                // to make it seem to the player that big number = better (the game works reversed lol)
                var difference = playerData.AbsolutePosition.DifferenceWithPlayer(GameLoop.m_player.m_body) * -1;
                color = (difference >= 0) ? Color.DarkRed : Color.DarkGreen;
                Items[4] = new TextInfo(difference.ToString("+#;-#;0"), color, Game1.instance.contentManager.font.MenuFontSmall);
            }
            else
            {
                Items[3] = new TextInfo(Camera.CurrentScreenIndex1.ToString(), color, Game1.instance.contentManager.font.MenuFontSmall);
            }
        }

        TextInfo[] Items { get; set; } = new TextInfo[5];
        public static int[] SizeForColumns { get; set; } = new int[5];

        protected override BTresult MyRun(TickData p_data)
        {
            // todo: make playerlist run for playerlistitem
            // todo: see if setting playerdata directly from MultiplayerManager does something
            if (bodyComp is BodyComp body)
            {
                playerData.AbsolutePosition = body.Position;
            }
            return base.MyRun(p_data);
        }

        public override void Draw(int x, int y, bool selected)
        {
            // format this better
            int initialX = x;

            // draw its stuff
            int i = 0;
            foreach (var item in Items)
            {
                if (item != null)
                {
                    item.Draw(x, y, false);
                }

                x += SizeForColumns[i];
                x += 10;
                i++;
            }

            // draw bottom line
            Sprite.Draw(new Rectangle(initialX, y + size.Y, size.X, 1));
        }

        private Point size;

        public override Point GetSize()
        {
            Point point = new Point(0, 1);

            int i = 0;
            foreach (var item in Items)
            {
                var size = Point.Zero;
                if (item != null)
                {
                    size = item.GetSize();
                    if (size.Y > point.Y)
                    {
                        point.Y = size.Y;
                    }
                }

                if (size.X > SizeForColumns[i])
                {
                    SizeForColumns[i] = size.X;
                }

                point.X += SizeForColumns[i];
                i++;

                // dont add end padding
                if (i == Items.Length)
                {
                    continue;
                }

                // horizontal padding
                point.X += 10;
            }

            size = point;
            //Debug.WriteLine($"[SIZE] [ITEM] {point.X} {point.Y}");
            return point;
        }
    }
}
