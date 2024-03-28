using JumpKing.PauseMenu;
using JumpKing;
using JumpKingMultiplayer.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorTree;

namespace JumpKingMultiplayer.Models.Infos
{
    internal class Info
    {
        const float ALPHA = 0.7f;

        protected RunnableDisplayFrame frame;
        protected float alpha;
        protected GuiFormat format = new GuiFormat
        {
            all_padding = 12,
            all_margin = 15,
            element_margin = 5,
            anchor_bounds = new Rectangle(0, 0, JumpGame.GAME_RECT.Width, JumpGame.GAME_RECT.Height),
            anchor = new Vector2(1f, 0f),
        };

        public Info(float alpha = ALPHA)
        {
            this.alpha = alpha;
            Initialize();
        }

        protected virtual void Initialize()
        {
            frame = new RunnableDisplayFrame(format, BTresult.Running, Color.White, alpha);
            frame.Initialize();
        }

        public virtual void Run(TickData data)
        {
            frame.Run(data);
        }

        public virtual void Draw()
        {
            frame.Draw();
        }
    }
}
