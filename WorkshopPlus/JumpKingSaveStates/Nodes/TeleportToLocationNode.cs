using BehaviorTree;
using JumpKing;
using JumpKing.GameManager;
using Microsoft.Xna.Framework;

namespace JumpKingSaveStates.Nodes
{
    internal class TeleportToLocationNode : IBTnode
    {
        private float positionX;
        private float positionY;

        public TeleportToLocationNode(float positionX, float positionY)
        {
            this.positionX = positionX;
            this.positionY = positionY;
        }
        protected override BTresult MyRun(TickData p_data)
        {
            GameLoop.m_player.m_body.Position.X = positionX;
            GameLoop.m_player.m_body.Position.Y = positionY;
            GameLoop.m_player.m_body.Velocity = Vector2.Zero;
            Camera.UpdateCamera(GameLoop.m_player.m_body.GetHitbox().Center);
            Game1.instance.contentManager.audio.menu.Select.Play();
            return BTresult.Success;
        }
    }
}
