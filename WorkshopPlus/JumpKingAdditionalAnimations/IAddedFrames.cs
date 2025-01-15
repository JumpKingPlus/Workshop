using JumpKing;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingAdditionalAnimations
{
    internal interface IAddedFrames
    {
        void AssignSprites();
        
        Point FrameSize { get; }

        List<Sprite> Sprites { get; }

        int SpriteIndex { get; }
    }
}
