using BehaviorTree;
using EntityComponent;
using EntityComponent.BT;
using HarmonyLib;
using JumpKing;
using JumpKing.Controller;
using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates.Menu
{
    public class CustomBindDisplay : EntityBTNode, IMenuItem, UnSelectable
    {
        private EBinding m_button;
        private object m_pad;
        private SpriteFont Font => Game1.instance.contentManager.font.MenuFontSmall;

        public CustomBindDisplay(Entity p_entity, EBinding p_button)
            : base(p_entity)
        {
            m_button = p_button;
        }

        public void Draw(int x, int y, bool selected)
        {
            // OnNewRun doesnt work
            if (m_pad is null)
            {
                // GetComponent
                MethodInfo m1 = typeof(EntityBTNode).GetMethod(nameof(EntityBTNode.GetComponent));

                // <BlackBoardComp>
                MethodInfo generic = m1.MakeGenericMethod(AccessTools.TypeByName("EntityComponent.BlackBoardComp"));
                var _x = generic.Invoke(this, null);

                // .Get
                MethodInfo m2 = AccessTools.Method("EntityComponent.BlackBoardComp:Get");

                // <PadInstance>
                MethodInfo generic2 = m2.MakeGenericMethod(AccessTools.TypeByName("JumpKing.Controller.PadInstance"));

                // ("BBKEY_PAD");
                m_pad = generic2.Invoke(_x, new object[] { "BBKEY_PAD" });
            }

            var m_traverse_pad = Traverse.Create(m_pad);

            // key type
            string p_string = m_button.ToString() + " : ";

            var pad = m_traverse_pad.Method("GetPad").GetValue<IPad>();

            MenuItemHelper.Draw(x, y, p_string, Color.Gray, Font);

            int x2 = GetSize().X;
            foreach (int bind in JumpKingSaveStates.Preferences.KeyBindings[m_button])
            {
                x += (int)((float)x2 / 3f);

                // this is so dumb
                p_string = Traverse.Create(pad).Method("ButtonToString", new object[] { bind }).GetValue<string>();
                //p_string = pad.ButtonToString(bind);

                p_string = FormatString(p_string);
                MenuItemHelper.Draw(x, y, p_string, Color.Gray, Font);
            }

            if (JumpKingSaveStates.Preferences.KeyBindings[m_button].Length == 0)
            {
                x += (int)((float)x2 / 3f);
                p_string = "-";
                MenuItemHelper.Draw(x + (int)((float)x2 / 3f * 1f), y, p_string, Color.Gray, Font);
            }
        }

        private string FormatString(string p_string)
        {
            int num = GetSize().X / 3;
            if (MenuItemHelper.GetSize(p_string, Font).X > num)
            {
                while (MenuItemHelper.GetSize(p_string, Font).X > num)
                {
                    p_string = p_string.Substring(0, p_string.Length - 1);
                }

                return p_string.Substring(0, p_string.Length - 1) + "*";
            }

            return p_string;
        }

        public Point GetSize()
        {
            return MenuItemHelper.GetSize("xbox 360 controller 1____         ", Font);
        }

        protected override BTresult MyRun(TickData p_data)
        {
            return BTresult.Failure;
        }
    }
}
