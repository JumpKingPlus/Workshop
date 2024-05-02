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
    /// An extension of <see cref="WarpBlock"/> to support the legacy behaviour present in JK+ before v1.8.2
    /// Functions the same as <see cref="WarpBlock"/> with the main difference being in the accompanying behaviour
    /// </summary>
    public class LegacyWarpBlock : WarpBlock
    {
        /// <summary>
        /// Ctor for creating a <see cref="LegacyWarpBlock"/>
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
        public LegacyWarpBlock(Rectangle p_collider, int p_rawX, int p_rawY) : base(p_collider, p_rawX, p_rawY)
        {
        }
    }
}
