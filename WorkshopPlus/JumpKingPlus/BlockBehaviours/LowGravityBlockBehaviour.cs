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
    public class LowGravityBlockBehaviour : IBlockBehaviour
    {
        /// <inheritdoc/>
        public float BlockPriority => 2f;

        /// <inheritdoc/>
        public bool IsPlayerOnBlock { get; set; }

        public const float LOW_GRAV_GRAVITY_MULTIPLIER = 0.75f;
        public const float LOW_GRAV_X_MOVE_MULTIPLIER = 1.1f;
        public const float LOW_GRAV_X_MOVE_MULTIPLIER_ON_GROUND = 0.825f;
        public const float LOW_GRAV_Y_MOVE_MULTIPLIER = 1.1f;

        /// <inheritdoc/>
        public bool AdditionalXCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            // Do nothing to affect collision checks
            return false;
        }

        /// <inheritdoc/>
        public bool AdditionalYCollisionCheck(AdvCollisionInfo info, BehaviourContext behaviourContext)
        {
            // Do nothing to affect collision checks
            return false;
        }

        /// <summary>
        /// Modify the Gravity that is to be applied to the Player's Y Velocity as 
        /// part of the update step
        /// </summary>
        /// <param name="inputGravity">The current Y gravity that would be applied</param>
        /// <param name="behaviourContext">An instance of <see cref="BehaviourContext"/> containing references to the player</param>
        /// <returns>The new Gravity that will be applied to the player</returns>
        public float ModifyGravity(float inputGravity, BehaviourContext behaviourContext)
        {
            // We also need to check during the gravity modification to ensure the application of low gravity is frame perfect
            // this is needed for some of the jumps in custom maps
            if (behaviourContext?.CollisionInfo?.StartOfFrameCollisionInfo != null)
            {
                IsPlayerOnBlock = behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<LowGravityBlock>();
            }

            return inputGravity * (IsPlayerOnBlock ? LOW_GRAV_GRAVITY_MULTIPLIER : 1f);
        }

        /// <inheritdoc/>
        public float ModifyXVelocity(float inputXVelocity, BehaviourContext behaviourContext)
        {
            // We also need to check during the X modification to ensure the application of low gravity is frame perfect
            // this is needed for some of the jumps in custom maps
            if (behaviourContext?.CollisionInfo?.StartOfFrameCollisionInfo != null)
            {
                IsPlayerOnBlock = behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<LowGravityBlock>();
            }

            // Apply low grav multiplier
            float modifier = 1f;
            if (IsPlayerOnBlock)
            {
                modifier = (behaviourContext.BodyComp.IsOnGround && !JumpKingPlusBlockFactory.Flags.Contains(JumpKingPlusBlockFactory.LegacyWalkSpeedInLowGravity)) 
                    ? LOW_GRAV_X_MOVE_MULTIPLIER_ON_GROUND
                    : LOW_GRAV_X_MOVE_MULTIPLIER;
            }

            // Apply the modifier to the velocity
            return inputXVelocity * modifier;
        }

        /// <inheritdoc/>
        public float ModifyYVelocity(float inputYVelocity, BehaviourContext behaviourContext)
        {
            BodyComp bodyComp = behaviourContext.BodyComp;

            // We also need to check during the Y modification to ensure the application of low gravity is frame perfect
            // this is needed for some of the jumps in custom maps
            if (behaviourContext?.CollisionInfo?.StartOfFrameCollisionInfo != null)
            {
                IsPlayerOnBlock = behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<LowGravityBlock>();
            }

            // Apply low grav jump velocity
            float modifier = IsPlayerOnBlock ? LOW_GRAV_Y_MOVE_MULTIPLIER : 1f;

            // Apply modifier to the velocity
            float newYVelocity = inputYVelocity * modifier;

            // If we're in low grav, but we are on ground, and are moving down
            // we want to bump our position a tiny bit for reasons
            if (IsPlayerOnBlock && bodyComp.IsOnGround && bodyComp.Velocity.Y > 0)
            {
                bodyComp.Position.Y += 1;
            }

            return newYVelocity;
        }

        /// <summary>
        /// Block-specific behaviour that may or may not affect the player. Executed during the player's update step.
        /// </summary>
        /// <param name="behaviourContext">An instance of <see cref="BehaviourContext"/> containing references to the player</param>
        /// <returns>
        /// <c>true</c> if the player's update step should continue after this. <c>false</c> if it shouldn't.
        /// </returns>
        public bool ExecuteBlockBehaviour(BehaviourContext behaviourContext)
        {
            if (behaviourContext?.CollisionInfo?.StartOfFrameCollisionInfo != null)
            {
                IsPlayerOnBlock = behaviourContext.CollisionInfo.StartOfFrameCollisionInfo.IsCollidingWith<LowGravityBlock>();
            }
            return true;
        }
    }
}
