using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;
using JumpKing.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLastJumpValue
{
    [HarmonyPatch(typeof(GameLoop))]
    class GameLoopDraw
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameLoop.Draw))]
        static void Draw(GameLoop __instance)
        {
            // if not in pause AND if enabled
            if (JumpKingLastJumpValue.IsEnabled && !Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
            {
                if (JumpKingLastJumpValue.DisplayType == ELastJumpDisplayType.Percentage)
                {
                    TextHelper.DrawString(
                    Game1.instance.contentManager.font.MenuFont,
                    $"{JumpKingLastJumpValue.JumpPercentage.ToString("0.00")}%",
                    new Vector2(12f, 26f),
                    //new Vector2(12f, 44f),
                    Color.White, Vector2.Zero, true);
                    return;
                }

                TextHelper.DrawString(
                    Game1.instance.contentManager.font.MenuFont,
                    $"{JumpKingLastJumpValue.JumpFrames} frames",
                    new Vector2(12f, 26f),
                    Color.White, Vector2.Zero, true);
            }
        }
    }
}
