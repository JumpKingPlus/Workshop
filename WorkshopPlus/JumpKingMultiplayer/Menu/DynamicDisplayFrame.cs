using BehaviorTree;
using HarmonyLib;
using JumpKing;
using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer.Menu
{
    public static class GuiFrameExtensions
    {
        public static void Draw(this GuiFrame frame, Color color, float alpha = 1f)
        {
            var m_bounds = Traverse.Create(frame).Field("m_bounds").GetValue<Rectangle>();
            Sprite[,] frameSprites = Game1.instance.contentManager.gui.FrameSprites;
            int width = frameSprites[0, 0].source.Width;
            int num = m_bounds.Width - width * 2;
            int num2 = m_bounds.Height - width * 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Sprite sprite = frameSprites[i, j];
                    Rectangle destinationRectangle = new Rectangle(0, 0, width, width);
                    switch (i)
                    {
                        case 1:
                            destinationRectangle.X = width;
                            destinationRectangle.Width = num;
                            break;
                        case 2:
                            destinationRectangle.X = num + width;
                            break;
                    }

                    switch (j)
                    {
                        case 1:
                            destinationRectangle.Y = width;
                            destinationRectangle.Height = num2;
                            break;
                        case 2:
                            destinationRectangle.Y = num2 + width;
                            break;
                    }

                    destinationRectangle.X += m_bounds.X;
                    destinationRectangle.Y += m_bounds.Y;
                    Game1.spriteBatch.Draw(sprite.texture, destinationRectangle, sprite.source, color * alpha);
                }
            }
        }
    }

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

    public class LeaderboardDisplayFrame : DynamicDisplayFrame
    {
        public LeaderboardDisplayFrame(GuiFormat p_format, BTresult p_static_result, float p_alpha)
            : base(p_format, p_static_result, p_alpha)
        {
        }

        public new void Draw()
        {
            base.Draw();
            Game1.spriteBatch.Draw(
                ModEntry.LeaderboardHeader, 
                position: new Vector2(
                    (JumpGame.GAME_RECT.Width / 2) - (ModEntry.LeaderboardHeader.Width / 2), // horizontally centered
                    Bounds.Y - ModEntry.LeaderboardHeader.Height), // vertically on top of the gui frame
                Color.White * m_alpha);
        }
    }

    public class DynamicDisplayFrame : RunnableDisplayFrame, INotifyPropertyChanged
    {
        public DynamicDisplayFrame(GuiFormat p_format, BTresult p_static_result, float p_alpha)
            : base(p_format, p_static_result, Color.White, p_alpha)
        {
            m_alpha = p_alpha;
        }

        internal float m_alpha;

        public override BTresult Run(TickData p_data)
        {
            foreach (var child in Children)
            {
                child.Run(p_data);
            }
            Initialize();
            GetBounds();
            return base.Run(p_data);
        }

        private void GetBounds()
        {
            var tFrame = Traverse.Create(this);
            var new_bounds = tFrame.Field("m_bounds").GetValue<Rectangle>();
            
            if (new_bounds != null)
            {
                var format = tFrame.Field("m_format").GetValue<GuiFormat>();
                new_bounds.Width -= format.all_margin * 2;
                
                if (new_bounds != last_bounds)
                {
                    Debug.WriteLine($"[SIZE] [FRAME] {new_bounds.Width} {new_bounds.Height} {new_bounds.X} {new_bounds.Y}");
                    Bounds = new_bounds;
                }
            }
        }

        private Rectangle last_bounds;

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public Rectangle Bounds
        {
            get => last_bounds;
            set
            {
                last_bounds = value;
                OnPropertyChanged();
            }
        }
    }
}
