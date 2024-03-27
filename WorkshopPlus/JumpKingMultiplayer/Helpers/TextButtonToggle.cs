using JumpKing;
using JumpKing.PauseMenu.BT.Actions;
using JumpKing.Util;
using Microsoft.Xna.Framework;
using JumpKingMultiplayer.Menu;

namespace JumpKingMultiplayer.Helpers
{
    public abstract class TextButtonToggle : ITextToggle
    {
        protected TextButtonToggle(bool p_start_value) : base(p_start_value)
        {
        }

        public override void Draw(int x, int y, bool selected)
        {
            TextHelper.DrawString(
                Game1.instance.contentManager.font.MenuFont,
                GetName(),
                new Vector2(x, y),
                CanChange() ? Color.White : Color.Gray,
                new Vector2(0f, 0f));
        }

        public override Point GetSize()
        {
            return Game1.instance.contentManager.font.MenuFont.MeasureString(GetName()).ToPoint();
        }
    }
}
