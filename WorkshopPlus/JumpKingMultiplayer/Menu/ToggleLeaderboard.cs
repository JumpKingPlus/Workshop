using JumpKing.PauseMenu.BT.Actions;
using JumpKingMultiplayer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer.Menu
{
    public static class Extensions
    {
        static public string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field == null)
                return enumValue.ToString();

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }

            return enumValue.ToString();
        }
    }

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

    public class ToggleProximityPlayers : ITextToggle
    {
        public ToggleProximityPlayers() 
            : base(ModEntry.Preferences.IsProximityPlayersEnabled)
        {
        }

        protected override string GetName() => "Display Proximity Players";

        protected override void OnToggle()
        {
            ModEntry.Preferences.IsProximityPlayersEnabled = !ModEntry.Preferences.IsProximityPlayersEnabled;
        }
    }

    public class GhostPlayerDisplayOption : IOptions
    {
        public GhostPlayerDisplayOption()
            : base(Enum.GetNames(typeof(EGhostPlayerDisplayType)).Length, (int)ModEntry.Preferences.GhostPlayerDisplayType, EdgeMode.Clamp)
        {
        }

        protected override bool CanChange() => true;

        protected override string CurrentOptionName()
        {
            return ((EGhostPlayerDisplayType)CurrentOption).GetDescription();
        }

        protected override void OnOptionChange(int option)
        {
            ModEntry.Preferences.GhostPlayerDisplayType = (EGhostPlayerDisplayType)option;
        }
    }

    public class GhostPlayerOpacityOption : IOptions
    {
        public GhostPlayerOpacityOption() 
            : base(11, (int)(ModEntry.Preferences.GhostPlayerOpacity * 10), EdgeMode.Clamp)
        {
        }

        protected override bool CanChange() => true;

        protected override string CurrentOptionName()
        {
            return $"Ghost opacity: {CurrentOption * 10}%";
        }

        protected override void OnOptionChange(int option)
        {
            ModEntry.Preferences.GhostPlayerOpacity = option * 0.1f;
        }
    }
}
