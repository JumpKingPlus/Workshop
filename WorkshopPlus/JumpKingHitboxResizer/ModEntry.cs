using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.Level;
using JumpKing.Mods;
using JumpKing.Player;
using JumpKing.Workshop;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace HitboxResizer
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "Phoenixx19.HitboxResizer";
        const string HARMONY_IDENTIFIER = "Phoenixx19.HitboxResizer.Harmony";

        const string FLAG_REGEX = "^KingCustomHitbox(Width|Height)=[0-9]+$";
        internal static Regex HitboxResizerRegEx = new Regex(FLAG_REGEX);

        public static int Width { get; set; } = PlayerValues.PLAYER_WIDTH;
        public static int Height { get; set; } = PlayerValues.PLAYER_HEIGHT;

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HARMONY_IDENTIFIER);

#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif

            harmony.Patch(
                AccessTools.Method(typeof(PlayerEntity), "Draw"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(ModEntry), nameof(DrawOverride)))
            );
        }

        static bool DrawOverride(PlayerEntity __instance)
        {
            var center = __instance.m_body.Position + new Vector2(Width / 2, Height);

            var tInstance = Traverse.Create(__instance);
            tInstance.Field("m_sprite").GetValue<Sprite>()
                .Draw(
                    Camera.TransformVector2(center), 
                    tInstance.Field("m_flip").GetValue<SpriteEffects>()
                );

            if (LevelScreen.DrawDebug)
            {
                Game1.spriteBatch.Draw(
                    Game1.instance.contentManager.Pixel.texture,
                    new Rectangle(
                        Camera.TransformVector2(new Vector2(__instance.m_body.Position.X, __instance.m_body.Position.Y)).ToPoint(),
                        new Vector2(Width, Height).ToPoint()
                    ),
                    new Color(Color.DarkKhaki, 0.66f)
                );

                Game1.spriteBatch.Draw(
                    Game1.instance.contentManager.Pixel.texture,
                    new Rectangle(
                        Camera.TransformVector2(__instance.m_body.GetHitbox().Center.ToVector2()).ToPoint(), new Point(1)
                    ),
                    Color.Red
                );
            }
            return false;
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            // custom hitboxes time
            if (HasAnyHitboxResizerTags(out int width, out int height))
            {
                SetHitbox(width, height);
                return;
            }

            // read default hitboxes
            SetHitbox();
        }

        private static bool HasAnyHitboxResizerTags(out int width, out int height)
        {
            // fallback values
            width = PlayerValues.PLAYER_WIDTH;
            height = PlayerValues.PLAYER_HEIGHT;
            bool isDefault = true;

            // null check
            if (Game1.instance.contentManager?.level?.Info.Tags is null)
            {
                return !isDefault;
            }

            foreach (string item in Game1.instance.contentManager.level.Info.Tags)
            {
                // no match found! (most probably its a different flag)
                if (!HitboxResizerRegEx.IsMatch(item))
                {
                    continue;
                }

                // KingCustomHitboxWidth=18
                // dividing it to:
                // KingCustomHitboxWidth AND 18
                string[] strings = item.Split('=');

                // this gotta be divided in 2 and it has to be a NUMBER OF SOME SORT
                if (strings.Length != 2 || !int.TryParse(strings[1], out int value))
                {
                    // uh oh
                    continue;
                }

                if (strings[0].Contains("Width"))
                {
                    // set width
                    width = value;
                }
                else if (strings[0].Contains("Height"))
                {
                    // set height
                    height = value;
                }

                // i gotta return something as a bool for a feedback!
                isDefault = false;
            }

            return !isDefault;
        }

        private static void SetHitbox(int width = PlayerValues.PLAYER_WIDTH, int height = PlayerValues.PLAYER_HEIGHT)
        {
            Width = width;
            Height = height;

            var tBody = Traverse.Create(GameLoop.m_player.m_body);
            tBody.Field("m_width").SetValue(Width);
            tBody.Field("m_height").SetValue(Height);
        }
    }
}
