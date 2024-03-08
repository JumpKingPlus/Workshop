using HarmonyLib;
using JumpKing.Controller;
using JumpKing.GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingTAS.Models
{
    public class PadStateGet
    {
        public PadStateGet(Harmony harmony)
        {
            harmony.Patch(
                typeof(ControllerManager).GetMethod(nameof(ControllerManager.GetPadState)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PadStateGet), nameof(PostPadState)))
            );
            harmony.Patch(
                typeof(ControllerManager).GetMethod(nameof(ControllerManager.GetPressedPadState)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(PadStateGet), nameof(PostPressedPadState)))
            );
        }

        static void PostPadState(ref PadState __result)
        {
            if (!Manager.Running)
            {
                return;
            }
            __result = Manager.GetPadState();
        }

        static void PostPressedPadState(ref PadState __result)
        {
            if (!Manager.Running)
            {
                return;
            }
            __result = Manager.GetPressed();
        }
    }
}
