using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.GameManager;
using JumpKingSaveStates.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingSaveStates
{
    internal class SavestateBehaviour : IBodyCompBehaviour
    {
        public bool ExecuteBehaviour(BehaviourContext behaviourContext)
        {
            CustomPadState state = JumpKingSaveStates.PadInstance.GetPressed();
            if (state.deletePos)
            {
                if (JumpKingSaveStates.Preferences.DeleteRecentSaveState())
                {
                    JumpKingSaveStates.DeleteSave.Play();
                }
            }
            if (state.savePos)
            {
                if (JumpKingSaveStates.Preferences.TryAddSaveState(
                    behaviourContext.BodyComp.Position.X,
                    behaviourContext.BodyComp.Position.Y,
                    behaviourContext.BodyComp.LastScreen + 1
                ))
                {
                    Game1.instance.contentManager.audio.menu.MenuFail.Play();
                }
            }
            if (state.loadPos && !JumpKingSaveStates.Preferences.SaveStates.IsEmpty)
            {
                SaveState saveState = JumpKingSaveStates.Preferences.SaveStates.Last();
                behaviourContext.BodyComp.Position.X = saveState.X;
                behaviourContext.BodyComp.Position.Y = saveState.Y;
                behaviourContext.BodyComp.Velocity = Vector2.Zero;
                Camera.UpdateCamera(behaviourContext.BodyComp.GetHitbox().Center);
                Game1.instance.contentManager.audio.menu.Select.Play();
            }
            return true;
        }
    }
}
