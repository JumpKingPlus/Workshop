﻿using BehaviorTree;
using EntityComponent;
using EntityComponent.BT;
using HarmonyLib;
using JumpKing.Controller;
using JumpKing.PauseMenu.BT.Actions.BindController;
using Microsoft.Xna.Framework;
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
        private EBinding m_button;
        private int m_order_index;
        private object m_pad;

        public CustomBindButton(Entity p_entity, EBinding p_button, int p_order_index) : base(p_entity)
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
                var binds = JumpKingSaveStates.Preferences.KeyBindings[m_button];
                JumpKingSaveStates.Preferences.KeyBindings[m_button] = Poll(binds, pressedButtons[0]);
#if DEBUG
                Debug.WriteLine($"Binded '{m_button}' with {pressedButtons[0]}!");
#endif
                return BTresult.Success;
            }
            return BTresult.Running;
        }

        private int[] Poll(int[] array, int num)
        {
            if (array == null)
            {
                array = new int[this.m_order_index + 1];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = ((i == this.m_order_index) ? num : -1);
                }
            }
            else if (array.Length > this.m_order_index)
            {
                array[this.m_order_index] = num;
            }
            else if (array.Length - 1 < this.m_order_index)
            {
                int[] array2 = new int[this.m_order_index + 1];
                for (int j = 0; j < array2.Length; j++)
                {
                    if (j < array.Length)
                    {
                        array2[j] = array[j];
                    }
                    else if (j == this.m_order_index)
                    {
                        array2[j] = num;
                    }
                    else
                    {
                        array2[j] = -1;
                    }
                }
                array = array2;
            }
            for (int k = 0; k < array.Length; k++)
            {
                for (int l = array.Length - 1; l > k; l--)
                {
                    if (array[k] == array[l])
                    {
                        int[] array3 = new int[array.Length - 1];
                        for (int m = 0; m < array.Length; m++)
                        {
                            if (m < l)
                            {
                                array3[m] = array[m];
                            }
                            else if (m > l)
                            {
                                array3[m - 1] = array[m];
                            }
                        }
                        array = array3;
                    }
                }
            }
            return array;
        }
    }
}
