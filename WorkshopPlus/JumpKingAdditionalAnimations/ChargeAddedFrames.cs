using BehaviorTree;
using HarmonyLib;
using JumpKing;
using JumpKing.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JumpKingAdditionalAnimations
{
    /// <summary>
    /// See <see cref="SplatAddedFrames"/> for comments
    /// </summary>
    public class ChargeAddedFrames : IAddedFrames
    {
        public Point FrameSize => new Point(48, 48);

        public ChargeAddedFrames(Harmony harmony)
        {
            AssignSprites();

            try
            {
                harmony.Patch(
                    AccessTools.Method("JumpKing.Player.JumpState:Start"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(ChargeAddedFrames), nameof(SetSpritePatch)))
                );
                harmony.Patch(
                    AccessTools.Method("JumpKing.Player.JumpState:MyRun"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(ChargeAddedFrames), nameof(PostRun)))
                );
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Sprite> Sprites { get; private set; }
        
        public void AssignSprites()
        {
            string mod_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string texture_path = Path.Combine(mod_path, "Jump_Charge");
            Texture2D texture = Game1.instance.contentManager.Load<Texture2D>(texture_path);


            if (texture.Width % FrameSize.X != 0 || texture.Height % FrameSize.Y != 0)
            {
                // something aint right
                throw new Exception($"Invalid image size for {nameof(ChargeAddedFrames)}!");
            }

            int columns = texture.Width / FrameSize.X;
            int rows = texture.Height / FrameSize.Y;
            Point cells = new Point(columns, rows);

            Sprites = JKUtils.SpriteChopUtilGrid(texture, cells, new Vector2(0.5f, 1f), texture.Bounds).ToList();
            SpriteIndex = 0;
        }

        public int SpriteIndex { get; set; }
        private static float deltaSum;

        static void SetSpritePatch(JumpState __instance)
        {
            __instance.player.SetSprite(ModEntry.Charge.Sprites[ModEntry.Charge.SpriteIndex]);
        }

        private static void PostRun(JumpState __instance, BTresult __result, TickData p_data)
        {
            if (__result != BTresult.Running)
                return;

            if (deltaSum < 0.16f)
            {
                deltaSum += p_data.delta_time;
                return;
            }

            deltaSum = 0f;
            SetSpritePatch(__instance);

            if (ModEntry.Charge.SpriteIndex == ModEntry.Charge.Sprites.Count - 1)
            {
                ModEntry.Charge.SpriteIndex = 0;
            }
            else
            {
                ModEntry.Charge.SpriteIndex++;
            }
        }
    }
}
