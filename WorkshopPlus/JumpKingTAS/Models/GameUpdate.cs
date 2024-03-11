using HarmonyLib;
using JumpKing;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TimerCallback;
using static JumpKing.Player.InputComponent;

namespace JumpKingTAS.Models
{
    public class GameUpdate
    {
        public GameUpdate(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(Game1), "Update"),
                new HarmonyMethod(AccessTools.Method(typeof(GameUpdate), nameof(Update)))
            );
            new ReversePatcher(
                harmony,
                AccessTools.Method(typeof(Game), "Update"),
                new HarmonyMethod(AccessTools.Method(typeof(GameUpdate), nameof(DummyUpdate)))
            );
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void DummyUpdate(Game instance, GameTime gameTime)
        {

        }

        private static Traverse Game1instance { get; set; }
        private static MethodInfo _baseUpdate = AccessTools.Method(typeof(Game), "Update");

        static bool Update(GameTime gameTime, Game1 __instance)
        {
            SteamAPI.RunCallbacks();

            if (Game1instance != Traverse.Create(__instance))
                Game1instance = Traverse.Create(__instance);

            float p_delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < Manager.FrameLoops; i++)
            {
                Manager.UpdateInputs();
                if ((Manager.state & State.FrameStep) == State.FrameStep)
                {
                    DummyUpdate(__instance, gameTime);
                    return false;
                }
                //__instance._callback_manager.Update(p_delta);
                Game1instance.Field("_callback_manager").GetValue<CallbackManager>().Update(p_delta);
                //__instance.MyUpdate(gameTime);
                AccessTools.Method(typeof(Game1), "MyUpdate").Invoke(__instance, new object[] { gameTime });
                //base.Update(gameTime);
                DummyUpdate(__instance, gameTime);
                //_baseUpdate.Invoke(__instance as Game, new object[] { gameTime });
            }
            return false;
        }
    }
}
