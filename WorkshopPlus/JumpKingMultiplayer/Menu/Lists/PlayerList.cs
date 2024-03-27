using BehaviorTree;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;
using JumpKing.Workshop;
using JumpKingMultiplayer.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using JumpKingMultiplayer.Extensions;
using JumpKingMultiplayer.Menu;
using JumpKingMultiplayer.Menu.Lists;

namespace JumpKingMultiplayer.Menu.Lists
{
    public static class TextInfoExtentions
    {
        public static bool CanUseFontProperly(this SpriteFont font, string text)
        {
            var glyphs = font.GetGlyphs();
            foreach (char character in text)
            {
                if (!glyphs.ContainsKey(character))
                    return false;
            }
            return true;
        }
    }

    internal class PlayerList : IBTSimpleMenuItem, UnSelectable
    {
        protected List<TextInfo> children = new List<TextInfo>();

        protected Rectangle bounds = Rectangle.Empty;
        protected GuiFormat format;

        public PlayerList(GuiFormat format)
        {
            this.format = format;
        }

        public override void Draw(int x, int y, bool selected)
        {
            var newBounds = new Rectangle(x, y, bounds.Width, bounds.Height);
            format.DrawMenuItems(children.Cast<IMenuItem>().ToArray(), newBounds);
        }

        public override Point GetSize()
        {
            return format.CalculateBounds(children.Cast<IMenuItem>().ToArray()).Size;
        }

        private float m_timer = 0f;
        protected readonly float RefreshTime = 0.2f;

        protected override BTresult MyRun(TickData p_data)
        {
            m_timer += p_data.delta_time;

            if (m_timer > RefreshTime && Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                m_timer = 0;
                var players = MultiplayerManager.instance.Players
                //    //.Where(x => x.IsDisposed == false) // this is rather useless to keep commented
                    .Where(x => x.LevelId == PlayerSpriteStateExtensions.GetLevelId())
                    .ToList();

                //if (snapshot_players != players)
                //{
                //    Debug.WriteLine("this shit different");
                //    var i = 0;
                //    foreach (var player in players)
                //    {
                //        children[i] = new PlayerListItem(players[i]);
                //        i++;
                //    }

                //    children.Add(new PlayerListItem(
                //        "You", MultiplayerManager.instance.UserSteamId, GameLoop.m_player.m_body
                //    ));
                //}
                //snapshot_players = players;
                List<IGhostPlayerData> _players = players.Cast<IGhostPlayerData>().ToList();
                List<PlayerListItem> players_in_map = new List<PlayerListItem>();

                // add all players
                foreach (var item in _players)
                {
                    players_in_map.Add(new PlayerListItem(item));
                }

                // add self
                players_in_map.Add(new PlayerListItem(
                    "You", MultiplayerManager.instance.UserSteamId, GameLoop.m_player.m_body
                ));

                // remove all and readd
                RemoveChildrenSafely();
                children = players_in_map
                    .OrderBy(x => x.playerData.AbsolutePosition.Y)
                    .Cast<TextInfo>().ToList();

                // other maps
#if true
                if (players.Count != MultiplayerManager.instance.Players.Count)
                {
                    var playersinothermaps = MultiplayerManager.instance.Players
                        .Where(x => x.LevelId != PlayerSpriteStateExtensions.GetLevelId())
                        .ToList();

                    if (playersinothermaps.Count != 0)
                    {
                        var levels = playersinothermaps.Select(x => x.LevelId).Distinct().ToList();
                        foreach (var item in levels)
                        {
                            if (!item.HasValue)
                            {
                                // default maps
                                children.Add(new TextInfo("Playing official levels:", Color.LightGray, Game1.instance.contentManager.font.MenuFontSmall));
                            }
                            else
                            {
                                // custom maps
                                try
                                {
                                    CachedList.Cached.TryGetValue(item.Value, out SerializedUGCDetails details);
                                    children.Add(new TextInfo($"Playing {details.m_rgchTitle}:", Color.LightGray, Game1.instance.contentManager.font.MenuFontSmall));
                                }
                                catch (Exception)
                                {
                                    children.Add(new TextInfo($"Playing an unknown level:", Color.LightGray, Game1.instance.contentManager.font.MenuFontSmall));
                                }
                            }

                            // list thru
                            foreach (var ghost in playersinothermaps.Where(x => x.LevelId == item))
                            {
                                children.Add(new PlayerListItem(ghost));
                            }
                        }
                    }
                } 
#endif
                bounds = format.CalculateBounds(children.Cast<IMenuItem>().ToArray());
            }
            return BTresult.Failure;
        }

        void RemoveChildrenSafely()
        {
            foreach (var item in children)
            {
                item.OnDispose();
            }
            children.Clear();
        }
    }
}
