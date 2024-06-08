using EntityComponent;
using JumpKing;
using JumpKing.Player;
using JumpKingLastJumpValue.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace JumpKingLastJumpValue.Entities
{
    public class EntityJumpGauge : Entity
    {
        private readonly BodyComp bodyComp;
        private readonly Texture2D texture;

        public EntityJumpGauge()
        {
            PlayerEntity player = EntityManager.instance.Find<PlayerEntity>();
            bodyComp = player.m_body;

            char sep = Path.DirectorySeparatorChar;
            texture = Game1.instance.contentManager.Load<Texture2D>($"{JumpKingLastJumpValue.AssemblyPath}{sep}Content{sep}gauge");
        }

        public override void Draw()
        {
            if (!JumpKingLastJumpValue.Preferences.ShowGauge)
            {
                return;
            }
            // dividing by 100 again is so stupid tbh
            float percentage = Math.Min(Math.Max(JumpChargeCalc.JumpPercentage / 100.0f, 0.0f), 100.0f);
            int absolute = (int)(texture.Height * percentage);
            Vector2 position = new Vector2(
                Math.Max(0, bodyComp.Position.X - 12),
                bodyComp.Position.Y + Camera.CurrentScreen * 360 - 14
            );
            Vector2 vectorBottom = new Vector2(
                position.X,
                position.Y + texture.Height - absolute
            );
            Rectangle rectangleBottom = new Rectangle(
                texture.Width / 2,
                texture.Height - absolute,
                texture.Width / 2,
                absolute
            );

            Game1.spriteBatch.Draw(
                        texture: texture,
                        position: vectorBottom,
                        sourceRectangle: rectangleBottom,
                        color: Color.White);
            Game1.spriteBatch.Draw(
                        texture: texture,
                        position: position,
                        sourceRectangle: new Rectangle(0, 0, texture.Width / 2, texture.Height),
                        color: Color.White);
        }
    }
}
