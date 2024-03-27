using BehaviorTree;
using JumpKing.PauseMenu.BT.Actions;
using JumpKingMultiplayer;
using JumpKingMultiplayer.Models;

namespace JumpKingMultiplayer.Menu.DisplayOptions
{
    public class PersonalColorChangeOption : IOptions
    {
        public PersonalColorChangeOption()
            : base(GhostPlayer.ColorList.Length, ModEntry.Preferences.LobbySettings.PersonalColor, EdgeMode.Clamp)
        {
        }

        protected override bool CanChange() => true;

        protected override string CurrentOptionName() => CurrentOption == 0 ? "Automatic" : GhostPlayer.ColorList[CurrentOption].ToString();

        protected override void OnOptionChange(int option)
        {
            // struct moment
            var lobby = ModEntry.Preferences.LobbySettings;
            lobby.PersonalColor = option;
            ModEntry.Preferences.LobbySettings = lobby;
        }
    }
}
