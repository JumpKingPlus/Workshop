using JumpKing;
using JumpKing.GameManager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingManager
{
    internal class Memory
    {
        internal static void TeleportPlayer(Screen screen, float x, float y)
        {
            GameLoop.m_player.m_body.Position.X = x;
            GameLoop.m_player.m_body.Position.Y = y;
            GameLoop.m_player.m_body.Velocity = Vector2.Zero;
            Camera.UpdateCamera(GameLoop.m_player.m_body.GetHitbox().Center);
        }

        internal static float PlayerX()
        {
            return GameLoop.m_player.m_body.Position.X;
        }

        internal static float PlayerY()
        {
            return GameLoop.m_player.m_body.Position.Y;
        }

        internal static Screen PlayerScreen()
        {
            Enum.TryParse(Camera.CurrentScreen.ToString(), out Screen result);
            return result;
        }
    }
}
