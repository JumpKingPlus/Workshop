using JumpKing.PauseMenu.BT.Actions;
using System;
using JumpKing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlayerCoordinates.Menu
{
    public class PlayerCoordinatesOption : IOptions
    {
        public PlayerCoordinatesOption()
            : base(Enum.GetNames(typeof(Coordinates)).Length, (int)Coordinates.Absolute, EdgeMode.Wrap)
        {
            CurrentOption = (int)JumpKingPlayerCoordinates.Preferences.DisplayType;
        }

        protected override bool CanChange() => JumpKingPlayerCoordinates.Preferences.IsEnabled;

        protected override string CurrentOptionName() => ((Coordinates)CurrentOption).ToString();

        protected override void OnOptionChange(int option)
        {
            JumpKingPlayerCoordinates.Preferences.DisplayType = (Coordinates)option;
        }
    }
}
