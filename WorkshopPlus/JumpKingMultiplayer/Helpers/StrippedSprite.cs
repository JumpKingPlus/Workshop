using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer.Helpers
{
    public class StrippedSprite
    {
        public Color[] TextureRaw;
        public Rectangle Source;

        public StrippedSprite(Color[] textureRaw, Rectangle source)
        {
            TextureRaw = textureRaw;
            Source = source;
        }
    }
}
