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
    /// An implementation of <see cref="IBlock"/> for the One-Way blocks used in Jump King+
    /// </summary>
    public class OneWayBlock : BoxBlock, IBlockDebugColor
    {
        private readonly Rectangle collider;

        /// <summary>
        /// The <see cref="OneWayType"/> of the One-Way block
        /// </summary>
        public OneWayType Type { get; private set; }

        /// <summary>
        /// We say that the player is allowed to collide with us always, we will determine
        /// the nuance of the collision during the block behaviour when we have more context
        /// </summary>
        protected override bool canBlockPlayer => false;

        public Color DebugColor => new Color(65, 65, 65);

        /// <summary>
        /// Ctor for creating a <see cref="OneWayBlock"/>
        /// </summary>
        /// <param name="p_collider">A <see cref="Rectangle"/> for the collision of the block</param>
        /// <param name="p_type">The <see cref="OneWayType"/> for the one-way block</param>
        public OneWayBlock(Rectangle p_collider, OneWayType p_type) : base(p_collider)
        {
            collider = p_collider;
            Type = p_type;
        }
    }
}
