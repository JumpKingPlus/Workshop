using BehaviorTree;
using HarmonyLib;
using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using JumpKing.Player;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingJumpArch.Models
{
    [HarmonyPatch(typeof(JumpState))]
    internal class JumpChargeCalc
    {
        public JumpChargeCalc(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(JumpState), "MyRun"),
                postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Run)))
            );
        }

        private static float previous_timer { get; set; }
        internal static List<Vector2> Vectors { get; set; }
        internal static Vector2 Hitbox { get; set; }


        private static void FakeJump(float p_intensity, JumpState jumpState, out Vector2 vector2)
        {
            vector2 = Vector2.Zero;
            var dir = jumpState.input.GetState().dpad.X;
            //if (dir == 0)
            //{
            //    //throw new NotImplementedException("this should not happen, but if it does let me know");
            //}

            if (jumpState.body.IsOnBlock(typeof(SnowBlock)) && p_intensity < 0.3f)
            {
                if (p_intensity > 0.2f)
                {
                    p_intensity = 0.3f;
                }
                else
                {
                    return;
                }
            }

            float vely = PlayerValues.JUMP * p_intensity;

            vector2 = jumpState.body.Velocity;
            vector2.Y = PlayerValues.JUMP * p_intensity;
            vector2.X += dir * PlayerValues.SPEED;
            vector2.X = ErikMaths.ErikMath.Clamp(vector2.X,
                -PlayerValues.SPEED,
                PlayerValues.SPEED
            );
        }

        private static BodyComp calcComp { get; set; }

        private static void Run(TickData p_data, BTresult __result, JumpState __instance)
        {
            // not charging a jump OR the mod is not enabled
            if (__result == BTresult.Failure || !ModEntry.Preferences.IsEnabled)
            {
                return;
            }

            // getting timer for percentage
            float m_timer = (float)Traverse.Create(__instance).Field("m_timer").GetValue();

            // missing last frame because JK clears it before being able to access it
            if (__result == BTresult.Success)
            {
                m_timer = previous_timer + p_data.delta_time * __instance.body.GetMultipliers();
            }

            var mainBody = __instance.body;
            var traverse = Traverse.Create(__instance.body);

            if (calcComp == null)
            {
                Hitbox = new Vector2(
                    traverse.Field("m_width").GetValue<int>(),
                    traverse.Field("m_height").GetValue<int>()
                );

                calcComp = new BodyComp(
                    mainBody.Position,
                    (int)Hitbox.X,
                    (int)Hitbox.Y
                );

                // remove ghost bump sound and screen change
                var behaviours = Traverse.Create(calcComp).Field("m_behaviours").GetValue<LinkedList<IBodyCompBehaviour>>();
                var bump_sfx = behaviours.FirstOrDefault(x => x.GetType() == typeof(PlayBumpSFXBehaviour));
                var teleport = behaviours.FirstOrDefault(x => x.GetType() == typeof(HandlePlayerTeleportBehaviour));
                behaviours.Remove(bump_sfx);
                behaviours.Remove(teleport);
            }

            var percentage = Math.Min(1, m_timer / __instance.CHARGE_TIME);
            FakeJump(percentage, __instance, out Vector2 velocity);
            calcComp.Velocity = velocity;
            calcComp.Position = mainBody.Position;

            float delta = 1f / PlayerValues.FPS;

            Task.Run(async () =>
            {
                var vectorList = new List<Vector2>();
                vectorList.Add(calcComp.Position);

                do
                {
                    // calcComp.Update(delta);
                    BodyCompUpdate.Invoke(calcComp, new object[] { delta });

                    var pos = Camera.TransformVector2(calcComp.Position);

                    if (vectorList.Count >= 75)
                        break;

                    // saves it from looping forever
                    if (pos.X < 0 || pos.X > 480 || pos.Y < 0 || pos.Y > 360)
                        break;

                    vectorList.Add(calcComp.Position);
                }
                // while calcComp.IsOnGround ...
                while (!calcComp.IsOnGround);

                Vectors = vectorList;
            });

            // save previous value
            previous_timer = m_timer;
        }

        private static MethodInfo BodyCompUpdate
        {
            get
            {
                if (bcUpdate == null)
                {
                    bcUpdate = AccessTools.Method(typeof(BodyComp), "Update");
                }
                return bcUpdate;
            }
        }
        private static MethodInfo bcUpdate;
    }
}
