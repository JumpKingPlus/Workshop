using BehaviorTree;
using HarmonyLib;
using JumpKing;
using JumpKing.Mods;
using JumpKing.PauseMenu;
using JumpKing.PauseMenu.BT;
using JumpKingLevelPercentage.Menu;
using JumpKingLevelPercentage.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLevelPercentage
{
    [JumpKingMod(IDENTIFIER)]
    public static class JumpKingLevelPercentage
    {
        const string IDENTIFIER = "Phoenixx19.LevelPercentage";
        const string HARMONY_IDENTIFIER = "Phoenixx19.LevelPercentage.Harmony";
        const string SETTINGS_FILE = "Phoenixx19.LevelPercentage.Settings.xml";

        private static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif

            // set path for dll
            AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // try reading config file
            try
            {
                Preferences = XmlSerializerHelper.Deserialize<Preferences>($@"{AssemblyPath}\{SETTINGS_FILE}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
                Preferences = new Preferences();
            }

            // add save on property changed
            Preferences.PropertyChanged += SaveSettingsOnFile;

            // setup harmony
            var harmony = new Harmony(HARMONY_IDENTIFIER);

            // patching on each class (is better than attributes)
            new GameLoopDraw(harmony);
        }

        #region Menu Items
        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleLevelProgress Toggle(object factory, GuiFormat format)
        {
            return new ToggleLevelProgress();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static LevelProgressOption Option(object factory, GuiFormat format)
        {
            return new LevelProgressOption();
        }
        #endregion
        
        private static void SaveSettingsOnFile(object sender, PropertyChangedEventArgs args)
        {
            try
            {
                XmlSerializerHelper.Serialize($@"{AssemblyPath}\{SETTINGS_FILE}", Preferences);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            }
        }
    }
}
