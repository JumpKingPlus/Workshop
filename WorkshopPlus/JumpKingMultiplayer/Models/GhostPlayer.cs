using JumpKing.GameManager;
using JumpKing.Util.DrawBT;
using JumpKing.Util;
using JumpKing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKing.Player;
using Microsoft.Xna.Framework.Graphics;
using static JumpKing.JKContentManager;
using JumpKing.JKMemory;
using JumpKingMultiplayer.Menu;
using Steamworks;
using JumpKingMultiplayer.Extensions;
using JumpKingMultiplayer.Menu.Lists;
using System.Diagnostics;

namespace JumpKingMultiplayer.Models
{
    internal class GhostPlayer : ISpriteEntity, IGhostPlayerData
    {
        internal class Tracker
        {
            GhostPlayer player;

            public List<TrackData> data { get; set; } = new List<TrackData>();

            public Tracker(GhostPlayer player)
            {
                this.player = player;
            }

            public void Track(TrackData item)
            {
                lock (data)
                {
                    data.Add(item);
                }
            }

            public static bool enableSync = false;

            public int replayFrameCount = 0;

            public void ReplayTick()
            {
                TrackData frame;
                if (enableSync && replayFrameCount <= 0) { return; }

                lock (data)
                {
                    if (data.Count == 0)
                    {
                        // sendUpdates = true;
                        return;
                    }
                    frame = data[0];
                    data.RemoveAt(0);
                }
                replayFrameCount -= 1;

                player.targetPos = new Vector2(frame.posX, frame.posY);
                player.skippedFrames = frame.skippedFrames;
                player.SetSprite(frame.sprite.ToSprite());
                player.SetSpriteEffects(
                    frame.flip == PlayerSpriteEffect.None 
                        ? SpriteEffects.None 
                        : frame.flip == PlayerSpriteEffect.FlipH 
                            ? SpriteEffects.FlipHorizontally 
                            : SpriteEffects.FlipVertically);

                // debug
                // player.m_body.Position += new Vector2(10f, 0);
            }
        }

        public int ScreenIndex1 { get; set; }
        public Vector2 RelativePosition { get; set; }
        public Vector2 AbsolutePosition { get; set; }
        public ulong? LevelId { get; set; }

        Vector2 targetPos { get; set; } = Vector2.Zero;
        int skippedFrames { get; set; } = 1;

        public Tracker tracker { get; set; }

        public string SteamName { get; set; }
        public Steamworks.CSteamID SteamId { get; set; }

        public bool IsDisposed { get; set; } = false;

        public Color Color { get; set; }


        public static Color[] ColorList = new Color[] { Color.Transparent, Color.Red, Color.Green, Color.Blue, Color.Purple, Color.Gold, Color.Orange, Color.Brown };

        public GhostPlayer()
        {
            this.tracker = new Tracker(this);
        }

        public GhostPlayer(string tag, CSteamID steamID, int idx) : this()
        {
            this.SteamName = tag;
            if (!Game1.instance.contentManager.font.MenuFontSmall.CanUseFontProperly(tag))
            {
                this.SteamName = "Jumper";
            }

            this.SteamId = steamID;
            Color = ColorList[idx];
            base.SetSprite(Game1.instance.contentManager.playerSprites.idle);
            RelativePosition = new Vector2(-100f, -100f);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void DrawFromOutside()
        {
            if (LevelId != PlayerSpriteStateExtensions.GetLevelId())
            {
                return;
            }

            switch (ModEntry.Preferences.GhostPlayerDisplayType)
            {
                case EGhostPlayerDisplayType.GhostPlayerOnly:
                    base.sprite.Draw(this.RelativePosition, base.spriteEffects, Color.White * ModEntry.Preferences.GhostPlayerOpacity);
                    break;

                case EGhostPlayerDisplayType.GhostPlayerAndLabel:
                    base.sprite.Draw(this.RelativePosition, base.spriteEffects, Color.White * ModEntry.Preferences.GhostPlayerOpacity);
                    DrawTag();
                    break;

                case EGhostPlayerDisplayType.GhostPlayerColored:
                    base.sprite.Draw(this.RelativePosition, base.spriteEffects, this.Color * ModEntry.Preferences.GhostPlayerOpacity);
                    break;

                case EGhostPlayerDisplayType.GhostPlayerColoredAndLabel:
                    base.sprite.Draw(this.RelativePosition, base.spriteEffects, this.Color * ModEntry.Preferences.GhostPlayerOpacity);
                    DrawTag();
                    break;

                default:
                    throw new Exception("GhostPlayerDisplayType is null!");
            }
        }

        void DrawTag()
        {
            if (!ModEntry.Preferences.IsLeaderboardEnabled) { return; }
            var color = Color * 0.7f;
            var charWidth = 2f;
            var offsetChars = SteamName.Length * charWidth;

            var textPos = this.RelativePosition - new Vector2((PlayerValues.PLAYER_WIDTH / 2f) + offsetChars, PlayerValues.PLAYER_HEIGHT + 18);
            TextHelper.DrawString(Game1.instance.contentManager.font.MenuFontSmall,
                SteamName, textPos, color, Vector2.Zero);
        }

        public static int frameNum = 0;

        public override void Destroy()
        {
            // nuh huh, not on my watch
            if (IsDisposed)
            {
                base.Destroy();
            }
        }

        protected override void Update(float p_delta)
        {
            if (IsDisposed == true)
            {
                base.Destroy();
            }

            frameNum += 1;
            base.Update(p_delta);

            tracker.ReplayTick();
            var dX = (targetPos.X - this.RelativePosition.X) / (skippedFrames == 0 ? 1 : skippedFrames);
            var posX = ErikMaths.ErikMath.MoveTowards(this.RelativePosition.X, targetPos.X, dX);
            var dY = (targetPos.Y - this.RelativePosition.Y) / (skippedFrames == 0 ? 1 : skippedFrames);
            var posY = ErikMaths.ErikMath.MoveTowards(this.RelativePosition.Y, targetPos.Y, dY);

            AbsolutePosition = new Vector2(posX, posY);

            var pos = Camera.TransformVector2(this.AbsolutePosition);
            // Debug.WriteLine($"x {m_body.position.X} y {m_body.position.Y}");
            this.RelativePosition = pos + new Vector2(PlayerValues.PLAYER_WIDTH / 2, PlayerValues.PLAYER_HEIGHT);
        }
    }
}
