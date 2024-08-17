using BehaviorTree;
using HarmonyLib;
using JumpKing.Player;
using System.Diagnostics;

namespace JumpKingLastJumpValue.Models
{
    [HarmonyPatch(typeof(JumpState))]
    class JumpChargeCalc
    {
        public JumpChargeCalc(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(JumpState), "MyRun"),
                postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Run)))
            );
        }

        private static float previous_timer { get; set; }

        static void Run(TickData p_data, BTresult __result, JumpState __instance)
        {
            // not charging a jump
            if (__result == BTresult.Failure)
            {
                return;
            }

            // getting timer for percentage
            float m_timer = (float)Traverse.Create(__instance).Field("m_timer").GetValue();

            // missing last frame because JK clears it before being able to access it
            if (__result == BTresult.Success)
            {
                m_timer = previous_timer + p_data.delta_time * __instance.body.GetMultipliers();
            }

            // calculating percentage
            JumpPercentage = m_timer / __instance.CHARGE_TIME;

            // reset values
            if (__instance.last_result != BTresult.Running)
            {
                JumpFrames = -1;
            }

            // calculating frames
            JumpFrames++;

            // save previous value
            previous_timer = m_timer;

#if DEBUG
            Debug.WriteLine($"JumpFrames: {JumpFrames} - JumpPercentage: {JumpPercentage} - Result: {__result} / {__instance.last_result} - Delta: {p_data.delta_time * __instance.body.GetMultipliers()}");
#endif
        }

        public static int JumpFrames { get; internal set; }
        public static float JumpPercentage { get; internal set; }
    }
}
