using JumpKing.API;
using JumpKing.Level;
using JumpKing.Level.Sampler;
using JumpKing.Workshop;
using JumpKingPlus.Blocks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlus
{
    /// <summary>
    /// An implementation of <see cref="IBlockFactory"/> for creating blocks used by JumpKing+
    /// </summary>
    public class JumpKingPlusBlockFactory : IBlockFactory
    {
        private static readonly Color BLOCKCODE_LOWGRAVITY = new Color(128, 255, 255);
        private static readonly Color BLOCKCODE_THINSNOW = new Color(255, 255, 128);

        // Warp blocks have additional logic in the R and G channels
        // warp goes from rgb(0,0,75) to (60,45,75)
        private static readonly int BLOCKCODE_WARPBLOCK_B = 75;
        private static readonly int BLOCKCODE_WARPBLOCK_R_MAX = 61;
        private static readonly int BLOCKCODE_WARPBLOCK_G_MAX = 46;

        // One way blocks have some variance in the Red channel based on their direction
        private static readonly Color BLOCKCODE_ONEWAY_TOP = new Color(65, 65, 65);
        private static readonly Color BLOCKCODE_ONEWAY_RIGHT = new Color(66, 65, 65);
        private static readonly Color BLOCKCODE_ONEWAY_BOTTOM = new Color(67, 65, 65);
        private static readonly Color BLOCKCODE_ONEWAY_LEFT = new Color(68, 65, 65);

        /// <summary>
        /// If present in <see cref="JumpKing.Workshop.Level.LevelSettings.Tags"/>, then the <see cref="LegacyWarpBlock"/> will be used
        /// instead of <see cref="WarpBlock"/>.
        /// </summary>
        public const string UseLegacyWarpBlockTag = "UseLegacyWarpBlock";

        /// <summary>
        /// If present in <see cref="JumpKing.Workshop.Level.LevelSettings.Tags"/>, then the <see cref="JumpKingPlusEntry.WarpTransition"/> sound effect
        /// will not be played.
        /// </summary>
        public const string MuteWarpTransitionSound = "MuteWarpTransitionSound";

        /// <summary>
        /// If present in <see cref="JumpKing.Workshop.Level.LevelSettings.Tags"/>, then the JumpKingPlus' walk speed on Low Gravity will be activated.
        /// </summary>
        public const string LegacyWalkSpeedInLowGravity = "LegacyWalkSpeedInLowGravity";

        /// <summary>
        /// Level flags.
        /// </summary>
        internal static string[] Flags = new string[0];

        private readonly HashSet<Color> supportedBlockCodes = new HashSet<Color>()
        {
            BLOCKCODE_LOWGRAVITY,
            BLOCKCODE_THINSNOW,
            BLOCKCODE_ONEWAY_TOP,
            BLOCKCODE_ONEWAY_RIGHT,
            BLOCKCODE_ONEWAY_BOTTOM,
            BLOCKCODE_ONEWAY_LEFT
        };

        public bool CanMakeBlock(Color blockCode, Level level)
        {
            return supportedBlockCodes.Contains(blockCode) ||
                            (blockCode.B == BLOCKCODE_WARPBLOCK_B &&
                             blockCode.R < BLOCKCODE_WARPBLOCK_R_MAX &&
                             blockCode.G < BLOCKCODE_WARPBLOCK_G_MAX);
        }

        public IBlock GetBlock(Color blockCode, Rectangle blockRect, JumpKing.Workshop.Level level, LevelTexture textureSrc, int currentScreen, int x, int y)
        {
            if (level?.Info.Tags != null)
            {
                Flags = level.Info.Tags;
            }
            if (blockCode == BLOCKCODE_LOWGRAVITY)
            {
                return new LowGravityBlock(blockRect);
            }
            else if (blockCode == BLOCKCODE_THINSNOW)
            {
                return new ThinSnowBlock(blockRect);
            }
            else if (blockCode.B == BLOCKCODE_WARPBLOCK_B && blockCode.R < BLOCKCODE_WARPBLOCK_R_MAX && blockCode.G < BLOCKCODE_WARPBLOCK_G_MAX)
            {
                // If the flag is present for legacy warp blocks, use that instead
                if (Flags.Contains(UseLegacyWarpBlockTag))
                {
                    return new LegacyWarpBlock(blockRect, (int)blockCode.R, (int)blockCode.G);
                }
                else
                {
                    return new WarpBlock(blockRect, (int)blockCode.R, (int)blockCode.G);
                }
            }
            else if (blockCode == BLOCKCODE_ONEWAY_TOP)
            {
                return new OneWayBlock(blockRect, OneWayType.Top);
            }
            else if (blockCode == BLOCKCODE_ONEWAY_RIGHT)
            {
                return new OneWayBlock(blockRect, OneWayType.Right);
            }
            else if (blockCode == BLOCKCODE_ONEWAY_BOTTOM)
            {
                return new OneWayBlock(blockRect, OneWayType.Bottom);
            }
            else if (blockCode == BLOCKCODE_ONEWAY_LEFT)
            {
                return new OneWayBlock(blockRect, OneWayType.Left);
            }
            else
            {
                // If we get to here then we have ran out of possible blocks
                throw new InvalidOperationException($"{typeof(JumpKingPlusBlockFactory).Name} is unable to create a block of Color code ({blockCode.R}, {blockCode.G}, {blockCode.B})");
            }
        }

        public bool IsSolidBlock(Color blockCode)
        {
            if (blockCode == BLOCKCODE_LOWGRAVITY ||
                blockCode == BLOCKCODE_ONEWAY_BOTTOM ||
                blockCode == BLOCKCODE_ONEWAY_LEFT ||
                blockCode == BLOCKCODE_ONEWAY_RIGHT ||
                blockCode == BLOCKCODE_ONEWAY_TOP ||
                (blockCode.B == BLOCKCODE_WARPBLOCK_B &&
                 blockCode.R < BLOCKCODE_WARPBLOCK_R_MAX &&
                 blockCode.G < BLOCKCODE_WARPBLOCK_G_MAX))
            {
                return false;
            }
            else if (blockCode == BLOCKCODE_THINSNOW)
            {
                return true;
            }
            else
            {
                // Not a known block
                return false;
            }
        }
    }
}
