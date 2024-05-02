using BehaviorTree;
using JumpKing;
using JumpKing.Player;
using JumpKingPlus.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlus.Nodes
{
    public class ThinSnowWalkAnimReset : PlayerNode
    {
        private readonly WalkAnim walkAnimNode;

        public ThinSnowWalkAnimReset(WalkAnim walkAnimNode, PlayerEntity playerEntity) : base(playerEntity)
        {
            this.walkAnimNode = walkAnimNode ?? throw new ArgumentNullException(nameof(walkAnimNode));
        }

        protected override BTresult MyRun(TickData p_data)
        {
            if (GetComponent<BodyComp>().IsOnBlock(typeof(ThinSnowBlock)))
            {
                player.SetSprite(Game1.instance.contentManager.playerSprites.idle);
                walkAnimNode?.Reset();
            }
            return BTresult.Success;
        }
    }
}
