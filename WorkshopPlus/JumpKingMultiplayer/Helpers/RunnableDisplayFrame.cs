using BehaviorTree;
using HarmonyLib;
using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;
using Microsoft.Xna.Framework;
using JumpKingMultiplayer.Extensions;
using JumpKingMultiplayer.Menu.DisplayFrames;
using System.Diagnostics;

namespace JumpKingMultiplayer.Helpers
{
    public class RunnableDisplayFrame : DisplayFrame
    {
        protected Color color;
        protected float alpha;

        public RunnableDisplayFrame(GuiFormat p_format, BTresult p_static_result, Color color, float alpha)
            : base(p_format, p_static_result)
        {
            this.color = color;
            this.alpha = alpha;
        }

        public float Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public override BTresult Run(TickData p_data)
        {
            foreach (var child in Children)
            {
                child.Run(p_data);
            }
            return base.Run(p_data);
        }

        public new void Draw()
        {
            if (base.last_result == BTresult.Running)
            {
                var tFrame = Traverse.Create(this);
                if (FrameEnabled)
                {
                    tFrame.Field("m_gui_frame").GetValue<GuiFrame>().Draw(color, alpha);
                }

                tFrame.Field("m_format").GetValue<GuiFormat>()
                    .DrawMenuItems(
                        tFrame.Field("m_menu_items").GetValue<IMenuItem[]>(),
                        tFrame.Field("m_bounds").GetValue<Rectangle>()
                    );
            }
        }
    }
}
