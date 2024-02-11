using BehaviorTree;
using HarmonyLib;
using JumpKing;
using JumpKing.Mods;
using JumpKingLastJumpValue.Models;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLastJumpValue
{
    [JumpKingMod(IDENTIFIER)]
    public static class JumpKingLastJumpValue
    {
        const string IDENTIFIER = "Phoenixx19.LastJumpValue";
        const string HARMONY_IDENTIFIER = "Phoenixx19.LastJumpValue.Harmony";

        public static Preferences Preferences { get; } = new Preferences();

        [BeforeLevelLoad]
        public static void OnLevelStart()
        {
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif

            // setup harmony
            var harmony = new Harmony(HARMONY_IDENTIFIER);

            //harmony.Patch(JumpChargeCalc.Method, new HarmonyMethod(JumpChargeCalc.HarmonyMethod));

            // patching on each class (is better than attributes)
            new JumpChargeCalc(harmony);
            new GameLoopDraw(ref harmony);
            new MenuOptions(ref harmony);
        }
    }
}
