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
    /// An implementation of <see cref="IBlock"/> for the Thin Snow block used in Jump King+
    /// </summary>
    public class ThinSnowBlock : BoxBlock
    {
        /// <inheritdoc/>
        protected override bool canBlockPlayer => true;

        /// <summary>
        /// Ctor for creating a <see cref="ThinSnowBlock"/>
        /// </summary>
        /// <param name="p_collider">A <see cref="Rectangle"/> for the collision of the block</param>
        public ThinSnowBlock(Rectangle p_collider) : base(p_collider)
        {
        }
    }
}
