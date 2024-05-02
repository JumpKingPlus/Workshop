using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using JumpKing.Player;
using JumpKingPlus.Blocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlus.BlockBehaviours
{
    /// <summary>
    /// An implementation of <see cref="IBlockBehaviour"/> representing how the <see cref="OneWayBlock"/> will
    /// affect the player during runtime
    /// </summary>
    internal class OneWayBlockBehaviour : IBlockBehaviour
    {
        /// <inheritdoc/>
        public float BlockPriority => 2f;

        /// <inheritdoc/>
        public bool IsPlayerOnBlock { get; set; }

        /// <summary>
        /// Any additional block-specific collision checks to be performed during the player's
        /// X collision resolution. Prevents the player moving the incorrect way through a one-way block
        /// </summary>
        /// <param name="info">Information about the current collision state of the player</param>
        /// <param name="behaviourContext">An instance of <see cref="BehaviourContext"/> containing references to the player</param>
        /// <returns>
        /// <c>true</c> if the player is still colliding with this block in the X axis, and the collision
        /// resolution steps should continue, <c>false</c> if the player is not colliding with this
        /// block
        /// </returns>
        public bool AdditionalXCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            if (info != null && behaviourContext != null)
            {
                BodyComp bodyComp = behaviourContext.BodyComp;
                foreach (IBlock block in info.GetCollidedBlocks<OneWayBlock>())
                {
                    if (block is OneWayBlock oneWayBlock)
                    {
                        if (oneWayBlock.Type == OneWayType.Right)
                        {
                            return info.IsCollidingWith<OneWayBlock>() &&
                                !behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<OneWayBlock>()
                                && bodyComp.Velocity.X > 0f;
                        }
                        if (oneWayBlock.Type == OneWayType.Left)
                        {
                            return info.IsCollidingWith<OneWayBlock>() &&
                                !behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<OneWayBlock>()
                                && bodyComp.Velocity.X < 0f;
                        }

                        Debug.WriteLine($"{oneWayBlock.Type} ruined everything, boo hoo!");
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Any additional block-specific collision checks to be performed during the player's
        /// Y collision resolution. Prevents the player moving the incorrect way through a one-way block
        /// </summary>
        /// <param name="info">Information about the current collision state of the player</param>
        /// <param name="behaviourContext">An instance of <see cref="BehaviourContext"/> containing references to the player</param>
        /// <returns>
        /// <c>true</c> if the player is still colliding with this block in the Y axis, and the collision
        /// resolution steps should continue, <c>false</c> if the player is not colliding with this
        /// block
        /// </returns>
        public bool AdditionalYCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            if (info != null && behaviourContext != null)
            {
                BodyComp bodyComp = behaviourContext.BodyComp;
                foreach (IBlock block in info.GetCollidedBlocks<OneWayBlock>())
                {
                    if (block is OneWayBlock oneWayBlock)
                    {
                        if (oneWayBlock.Type == OneWayType.Top)
                        {
                            return info.IsCollidingWith<OneWayBlock>() &&
                                !behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<OneWayBlock>()
                                && bodyComp.Velocity.Y > 0f;
                        }
                        if (oneWayBlock.Type == OneWayType.Bottom)
                        {
                            return info.IsCollidingWith<OneWayBlock>() &&
                                !behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<OneWayBlock>()
                                && bodyComp.Velocity.Y < 0f;
                        }
                        Debug.WriteLine($"{oneWayBlock.Type} ruined everything, boo hoo!");
                    }
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool ExecuteBlockBehaviour(BehaviourContext behaviourContext)
        {
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
