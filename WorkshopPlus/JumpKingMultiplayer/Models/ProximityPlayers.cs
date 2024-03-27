using JumpKing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using JumpKingMultiplayer.Extensions;

namespace JumpKingMultiplayer.Models
{
    internal class ProximityPlayers
    {
        Sprite sprite;
        public int Threshold = 3;

        public ProximityPlayers()
        {
        }

        public void Draw()
        {
            if (!ModEntry.Preferences.IsProximityPlayersEnabled)
            {
                return;
            }

            foreach (IGhostPlayerData ghost in MultiplayerManager.instance.Players)
            {
                // not in this level
                if (ghost.IsDisposed || ghost.LevelId != PlayerSpriteStateExtensions.GetLevelId())
                {
                    continue;
                }

                // on screen
                if (ghost.ScreenIndex1 == Camera.CurrentScreenIndex1)
                {
                    continue;
                }

                // too far behind or ahead
                if (ghost.ScreenIndex1 < Camera.CurrentScreenIndex1 - Threshold || 
                    ghost.ScreenIndex1 > Camera.CurrentScreenIndex1 + Threshold)
                {
                    continue;
                }

                // creates opacity based on distance
                var calc = Camera.CurrentScreenIndex1 - ghost.ScreenIndex1;
                var range = Math.Abs(calc);
                float alpha = 0.8f;
                while (range > 0)
                {
                    alpha -= 0.22f;
                    range--;
                }

                var arrowSource = Game1.instance.contentManager.gui.ArrowLeft.source;
                var resizedRect = arrowSource.Size;
                resizedRect.X *= 2;
                resizedRect.Y *= 2;

                // relative position for player proximity
                Vector2 drawingPosition;
                float get_rotated;
                //Vector2 textPosition;
                if (calc >= 0)
                {
                    drawingPosition = new Vector2(ghost.AbsolutePosition.X, JumpGame.GAME_RECT.Height - 9f);
                    get_rotated = MathHelper.ToRadians(270);
                    //textPosition = drawingPosition - new Vector2(0, 15);
                }
                else
                {
                    drawingPosition = new Vector2(ghost.AbsolutePosition.X, 9f);
                    get_rotated = MathHelper.ToRadians(90);
                    //textPosition = drawingPosition + new Vector2(0, 15);
                }

                Rectangle rect = new Rectangle(drawingPosition.ToPoint(), resizedRect);

                // if ghost player y pos is negative = arrow up
                // otherwise arrow down

                //textPosition.X -= Game1.instance.contentManager.font.MenuFontSmall.MeasureString(ghost.SteamName).X / 2;
                //Game1.spriteBatch.DrawString(
                //    Game1.instance.contentManager.font.MenuFontSmall, 
                //    ghost.SteamName, 
                //    textPosition,
                //    Color.White
                //);

                Game1.spriteBatch.Draw(
                    Game1.instance.contentManager.gui.ArrowLeft.texture,
                    destinationRectangle: rect,
                    sourceRectangle: arrowSource,
                    rotation: get_rotated,
                    color: ghost.Color * alpha
                );
            }
        }
    }
}