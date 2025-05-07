using HarmonyLib;
using JumpKing.Mods;
using JumpKing.PauseMenu;
using JumpKingJumpArch.Menu;
using JumpKingJumpArch.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace JumpKingJumpArch
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "Phoenixx19.JumpKingJumpArch";
        const string HARMONY_IDENTIFIER = "Phoenixx19.JumpKingJumpArch.Harmony";
        const string SETTINGS_FILE = "Phoenixx19.JumpKingJumpArch.Settings.xml";

        public const int LIMIT = 75;

        public static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
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
            Harmony harmony = new Harmony(HARMONY_IDENTIFIER);

            // patching on each class (is better than attributes)
            new JumpChargeCalc(harmony);
            new GameLoopDraw(harmony);
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleJumpArch Toggle(object factory, GuiFormat format)
        {
            return new ToggleJumpArch();
        }

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
