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
    /// An implementation of <see cref="IBlock"/> for the Low Gravity block used in Jump King+
    /// </summary>
    public class LowGravityBlock : IBlock, IBlockDebugColor
    {
        private readonly Rectangle collider;

        /// <summary>
        /// Ctor for creating a <see cref="LowGravityBlock"/>
        /// </summary>
        /// <param name="p_collider">A <see cref="Rectangle"/> for the collision of the block</param>
        public LowGravityBlock(Rectangle p_collider)
        {
            collider = p_collider;
        }

        public Color DebugColor => new Color(224, 255, 255, 128);

        /// <summary>
        /// Gets the collider for the block
        /// </summary>
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
