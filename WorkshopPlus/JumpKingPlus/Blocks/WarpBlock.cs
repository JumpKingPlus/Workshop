using JumpKing.Level;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlus.Blocks
{
    /// <summary>
    /// An implementation of <see cref="IBlock"/> for the Warp Blocks used in Jump King+
    /// </summary>
    public class WarpBlock : IBlock, IBlockDebugColor
    {
        public const int PixelToGameFactor = 8;

        private readonly Rectangle collider;

        /// <summary>
        /// The X position to warp to
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y position to warp to
        /// </summary>
        public int Y { get; private set; }

        public Color DebugColor => new Color(0, 0, 75);

        /// <summary>
        /// Ctor for creating a <see cref="WarpBlock"/>
        /// </summary>
        /// <param name="p_collider">A <see cref="Rectangle"/> for the collision of the block</param>
        /// <param name="p_rawX">
        /// The raw X value from the block code indicating where to warp to, taken from the red channel. 
        /// Will be multiplied by <see cref="PixelToGameFactor"/> to get the actual in-game location
        /// </param>
        /// <param name="p_rawY">
        /// The raw Y value from the block code indicating where to warp to, taken from the green channel. 
        /// Will be multiplied by <see cref="PixelToGameFactor"/> to get the actual in-game location
        /// </param>
        public WarpBlock(Rectangle p_collider, int p_rawX, int p_rawY)
        {
            collider = p_collider;

            // 1 pixel in img = 8 in game
            X = p_rawX * PixelToGameFactor;
            Y = p_rawY * PixelToGameFactor;
        }

        /// <inheritdoc/>
        public Rectangle GetRect()
        {
            return collider;
        }

        /// <summary>
        /// Determines if the provided hitbox is colliding with this block.
        /// Return <see cref="BlockCollisionType.Collision_NonBlocking"/> if colliding, and <see cref="BlockCollisionType.NoCollision"/>
        /// if not
        /// </summary>
        /// <param name="p_hitbox">A <see cref="Rectangle"/> to test against</param>
        /// <param name="p_intersection">A <see cref="Rectangle"/> indicating the intersection of the collision</param>
        /// <returns>A <see cref="BlockCollisionType"/> indicating the type of collision</returns>
        public BlockCollisionType Intersects(Rectangle p_hitbox, out Rectangle p_intersection)
        {
            bool ret = collider.Intersects(p_hitbox);

            if (ret)
            {
                p_intersection = Rectangle.Intersect(p_hitbox, collider);
                return BlockCollisionType.Collision_NonBlocking;
            }
            else
            {
                p_intersection = new Rectangle(0, 0, 0, 0);
                return BlockCollisionType.NoCollision;
            }
        }
    }
}
