using JumpKing;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingAdditionalAnimations
{
    internal class JKUtils
    {
        /// <summary>
        /// Can't access <see cref="JumpKing.JKContentManager.Util.SpriteChopUtilGrid()"/> at all, so I just copied the code lol
        /// </summary>
        /// <param name="p_texture"></param>
        /// <param name="p_cells"></param>
        /// <param name="p_center"></param>
        /// <param name="p_source"></param>
        /// <returns></returns>
        public static Sprite[] SpriteChopUtilGrid(Texture2D p_texture, Point p_cells, Vector2 p_center, Rectangle p_source)
        {
            int num = p_source.Width / p_cells.X;
            int num2 = p_source.Height / p_cells.Y;
            Sprite[] array = new Sprite[p_cells.X * p_cells.Y];
            for (int i = 0; i < p_cells.X; i++)
            {
                for (int j = 0; j < p_cells.Y; j++)
                {
                    array[i + j * p_cells.X] = Sprite.CreateSpriteWithCenter(p_texture, new Rectangle(p_source.X + num * i, p_source.Y + num2 * j, num, num2), p_center);
                }
            }
            return array;
        }
    }
}
