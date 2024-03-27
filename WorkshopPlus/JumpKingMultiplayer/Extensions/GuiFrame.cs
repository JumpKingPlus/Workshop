using HarmonyLib;
using JumpKing;
using JumpKing.PauseMenu;
using Microsoft.Xna.Framework;
using JumpKingMultiplayer.Menu;

namespace JumpKingMultiplayer.Extensions
{
    public static class GuiFrameExtensions
    {
        public static void Draw(this GuiFrame frame, Color color, float alpha = 1f)
        {
            var m_bounds = Traverse.Create(frame).Field("m_bounds").GetValue<Rectangle>();
            Sprite[,] frameSprites = Game1.instance.contentManager.gui.FrameSprites;
            int width = frameSprites[0, 0].source.Width;
            int num = m_bounds.Width - width * 2;
            int num2 = m_bounds.Height - width * 2;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Sprite sprite = frameSprites[i, j];
                    Rectangle destinationRectangle = new Rectangle(0, 0, width, width);
                    switch (i)
                    {
                        case 1:
                            destinationRectangle.X = width;
                            destinationRectangle.Width = num;
                            break;
                        case 2:
                            destinationRectangle.X = num + width;
                            break;
                    }

                    switch (j)
                    {
                        case 1:
                            destinationRectangle.Y = width;
                            destinationRectangle.Height = num2;
                            break;
                        case 2:
                            destinationRectangle.Y = num2 + width;
                            break;
                    }

                    destinationRectangle.X += m_bounds.X;
                    destinationRectangle.Y += m_bounds.Y;
                    Game1.spriteBatch.Draw(sprite.texture, destinationRectangle, sprite.source, color * alpha);
                }
            }
        }
    }
}
