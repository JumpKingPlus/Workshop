using BehaviorTree;
using EntityComponent;
using EntityComponent.BT;
using HarmonyLib;
using JumpKing.Controller;
using JumpKing.PauseMenu.BT.Actions.BindController;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates.Menu
{
    public class CustomBindButton : EntityBTNode
    {
        private string m_button;
        private int m_order_index;
        private object m_pad;

        public CustomBindButton(Entity p_entity, string p_button, int p_order_index) : base(p_entity)
        {
            m_button = p_button;
            m_order_index = p_order_index;
        }

        protected override void OnNewRun()
        {
            // GetComponent
            MethodInfo m1 = typeof(EntityBTNode).GetMethod(nameof(EntityBTNode.GetComponent));

            // <BlackBoardComp>
            MethodInfo generic = m1.MakeGenericMethod(AccessTools.TypeByName("EntityComponent.BlackBoardComp"));
            var x = generic.Invoke(this, null);

            // .Get
            MethodInfo m2 = AccessTools.Method("EntityComponent.BlackBoardComp:Get");

            // <PadInstance>
            MethodInfo generic2 = m2.MakeGenericMethod(AccessTools.TypeByName("JumpKing.Controller.PadInstance"));

            // ("BBKEY_PAD");
            m_pad = generic2.Invoke(x, new object[] { "BBKEY_PAD" });
        }

        protected override BTresult MyRun(TickData p_data)
        {
            var m_traverse_pad = Traverse.Create(m_pad);

            var is_valid = m_traverse_pad.Property("IsValid").GetValue<bool>();
            var is_connected = m_traverse_pad.Property("IsConnected").GetValue<bool>();

            if (!is_valid || !is_connected)
            {
                return BTresult.Failure;
            }

            var pressedButtons = m_traverse_pad.Method("GetPad").Method("GetPressedButtons").GetValue<int[]>();
            if (pressedButtons.Length != 0)
            {
                Debug.WriteLine($"Binded '{m_button}' with {pressedButtons[0]}!");
                return BTresult.Success;
            }
            return BTresult.Running;
        }
    }
}
