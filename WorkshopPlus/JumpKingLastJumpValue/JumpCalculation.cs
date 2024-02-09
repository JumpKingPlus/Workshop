using BehaviorTree;
using HarmonyLib;
using JumpKing.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLastJumpValue
{
    [HarmonyPatch(typeof(JumpState))]
    class JumpCalculation
    {
        private static float previous_timer { get; set; }

        [HarmonyPostfix]
        [HarmonyPatch("MyRun")]
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
            JumpKingLastJumpValue.JumpPercentage = m_timer / __instance.CHARGE_TIME * 100f;

            // reset values
            if (__instance.last_result != BTresult.Running)
            {
                JumpKingLastJumpValue.JumpFrames = 0;
            }

            // calculating frames
            JumpKingLastJumpValue.JumpFrames++;

            // save previous value
            previous_timer = m_timer;

#if DEBUG
            Debug.WriteLine($"JumpFrames: {JumpKingLastJumpValue.JumpFrames} - JumpPercentage: {JumpKingLastJumpValue.JumpPercentage} - Result: {__result} / {__instance.last_result} - Delta: {p_data.delta_time * __instance.body.GetMultipliers()}");
#endif
        }
    }
}
