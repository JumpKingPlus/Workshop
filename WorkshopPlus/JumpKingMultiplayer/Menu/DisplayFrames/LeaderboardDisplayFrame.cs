using BehaviorTree;
using JumpKing;
using JumpKing.PauseMenu;
using Microsoft.Xna.Framework;
using JumpKingMultiplayer.Helpers;

namespace JumpKingMultiplayer.Menu.DisplayFrames
{
    public class LeaderboardDisplayFrame : DynamicDisplayFrame
    {
        public LeaderboardDisplayFrame(GuiFormat p_format, BTresult p_static_result, float p_alpha)
            : base(p_format, p_static_result, p_alpha)
        {
        }

        public new void Draw()
        {
            base.Draw();
            Game1.spriteBatch.Draw(
                ModEntry.LeaderboardHeader,
                position: new Vector2(
                    (JumpGame.GAME_RECT.Width / 2) - (ModEntry.LeaderboardHeader.Width / 2), // horizontally centered
                    Bounds.Y - ModEntry.LeaderboardHeader.Height), // vertically on top of the gui frame
                Color.White * alpha);
        }
    }
}
