using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.Player;
using JumpKingJumpArch.Extensions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingJumpArch.Models
{
    internal class GameLoopDraw
    {
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
            if (!Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>() && ModEntry.Preferences.IsEnabled)
            {
                DrawArchPoints();
            }
        }

        private static void DrawArchPoints(int limit = 75)
        {
            if (JumpChargeCalc.Vectors == null)
            {
                return;
            }

            var min = Math.Min(limit, JumpChargeCalc.Vectors.Count - 1);

            for (int i = 0; i < JumpChargeCalc.Vectors.Count; i++)
            {
                if (i == 0)
                    continue;

                if (i == limit)
                    return;


                var previous = Camera.TransformVector2(JumpChargeCalc.Vectors[i - 1]);
                var current = Camera.TransformVector2(JumpChargeCalc.Vectors[i]);

                // this cast shit is so stupid
                int g = (int)((float)((float)(min - i) / (float)min) * 255);
                int a = (int)(((float)i / (float)min) * 255);

                // there should be a function that makes this better in JK but I could not be bothered to find it
                previous.X += JumpChargeCalc.Hitbox.X / 2;
                previous.Y += JumpChargeCalc.Hitbox.Y;
                current.X += JumpChargeCalc.Hitbox.X / 2;
                current.Y += JumpChargeCalc.Hitbox.Y;

                // alpha doesnt work for some reason
                Game1.spriteBatch.DrawLine(
                    previous,
                    current,
                    new Color(0, g, 0, a),
                    2
                );

                if (i == JumpChargeCalc.Vectors.Count - 1)
                    DrawLandingHelpers(current);
            }
        }

        private static void DrawLandingHelpers(Vector2 current)
        {
            var start = new Vector2(current.X - JumpChargeCalc.Hitbox.X / 2, current.Y);
            var end = new Vector2(current.X + JumpChargeCalc.Hitbox.X / 2, current.Y);

            // horizontal line
            Game1.spriteBatch.DrawLine(
                start,
                end,
                Color.Black,
                1
            );

            // vertical lines
            Game1.spriteBatch.DrawLine(
                start,
                new Vector2(start.X, start.Y - 6),
                Color.Black,
                1
            );

            Game1.spriteBatch.DrawLine(
                end,
                new Vector2(end.X, end.Y - 6),
                Color.Black,
                1
            );
        }
    }
}
