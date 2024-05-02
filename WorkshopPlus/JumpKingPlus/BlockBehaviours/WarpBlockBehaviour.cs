using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using JumpKing.Player;
using JumpKingPlus.Blocks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlus.BlockBehaviours
{ 
    /// <summary>
    /// An implementation of <see cref="IBlockBehaviour"/> for how the player interacts with the <see cref="WarpBlock"/>
    /// </summary>
    public class WarpBlockBehaviour : IBlockBehaviour
    {
        /// <inheritdoc/>
        public float BlockPriority => 2f;

        /// <inheritdoc/>
        public bool IsPlayerOnBlock { get; set; }

        /// <inheritdoc/>
        public bool AdditionalXCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool AdditionalYCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool ExecuteBlockBehaviour(BehaviourContext behaviourContext)
        {
            if (behaviourContext?.CollisionInfo?.StartOfFrameCollisionInfo != null)
            {
                IsPlayerOnBlock = behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<WarpBlock>();
                if (IsPlayerOnBlock)
                {
                    foreach (WarpBlock item in behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.GetCollidedBlocks<WarpBlock>())
                    {
                        behaviourContext.BodyComp.Position.X = item.X;
                        behaviourContext.BodyComp.Position.Y = item.Y - (LevelManager.PIXEL_HEIGHT * Camera.CurrentScreen);
                        if (!JumpKingPlusBlockFactory.Flags.Contains(JumpKingPlusBlockFactory.MuteWarpTransitionSound))
                        {
                            JumpKingPlusEntry.WarpTransition?.PlayOneShot();
                        }
                        break;
                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public float ModifyGravity(float inputGravity, BehaviourContext behaviourContext)
        {
            return inputGravity;
        }

        /// <inheritdoc/>
        public float ModifyXVelocity(float inputXVelocity, BehaviourContext behaviourContext)
        {
            return inputXVelocity;
        }

        /// <inheritdoc/>
        public float ModifyYVelocity(float inputYVelocity, BehaviourContext behaviourContext)
        {
            return inputYVelocity;
        }
    }
}
