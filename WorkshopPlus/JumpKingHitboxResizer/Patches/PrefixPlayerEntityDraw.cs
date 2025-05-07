using HarmonyLib;
using JumpKing;
using JumpKing.Level;
using JumpKing.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HitboxResizer.Patches
{
    [HarmonyPatch(typeof(PlayerEntity), "Draw")]
    public class PrefixPlayerEntityDraw
    {
        public static int Width { get; set; } = PlayerValues.PLAYER_WIDTH;
        public static int Height { get; set; } = PlayerValues.PLAYER_HEIGHT;
        public static bool IsCustomHitbox { get; set; } = false;

        public static bool Prefix(PlayerEntity __instance)
        {
            if (!IsCustomHitbox)
            {
                // If we don't have any custom hitbox allow the original method.
                // For full compatibility with the mod that flips the player
                // upside down this will have to be a transpiler.
                return true;
            }

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
    }
}
