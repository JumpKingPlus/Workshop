using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKing;

namespace JumpKingMultiplayer.Extensions
{
    public static class TextureExtension
    {
        /// <summary>
        /// WARNING: THIS IS A TERRIBLE IDEA.
        /// Turns a <see cref="Texture2D"/> into an array of <see cref="Color"/>.
        /// </summary>
        /// <param name="rect">The dimension you want to convert.</param>
        /// <returns>The array of colors which equals to the image.</returns>
        //public static Color[] ToTextureRaw(this Texture2D src, Rectangle rect, Color[] colors = null)
        //{
        //    if (src == null)
        //    {
        //        return new Color[0];
        //    }

        //    int count = rect.Width * rect.Height;
        //    Color[] data = new Color[count];
        //    src.GetData(0, rect, data, 0, count);
        //    colors = data;
        //    return data;
        //}

        /// <summary>
        /// WARNING: THIS IS A TERRIBLE IDEA.
        /// Turns an array of <see cref="Color"/> into a <see cref="Texture2D"/>.
        /// </summary>
        /// <param name="rect">The dimension you want to convert.</param>
        /// <returns>The Texture2D from the array of colors.</returns>
        //public static Texture2D ToTexture2D(this Color[] src, Rectangle rect)
        //{
        //    Texture2D tex = new Texture2D(Game1.instance.GraphicsDevice, rect.Width, rect.Height);
        //    tex.SetData(src);
        //    return tex;
        //}
    }
}
