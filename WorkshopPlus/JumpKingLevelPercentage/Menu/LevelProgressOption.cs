using JumpKing.PauseMenu.BT.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLevelPercentage.Menu
{
    public class LevelProgressOption : IOptions
    {
        public LevelProgressOption() 
            : base(Enum.GetNames(typeof(ELevelPercentageDisplayType)).Length, (int)ELevelPercentageDisplayType.Percentage, EdgeMode.Wrap)
        {
            CurrentOption = (int)JumpKingLevelPercentage.Preferences.DisplayType;
        }

        protected override bool CanChange() => JumpKingLevelPercentage.Preferences.IsEnabled;

        protected override string CurrentOptionName() => ((ELevelPercentageDisplayType)CurrentOption).ToString();

        protected override void OnOptionChange(int option)
        {
            JumpKingLevelPercentage.Preferences.DisplayType = (ELevelPercentageDisplayType)option;
        }
    }
}
