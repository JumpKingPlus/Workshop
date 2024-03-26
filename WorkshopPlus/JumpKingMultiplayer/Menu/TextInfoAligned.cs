using JumpKing.PauseMenu.BT;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer.Menu
{
    enum TextAlignment
    {
        Left,
        Center,
        Right
    }

    internal class TextInfoAligned : TextInfo
    {
        TextAlignment alignment;
        public Rectangle bounds;

        public TextInfoAligned(string p_text, Color p_color, TextAlignment p_alignment, Rectangle bounds) : base(p_text, p_color)
        {
            alignment = p_alignment;
            this.bounds = bounds;
        }

        public override void Draw(int x, int y, bool selected)
        {
            var textSize = base.GetSize();
            var location = new Vector2(x, y);
            
            switch (alignment)
            {
                case TextAlignment.Center:
                    location.X += (bounds.Width / 2) - (textSize.X / 2);
                    break;

                case TextAlignment.Right:
                    location.X += bounds.Width - textSize.X;
                    break;

                case TextAlignment.Left:
                default:
                    break;
            }

            base.Draw((int)location.X, (int)location.Y, selected);
        }
    }
}
