using HarmonyLib;
using JumpKing;
using JumpKing.Player;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BehaviorTree;

namespace JumpKingAdditionalAnimations
{
    public class SplatAddedFrames : IAddedFrames
    {
        public Point FrameSize => new Point(48, 48);

        public SplatAddedFrames(Harmony harmony)
        {
            // temporary solution is to load the texture in local
            // later on: read each item, if it has a XML file on root (splat_extra.xml idk name yet)
            AssignSprites();

            // override splat sprite
            // Game1.instance.contentManager.playerSprites.splat
            // later on move to IKingSpriteGroup.m_key_sprites
            try
            {
                // does not work, gets inlined
                //harmony.Patch(
                //    AccessTools.Method("JumpKing.JKContentManager.PlayerSprites:get_splat"),
                //    postfix: new HarmonyMethod(typeof(SplatAddedFrames).GetMethod(nameof(PatchSplat)))
                //);
                harmony.Patch(
                    AccessTools.Method("JumpKing.Player.FailState:SetSprite"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(SplatAddedFrames), nameof(SetSpritePatch)))
                );

                // change splat sprite
                // FailState.UpdateTimer(float p_delta)
                harmony.Patch(
                    AccessTools.Method("JumpKing.Player.FailState:MyRun"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(SplatAddedFrames), nameof(PostRun)))
                );

                // reset splat sprite
                // FailState.Reset()
                // later on: move to IKingSpriteGroup.m_key_sprites or figure out how skins does it
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
            string texture_path = Path.Combine(mod_path, "Slap");
            Texture2D texture = Game1.instance.contentManager.Load<Texture2D>(texture_path);


            if (texture.Width % FrameSize.X != 0 || texture.Height % FrameSize.Y != 0)
            {
                // something aint right
                throw new Exception($"Invalid image size for {nameof(SplatAddedFrames)}!");
            }

            int columns = texture.Width / FrameSize.X;
            int rows = texture.Height / FrameSize.Y;
            Point cells = new Point(columns, rows);

            Sprites = JKUtils.SpriteChopUtilGrid(texture, cells, new Vector2(0.5f, 1f), texture.Bounds).ToList();
            SpriteIndex = 0;

            //var x = AccessTools.Method(
            //    AccessTools.TypeByName("JumpKing.JKContentManager.Util"),
            //    "SpriteChopUtilGrid"
            //);
            //var y = x;
            // JumpKing.JKContentManager.Util.SpriteChopUtilGrid(Texture2D texture, Point cells)
            // todo: needed row and columns
            //var scug_method = AccessTools.Method("JumpKing.JKContentManager.Util:SpriteChopUtilGrid");
            //try
            //{
            //    var sprites = scug_method.Invoke(null, new object[] {  });
            //    Sprites = ((Sprite[])sprites).ToList();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public static void SetSpritePatch(FailState __instance)
        {
            __instance.player.SetSprite(ModEntry.Splat.Sprites[ModEntry.Splat.SpriteIndex]);
        }

        public int SpriteIndex { get; set; }
        private static float deltaSum;

        public static void PostRun(FailState __instance, BTresult __result, TickData p_data)
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

            if (ModEntry.Splat.SpriteIndex == ModEntry.Splat.Sprites.Count - 1)
            {
                ModEntry.Splat.SpriteIndex = 0;
            }
            else
            {
                ModEntry.Splat.SpriteIndex++;
            }
        }
    }
}
