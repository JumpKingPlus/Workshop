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
        static void Draw()
        {
            // if not in pause AND if enabled

            TextHelper.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                $"{JumpKingLastJumpValue.JumpFrames} frames",
                new Vector2(12f, 26f),
                Color.White, Vector2.Zero, true);

            TextHelper.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                $"{JumpKingLastJumpValue.JumpPercentage.ToString("0.00")}%",
                new Vector2(12f, 44f),
                Color.White, Vector2.Zero, true);
        }
    }
}
