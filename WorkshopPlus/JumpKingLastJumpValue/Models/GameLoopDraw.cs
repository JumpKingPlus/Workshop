using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.Player;
using JumpKing.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace JumpKingLastJumpValue.Models
{
    class GameLoopDraw
    {
        public static BodyComp bodyComp;
        public static Texture2D texture;

        public GameLoopDraw(Harmony harmony)
        {
            harmony.Patch(
                typeof(GameLoop).GetMethod(nameof(GameLoop.Draw)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GameLoopDraw), nameof(Draw)))
            );
        }

        static void Draw(GameLoop __instance)
        {
            // if not in pause (!PauseManager.instance.IsPaused)
            if (!Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
            {
                if (JumpKingLastJumpValue.Preferences.IsEnabled)
                {
                    DrawText();
                }
                if (JumpKingLastJumpValue.Preferences.ShowGauge)
                {
                    DrawGauge();
                }
            }
        }

        private static void DrawText()
        {
            if (JumpKingLastJumpValue.Preferences.DisplayType == ELastJumpDisplayType.Percentage)
            {
                TextHelper.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                $"Last Jump: {(JumpChargeCalc.JumpPercentage * 100.0f).ToString("0.00")}%",
                new Vector2(12f, 26f),
                //new Vector2(12f, 44f),
                Color.White, Vector2.Zero, true);
                return;
            }

            TextHelper.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                $"{JumpChargeCalc.JumpFrames} frames",
                new Vector2(12f, 26f),
                Color.White, Vector2.Zero, true);
        }

        private static void DrawGauge()
        {
            if (bodyComp == null || texture == null)
            {
                return;
            }

            // when in water the percentages are a bit off (and can go above 100) so we limit
            float percentage = Math.Min(JumpChargeCalc.JumpPercentage, 100.0f);
            int absolute = (int)(texture.Height * percentage);
            Vector2 position = new Vector2(
                Math.Max(0, (int)(bodyComp.Position.X + 0.5f) - 12),
                bodyComp.Position.Y + Camera.CurrentScreen * 360 - 14);
            Vector2 vectorBottom = new Vector2(
                position.X,
                position.Y + texture.Height - absolute);
            Rectangle rectangleBottom = new Rectangle(
                texture.Width / 2,
                texture.Height - absolute,
                texture.Width / 2,
                absolute);

            Game1.spriteBatch.Draw(
                texture: texture,
                position: vectorBottom,
                sourceRectangle: rectangleBottom,
                color: Color.White);
            Game1.spriteBatch.Draw(
                texture: texture,
                position: position,
                sourceRectangle: new Rectangle(0, 0, texture.Width / 2, texture.Height),
                color: Color.White);
        }

    }
}

