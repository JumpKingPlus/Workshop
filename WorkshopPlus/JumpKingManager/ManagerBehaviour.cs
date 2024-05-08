using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.GameManager;
using JumpKing.Player;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingManager
{
    internal class ManagerBehaviour : IBodyCompBehaviour
    {
        public bool ExecuteBehaviour(BehaviourContext behaviourContext)
        {
            BodyComp = behaviourContext.BodyComp;
            return true;
        }

        public BodyComp BodyComp { get; set; }

        public float PlayerX() => BodyComp.Position.X;
        public float PlayerY() => BodyComp.Position.Y;

        internal Screen PlayerScreen()
        {
            Enum.TryParse(Camera.CurrentScreen.ToString(), out Screen result);
            return result;
        }

        internal void TeleportPlayer(Screen screen, float x, float y)
        {
            BodyComp.Position.X = x;
            BodyComp.Position.Y = y;
            BodyComp.Velocity = Vector2.Zero;
            Camera.UpdateCamera(BodyComp.GetHitbox().Center);
        }
    }
}
