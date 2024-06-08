using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.Util;
using Microsoft.Xna.Framework;

namespace JumpKingLastJumpValue.Models
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
            // if not in pause (!PauseManager.instance.IsPaused) AND if enabled
            if (JumpKingLastJumpValue.Preferences.IsEnabled &&
                !Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
            {
                if (JumpKingLastJumpValue.Preferences.DisplayType == ELastJumpDisplayType.Percentage)
                {
                    TextHelper.DrawString(
                    Game1.instance.contentManager.font.MenuFont,
                    $"Last Jump: {JumpChargeCalc.JumpPercentage.ToString("0.00")}%",
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
        }
    }
}
