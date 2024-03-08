using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace JumpKingTAS.Models
{
    class GameLoopDraw
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
            if (!Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
            {
                TextHelper.DrawString(
                    Game1.instance.contentManager.font.MenuFont,
                    "Tool Assisted Run",
                    new Vector2(468f, 351f),
                    Color.DarkRed, Vector2.One, 
                    true
                );
            }
        }
    }
}
