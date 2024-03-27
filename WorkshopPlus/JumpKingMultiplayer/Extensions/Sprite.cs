using JumpKing;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using JumpKingMultiplayer.Extensions;
using JumpKingMultiplayer.Models;

namespace JumpKingMultiplayer.Extensions
{
    static class SpriteExtensions
    {
        public static void Draw(this Sprite sprite, Vector2 position, SpriteEffects spriteEffects, Color? c_override = null)
        {
            var layeredSpriteType = AccessTools.TypeByName("JumpKing.XnaWrappers.LayeredSprite");
            if (sprite.GetType() == layeredSpriteType)
            {
                foreach (Sprite spr in Traverse.Create(sprite).Property("Sprites").GetValue<List<Sprite>>())
                {
                    spr.Draw(position, spriteEffects, c_override);
                }
                return;
            }

            if (sprite.texture == null)
            {
                return;
            }
            var tSprite = Traverse.Create(sprite);
            var m_color = tSprite.Field("m_color").GetValue<Color>();

            position -= sprite.source.Size.ToVector2() * sprite.center;
            position = position.ToPoint().ToVector2();
            Game1.spriteBatch.Draw(sprite.texture, position, new Rectangle?(sprite.source), c_override ?? m_color, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
        }
    }
}
