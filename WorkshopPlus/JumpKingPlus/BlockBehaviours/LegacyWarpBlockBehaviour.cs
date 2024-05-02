using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using JumpKing.Player;
using JumpKingPlus.Blocks;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlus.BlockBehaviours
{
    /// <summary>
    /// An implementation of <see cref="IBlockBehaviour"/> for how the player interacts with the <see cref="LegacyWarpBlock"/>
    /// present in JK+ prior to v1.8.2
    /// </summary>
    public class LegacyWarpBlockBehaviour : IBlockBehaviour
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
            //Debug.WriteLine($"[IS_NULL] PreResolution: {behaviourContext.CollisionInfo.PreResolutionCollisionInfo is null} - StartOfFrame: {behaviourContext.CollisionInfo.StartOfFrameCollisionInfo is null}");
            if (behaviourContext?.CollisionInfo?.StartOfFrameCollisionInfo != null)
            {
                IsPlayerOnBlock = behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<LegacyWarpBlock>();
                //var current = behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<LegacyWarpBlock>();
                //Debug.WriteLine($"[IS_COLLIDING] PreResolution: {IsPlayerOnBlock} - StartOfFrame: {current}");
                if (IsPlayerOnBlock)
                {
                    BodyComp bodyComp = behaviourContext.BodyComp;
                    //Debug.WriteLine($"[BEFORE] PosX: {bodyComp.Position.X} - PosY: {bodyComp.Position.Y}");

                    foreach (WarpBlock item in behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.GetCollidedBlocks<LegacyWarpBlock>())
                    {
                        // This code contains some issues and doesn't function correctly
                        // it was eventually removed in JK+ v1.8.2 but is needed here for 
                        // legacy support
                        if (bodyComp.Velocity.X > 0f || bodyComp.LastVelocity.X > 0f)
                        {
                            bodyComp.Position.X = (float)item.X + (float)(480 * ((Camera.CurrentScreen - 1) / 13)) + 8f;
                            bodyComp.Position.Y = (float)item.Y - (float)(360 * (Camera.CurrentScreen % 13)) - 16f;
                            //Debug.WriteLine($"[AFTER] [>0] PosX: {bodyComp.Position.X} - PosY: {bodyComp.Position.Y}");
                        }
                        else if (bodyComp.Velocity.X < 0f || bodyComp.LastVelocity.X < 0f)
                        {
                            bodyComp.Position.X = (float)item.X + (float)(480 * ((Camera.CurrentScreen - 1) / 13)) - 16f;
                            bodyComp.Position.Y = (float)item.Y - (float)(360 * (Camera.CurrentScreen % 13)) - 16f;
                            //Debug.WriteLine($"[AFTER] [<0] PosX: {bodyComp.Position.X} - PosY: {bodyComp.Position.Y}");
                        }
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
