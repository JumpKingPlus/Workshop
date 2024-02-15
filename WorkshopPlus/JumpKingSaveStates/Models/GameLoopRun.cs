using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates.Models
{
    class GameLoopRun
    {
        public GameLoopRun(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(GameLoop), "MyRun"),
                new HarmonyMethod(AccessTools.Method(typeof(GameLoopRun), nameof(Run)))
            );
        }

        static void Run(GameLoop __instance)
        {
            var traverse_instance = Traverse.Create(__instance);
            if (!traverse_instance.Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
            {
                CustomPadState state = JumpKingSaveStates.PadInstance.GetPressed();
                if (state.savePos)
                {
                    JumpKingSaveStates.Preferences.SetSaveState(
                        GameLoop.m_player.m_body.Position.X,
                        GameLoop.m_player.m_body.Position.Y,
                        GameLoop.m_player.m_body.LastScreen
                    );
                    Game1.instance.contentManager.audio.menu.MenuFail.Play();
                }
                if (state.loadPos)
                {
                    GameLoop.m_player.m_body.Position.X = JumpKingSaveStates.Preferences.PositionX;
                    GameLoop.m_player.m_body.Position.Y = JumpKingSaveStates.Preferences.PositionY;
                    GameLoop.m_player.m_body.Velocity = Vector2.Zero;
                    Camera.UpdateCamera(GameLoop.m_player.m_body.GetHitbox().Center);
                    Game1.instance.contentManager.audio.menu.Select.Play();
                }
            }
        }
    }
}
