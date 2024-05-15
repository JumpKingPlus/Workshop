using HarmonyLib;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.GameManager.MultiEnding.NewBabePlusEnding;
using JumpKing.GameManager.MultiEnding.NormalEnding;
using JumpKing.GameManager.MultiEnding.OwlEnding;
using JumpKing.MiscSystems.LocationText;
using JumpKing.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JumpKing.JKContentManager;

namespace JumpKingLevelPercentage.Models
{
    public class GameLoopDraw
    {
        public GameLoopDraw(Harmony harmony)
        {
            harmony.Patch(
                typeof(GameLoop).GetMethod(nameof(GameLoop.Draw)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GameLoopDraw), nameof(Draw)))
            );

            // cannot patch JumpKing.MiscSystems.LocationText.LocationTextManager:SetSettingsData(LocationSettings p_settings)
            // as it gets inlined, read https://harmony.pardeike.net/articles/intro.html#limits-of-runtime-patching
            harmony.Patch(
                AccessTools.Method(typeof(GameLoop), nameof(GameLoop.OnPreGameStart)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(GameLoopDraw), nameof(GetLocations)))
            );
        }

        static void GetLocations()
        {
            var p_settings = AccessTools.StaticFieldRefAccess<LocationSettings>(
                "JumpKing.MiscSystems.LocationText.LocationTextManager:_settings"
            );
            //var settings = Traverse.Create(__instance)
            //    .Field("m_location_text_manager")
            //    .Property("SETTINGS")
            //    .GetValue<LocationSettings>();

            Locations = ((LocationSettings)p_settings).locations;

            //NormalEnding.ENDING_SCREEN0
            //NewBabePlusEnding.ENDING_SCREEN0
            //OwlEnding.ENDING_SCREEN0
            FirstBabeAreas = GetAllScreensFromBabeScreen(Locations, NormalEnding.ENDING_SCREEN0, out Location[] locations2);
            SecondBabeAreas = GetAllScreensFromBabeScreen(locations2, NewBabePlusEnding.ENDING_SCREEN0, out Location[] locations3);
            ThirdBabeAreas = GetAllScreensFromBabeScreen(locations3, OwlEnding.ENDING_SCREEN0, out _);

            // correcting percentage lol
            if (Game1.instance.contentManager.root == "Content")
            {
                // nb+
                List<int> nbp = SecondBabeAreas.ToList();
                nbp.InsertRange(0, new int[] { 45, 46 });
                SecondBabeAreas = nbp.ToArray();

                // gotb
                List<int> gotb = ThirdBabeAreas.ToList();
                // screen before philosopher / entrance
                gotb.Insert(0, 156);
                // the hole
                gotb.InsertRange(5, new int[] { 164, 163, 162, 161 });
                // missing middle values
                gotb.Insert(30, 123);
                gotb.Insert(38, 131);
                gotb.Insert(46, 139);
                ThirdBabeAreas = gotb.ToArray();
            }
        }

        private static int[] GetAllScreensFromBabeScreen(Location[] locations, int babe_ending_screen, out Location[] remaining)
        {
            var last_item = Array.Find(locations, x => x.start <= babe_ending_screen && x.end >= babe_ending_screen);
            var index_last_item = Array.IndexOf(locations, last_item);
            
            remaining = locations.Skip(index_last_item + 1).ToArray();

            HashSet<int> screens = new HashSet<int>();
            foreach (Location location in locations.Take(index_last_item + 1))
            {
                for (int i = location.start; i <= location.end; i++)
                {
                    screens.Add(i);
                }
            }
            return screens.ToArray();
        }

        static Location[] Locations { get; set; }

        private static int[] FirstBabeAreas { get; set; } = new int[0];
        private static int[] SecondBabeAreas { get; set; } = new int[0];
        private static int[] ThirdBabeAreas { get; set; } = new int[0];

        static void Draw(GameLoop __instance)
        {
            // if not in pause (!PauseManager.instance.IsPaused) AND if enabled
            if (JumpKingLevelPercentage.Preferences.IsEnabled &&
                !Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
            {
                if (JumpKingLevelPercentage.Preferences.DisplayType == ELevelPercentageDisplayType.Percentage)
                {
                    TextHelper.DrawString(
                        Game1.instance.contentManager.font.MenuFont,
                        $"{GamePercent(Camera.CurrentScreenIndex1)}%",
                        new Vector2(12f, 62f),
                        //new Vector2(12f, 44f),
                        Color.White, Vector2.Zero, true
                    );
                    return;
                }

                TextHelper.DrawString(
                    Game1.instance.contentManager.font.MenuFont,
                    $"{GameProgress(Camera.CurrentScreenIndex1)}%",
                    new Vector2(12f, 62f),
                    //new Vector2(12f, 44f),
                    Color.White, Vector2.Zero, true
                );
            }
        }

        private static string GameProgress(int currentScreen)
        {
            if (FirstBabeAreas.Contains(currentScreen))
            {
                return Array.IndexOf(FirstBabeAreas, currentScreen) + "/" + (FirstBabeAreas.Length - 1);
            }

            if (SecondBabeAreas.Contains(currentScreen))
            {
                return Array.IndexOf(SecondBabeAreas, currentScreen) + "/" + (SecondBabeAreas.Length - 1);
            }

            if (ThirdBabeAreas.Contains(currentScreen))
            {
                return Array.IndexOf(ThirdBabeAreas, currentScreen) + "/" + (ThirdBabeAreas.Length - 1);
            }

            return "Progress disabled";
        }

        public static string GamePercent(int currentScreen)
        {
            if (FirstBabeAreas.Contains(currentScreen))
            {
                var index = Array.IndexOf(FirstBabeAreas, currentScreen);
                float val = (float)((float)index / (float)(FirstBabeAreas.Length - 1)) * 100f;
                return val.ToString("0.00");
            }

            if (SecondBabeAreas.Contains(currentScreen))
            {
                var index = Array.IndexOf(SecondBabeAreas, currentScreen);
                float val = (float)((float)index / (float)(SecondBabeAreas.Length - 1)) * 100f;
                return val.ToString("0.00");
            }

            if (ThirdBabeAreas.Contains(currentScreen))
            {
                var index = Array.IndexOf(ThirdBabeAreas, currentScreen);
                float val = (float)((float)index / (float)(ThirdBabeAreas.Length - 1)) * 100f;
                return val.ToString("0.00");
            }

            return "% disabled";
        }

    }
}
