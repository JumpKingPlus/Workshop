using BehaviorTree;
using HarmonyLib;
using JumpKing;
using JumpKing.API;
using JumpKing.BodyCompBehaviours;
using JumpKing.Level;
using JumpKing.Player;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Run))));
        }

        private static float PreviousTimer { get; set; }
        internal static List<Vector2> Vectors { get; set; }
        internal static Vector2 Hitbox { get; set; }


        private static void FakeJump(float p_intensity, JumpState jumpState, out Vector2 vector2)
        {
            vector2 = Vector2.Zero;
            int dir = jumpState.input.GetState().dpad.X;
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

            vector2 = jumpState.body.Velocity;
            vector2.Y = PlayerValues.JUMP * p_intensity;
            vector2.X += dir * PlayerValues.SPEED;
            vector2.X = ErikMaths.ErikMath.Clamp(vector2.X,
                -PlayerValues.SPEED,
                PlayerValues.SPEED);
        }

        private static BodyComp CalcComp { get; set; }
        private const float DELTA = 1.0f / PlayerValues.FPS;

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
                m_timer = PreviousTimer + p_data.delta_time * __instance.body.GetMultipliers();
            }

            BodyComp mainBody = __instance.body;
            Traverse traverse = Traverse.Create(__instance.body);
            int width = traverse.Field("m_width").GetValue<int>();
            int height = traverse.Field("m_height").GetValue<int>();

            if (CalcComp == null)
            {
                Hitbox = new Vector2(
                    width,
                    height);

                CalcComp = new BodyComp(
                    mainBody.Position,
                    (int)Hitbox.X,
                    (int)Hitbox.Y);

                // remove ghost bump sound and screen change
                var behaviours = Traverse.Create(CalcComp).Field("m_behaviours").GetValue<LinkedList<IBodyCompBehaviour>>();
                var bumpSfx = behaviours.FirstOrDefault(x => x.GetType() == typeof(PlayBumpSFXBehaviour));
                var teleport = behaviours.FirstOrDefault(x => x.GetType() == typeof(HandlePlayerTeleportBehaviour));
                var waterSplash = behaviours.FirstOrDefault(x => x.GetType() == AccessTools.TypeByName("JumpKing.BodyCompBehaviours.WaterParticleSpawningBehaviour"));
                behaviours.Remove(bumpSfx);
                behaviours.Remove(teleport);
                behaviours.Remove(waterSplash);
            }

            float percentage = Math.Min(1.0f, m_timer / __instance.CHARGE_TIME);
            FakeJump(percentage, __instance, out Vector2 velocity);
            CalcComp.Velocity = velocity;
            CalcComp.Position = mainBody.Position;

            Task.Run(() =>
            {
                List<Vector2> vectorList = new List<Vector2>(ModEntry.LIMIT) { CalcComp.Position };

                do
                {
                    BodyCompUpdate.Invoke(CalcComp, new object[] { DELTA });

                    Vector2 pos = Camera.TransformVector2(CalcComp.Position);

                    if (vectorList.Count >= ModEntry.LIMIT)
                    {
                        break;
                    }

                    // saves it from looping forever, probably needs more adjusting to truly hit the edge of the screen
                    if (pos.X < 0 - width || pos.X > 480 + width || pos.Y < 0 - height || pos.Y > 360 + height)
                    {
                        break;
                    }

                    vectorList.Add(CalcComp.Position);
                }
                while (!CalcComp.IsOnGround);

                Vectors = vectorList;
            });

            PreviousTimer = m_timer;
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
