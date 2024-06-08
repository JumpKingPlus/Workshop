using JumpKing.PauseMenu.BT.Actions;

namespace JumpKingLastJumpValue.Menu
{
    public class ToggleGaugeDisplay : ITextToggle
    {
        public ToggleGaugeDisplay() : base(JumpKingLastJumpValue.Preferences.ShowGauge)
        {
        }

        protected override string GetName() => "Show Gauge";

        protected override void OnToggle()
        {
            JumpKingLastJumpValue.Preferences.ShowGauge = !JumpKingLastJumpValue.Preferences.ShowGauge;
        }
    }
}
