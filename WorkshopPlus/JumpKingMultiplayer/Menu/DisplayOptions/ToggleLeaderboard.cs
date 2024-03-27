using JumpKing.PauseMenu.BT.Actions;
using JumpKingMultiplayer.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKingMultiplayer.Menu;

namespace JumpKingMultiplayer.Menu.DisplayOptions
{
    public class ToggleLeaderboard : ITextToggle
    {
        public ToggleLeaderboard() : base(ModEntry.Preferences.IsLeaderboardEnabled)
        {
        }

        protected override string GetName() => "Display Leaderboard";

        protected override void OnToggle()
        {
            ModEntry.Preferences.IsLeaderboardEnabled = !ModEntry.Preferences.IsLeaderboardEnabled;
        }
    }
}
