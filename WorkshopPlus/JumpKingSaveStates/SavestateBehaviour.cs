using HarmonyLib;
using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.MiscSystems.Achievements;
using JumpKingSaveStates.Models;
using Microsoft.Xna.Framework;
using System.Linq;

namespace JumpKingSaveStates
{
    internal class SavestateBehaviour : IBodyCompBehaviour
    {
        private static Traverse TraverseAM { get; } = Traverse.Create(
            AccessTools.Field("JumpKing.MiscSystems.Achievements.AchievementManager:instance").GetValue(null));

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
                var allTimeTicks = TraverseAM
                        .Field("m_all_time_stats")
                        .GetValue<PlayerStats>()
                        ._ticks;
                var snapshotTicks = TraverseAM
                            .Field("m_snapshot")
                            .GetValue<PlayerStats>()
                            ._ticks;
                var ticks = allTimeTicks - snapshotTicks;
                if (JumpKingSaveStates.Preferences.TryAddSaveState(
                    behaviourContext.BodyComp.Position.X,
                    behaviourContext.BodyComp.Position.Y,
                    ticks
                ))
                {
                    Game1.instance.contentManager.audio.menu.MenuFail.Play();
                }
            }
            if (state.loadPos && !JumpKingSaveStates.Preferences.SaveStates.IsEmpty)
            {
                SaveState saveState = JumpKingSaveStates.Preferences.SaveStates.Last();
                // Player pos
                behaviourContext.BodyComp.Position.X = saveState.X;
                behaviourContext.BodyComp.Position.Y = saveState.Y;
                behaviourContext.BodyComp.Velocity = Vector2.Zero;
                Camera.UpdateCamera(behaviourContext.BodyComp.GetHitbox().Center);
                // Game ticks, don't set if ticks are the default 0 though.
                if (JumpKingSaveStates.Preferences.IncludeTicks
                    && saveState.Ticks > 0)
                {
                    var allTimeTicks = TraverseAM
                        .Field("m_all_time_stats")
                        .GetValue<PlayerStats>()
                        ._ticks;
                    var snapshot = TraverseAM
                                .Field("m_snapshot");
                    var stats = snapshot.GetValue<PlayerStats>();
                    stats._ticks = allTimeTicks - saveState.Ticks;
                    snapshot.SetValue(stats);
                }
                Game1.instance.contentManager.audio.menu.Select.Play();
            }
            return true;
        }
    }
}
