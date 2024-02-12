using HarmonyLib;
using JumpKing.GameManager;
using JumpKing.Util;
using JumpKing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JumpKingPlayerCoordinates.Models
{
    class GameLoopDraw
    {
        /// <summary>
        ///  JumpKing.GameManager.GameLoop:RelativeCoordsFormat()
        /// </summary>
        private static MethodInfo RelativePosition => AccessTools.Method(typeof(GameLoop), "RelativeCoordsFormat");

        public GameLoopDraw(Harmony harmony)
        {
            harmony.Patch(
                typeof(GameLoop).GetMethod(nameof(GameLoop.Draw)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GameLoopDraw), nameof(Draw)))
            );
        }

        static void Draw(GameLoop __instance)
        {
            // if not in pause (!PauseManager.instance.IsPaused) AND if enabled
            if (JumpKingPlayerCoordinates.Preferences.IsEnabled &&
                !Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
            {
                string text = $"X: {GameLoop.m_player.m_body.Position.X.ToString("0.0000")} Y: ";
                
                if (JumpKingPlayerCoordinates.Preferences.DisplayType == Coordinates.Absolute)
                {
                    text = string.Concat(text, GameLoop.m_player.m_body.Position.Y.ToString("0.0000"));
                    
                    TextHelper.DrawString(
                        Game1.instance.contentManager.font.MenuFont,
                        text,
                        new Vector2(12f, 44f),
                        //new Vector2(12f, 44f),
                        Color.White, Vector2.Zero, true
                    );
                    return;
                }

                // how is this legal
                // https://stackoverflow.com/questions/135443/how-do-i-use-reflection-to-invoke-a-private-method
                text = string.Concat(text, RelativePosition.Invoke(__instance, new object[] { }));
                TextHelper.DrawString(
                    Game1.instance.contentManager.font.MenuFont,
                    text,
                    new Vector2(12f, 44f),
                    Color.White, Vector2.Zero, true
                );
            }
        }
    }
}
