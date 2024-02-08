using HarmonyLib;
using JumpKing.Mods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingInstAchievements
{

    [JumpKingMod(IDENTIFIER)]
    public static class JumpKingInstAchievements
    {
        const string IDENTIFIER = "Phoenixx19.InstAchievements";
        const string HARMONY_IDENTIFIER = "Phoenixx19.InstAchievements.Harmony";

        [BeforeLevelLoad]
        public static void OnLevelStart()
        {
#if DEBUG
            Debugger.Launch();
#endif

            // setup harmony
            var harmony = new Harmony(HARMONY_IDENTIFIER);

            // get achievement register method
            var achievement = AccessTools.Method(
                "JumpKing.MiscSystems.Achievements.AchievementRegister:RegisterAchievement"
            );

            // get method
            var postfix = typeof(JumpKingInstAchievements).GetMethod(nameof(FixAchievements));

            // do the patching
            harmony.Patch(achievement, postfix: new HarmonyMethod(postfix));
        }

        public static void FixAchievements()
        {
            // instant achievement
            Steamworks.SteamUserStats.StoreStats();
        }
    }
}
