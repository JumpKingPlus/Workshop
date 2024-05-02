using BehaviorTree;
using EntityComponent.BT;
using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using JumpKing.MiscEntities.WorldItems;
using JumpKing.MiscEntities.WorldItems.Inventory;
using JumpKing.Player;
using JumpKingPlus.Blocks;
using JumpKingPlus.Nodes;
using Microsoft.Xna.Framework;

namespace JumpKingPlus.BlockBehaviours
{
    /// <summary>
    /// An implementation of <see cref="IBlockBehaviour"/> for handling how the player interacts
    /// with <see cref="ThinSnowBlock"/>
    /// </summary>
    public class ThinSnowBlockBehaviour : IBlockBehaviour
    {
        /// <inheritdoc/>
        public float BlockPriority => 1f;

        /// <summary>
        /// Cached state of whether the player is currently on this block or not
        /// Factors in usage of the <see cref="Items.SnakeRing"/>
        /// </summary>
        public bool IsPlayerOnBlock
        {
            get
            {
                if (InventoryManager.HasItemEnabled(Items.SnakeRing))
                {
                    return false;
                }
                return isPlayerOnBlock;
            }
            set
            {
                isPlayerOnBlock = value;
            }
        }
        private bool isPlayerOnBlock;

        private readonly PlayerEntity player;

        /// <summary>
        /// Ctor for creating a <see cref="ThinSnowBlockBehaviour"/>
        /// </summary>
        public ThinSnowBlockBehaviour(PlayerEntity player)
        {
            this.player = player ?? throw new System.ArgumentNullException(nameof(player));

            // Register our own node into the behaviour tree to handle animation
            // When we want to cancel player movement, we want to also stop their animation
            // The Walk & WalkAnim nodes occur after all our behaviours, resetting velocity and applying animation
            // for the next frame
            IBTcomposite walkSequencor = player.GetComponent<BehaviorTreeComp>()?.GetRaw().FindParentNodeOf<WalkAnim>() as IBTcomposite;
            WalkAnim walkAnimNode = player.GetComponent<BehaviorTreeComp>()?.GetRaw().FindNode<WalkAnim>();
            if (walkSequencor != null && walkAnimNode != null)
            {
                walkSequencor.AddChild(new ThinSnowWalkAnimReset(walkAnimNode, player));
            }
        }

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
            if (behaviourContext?.CollisionInfo?.PreResolutionCollisionInfo != null)
            {
                IsPlayerOnBlock = behaviourContext.CollisionInfo.PreResolutionCollisionInfo.IsCollidingWith<ThinSnowBlock>();
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
            // Velocity application is handled early in the frame, meaning we can't rely on
            // collision data from this frame to see if we have intersected with a ThinSnowBlock, 
            // we must check the previous frame
            if (behaviourContext?.LastFrameCollisionInfo?.PreResolutionCollisionInfo != null)
            {
                // Player can't walk when on Thin Snow so we set the X velocity to 0
                IsPlayerOnBlock = behaviourContext.LastFrameCollisionInfo.PreResolutionCollisionInfo.IsCollidingWith<ThinSnowBlock>();
                if (IsPlayerOnBlock)
                {
                    // Reset the sprite to idle
                    return 0;
                }
            }
            return inputXVelocity;
        }

        /// <inheritdoc/>
        public float ModifyYVelocity(float inputYVelocity, BehaviourContext behaviourContext)
        {
            return inputYVelocity;
        }
    }
}
