using HarmonyLib;
using JumpKing.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates.Models
{
    public class CustomPadInstance
    {
        private IPad pad;
        private CustomPadState last_state;
        private CustomPadState current_state;

        public CustomPadInstance(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method("JumpKing.Controller.ControllerManager:Update"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(CustomPadInstance), nameof(MyUpdate)))
            );
        }

        public void Update(object __instance)
        {
            var traverse_pad = Traverse.Create(__instance).Field("_current_main").Field("m_pad").GetValue<IPad>();
            if (traverse_pad != pad)
            {
                pad = traverse_pad;
            }

            if (pad != null)
            {
                this.last_state = this.current_state;
                this.current_state = this.GetPadState();
#if DEBUG
                Debug.WriteLine($"{EBinding.SavePos}: {current_state.savePos} " +
                    $"- {EBinding.LoadPos}: {current_state.loadPos} " +
                    $"- {EBinding.DeletePos}: {current_state.deletePos}");
#endif
            }
        }

        public static void MyUpdate(object __instance)
        {
            JumpKingSaveStates.PadInstance.Update(__instance);
        }

        public CustomPadState GetState()
        {
            return this.current_state;
        }

        public CustomPadState GetPressed()
        {
            return new CustomPadState
            {
                savePos = (!last_state.savePos && current_state.savePos),
                loadPos = (!last_state.loadPos && current_state.loadPos),
                deletePos = (!last_state.deletePos && current_state.deletePos)
            };
        }

        private CustomPadState GetPadState()
        {
            int[] pressedButtons = this.pad.GetPressedButtons();
            return new CustomPadState
            {
                savePos = IsPressed(pressedButtons, JumpKingSaveStates.Preferences.KeyBindings[EBinding.SavePos]),
                loadPos = IsPressed(pressedButtons, JumpKingSaveStates.Preferences.KeyBindings[EBinding.LoadPos]),
                deletePos = IsPressed(pressedButtons, JumpKingSaveStates.Preferences.KeyBindings[EBinding.DeletePos])
            };
        }

        private bool IsPressed(int[] pressed, int[] bind)
        {
            foreach (int num in pressed)
            {
                foreach (int num2 in bind)
                {
                    if (num == num2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public struct CustomPadState
    {
        public bool savePos;
        public bool loadPos;
        public bool deletePos;
    }
}
