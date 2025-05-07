using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKingJumpArch.Extensions;
using Microsoft.Xna.Framework;
using System;

namespace JumpKingJumpArch.Models
{
    internal class GameLoopDraw
    {
        public GameLoopDraw(Harmony harmony)
        {
            harmony.Patch(
                typeof(GameLoop).GetMethod(nameof(GameLoop.Draw)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GameLoopDraw), nameof(Draw))));
        }

        static void Draw(GameLoop __instance)
        {
            // if not in pause (!PauseManager.instance.IsPaused)
            if (!Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>() && ModEntry.Preferences.IsEnabled)
            {
                DrawArchPoints();
            }
        }

        private static void DrawArchPoints(int limit = ModEntry.LIMIT - 1)
        {
            if (JumpChargeCalc.Vectors == null || JumpChargeCalc.Vectors.Count < 2)
            {
                return;
            }

            float min = Math.Min(limit, JumpChargeCalc.Vectors.Count);

            Vector2 previous = Camera.TransformVector2(JumpChargeCalc.Vectors[0]);
            previous.X += JumpChargeCalc.Hitbox.X / 2;
            previous.Y += JumpChargeCalc.Hitbox.Y;

            for (int i = 1; i < min; i++)
            {
                Vector2 current = Camera.TransformVector2(JumpChargeCalc.Vectors[i]);
                current.X += JumpChargeCalc.Hitbox.X / 2;
                current.Y += JumpChargeCalc.Hitbox.Y;

                Game1.spriteBatch.DrawLine(
                    previous,
                    current,
                    Color.Lime,
                    2);

                previous = current;

                if (i == JumpChargeCalc.Vectors.Count - 1)
                {
                    DrawLandingHelpers(current);
                }
            }
        }

        private static void DrawLandingHelpers(Vector2 current)
        {
            Vector2 start = new Vector2(current.X - JumpChargeCalc.Hitbox.X / 2, current.Y);
            Vector2 end = new Vector2(current.X + JumpChargeCalc.Hitbox.X / 2, current.Y);

            // horizontal line
            Game1.spriteBatch.DrawLine(
                start,
                end,
                Color.Lime);

            // vertical lines
            Game1.spriteBatch.DrawLine(
                start,
                new Vector2(start.X, start.Y - 6),
                Color.Lime);

            Game1.spriteBatch.DrawLine(
                end,
                new Vector2(end.X, end.Y - 6),
                Color.Lime);
        }
    }
}
