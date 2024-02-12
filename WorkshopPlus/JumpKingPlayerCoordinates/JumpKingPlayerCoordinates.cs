using HarmonyLib;
using JumpKing.Mods;
using JumpKingPlayerCoordinates.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingPlayerCoordinates
{
    [JumpKingMod(IDENTIFIER)]
    public class JumpKingPlayerCoordinates
    {
        const string IDENTIFIER = "Phoenixx19.PlayerCoordinates";
        const string HARMONY_IDENTIFIER = "Phoenixx19.PlayerCoordinates.Harmony";
        const string SETTINGS_FILE = "Phoenixx19.PlayerCoordinates.Settings.xml";

        private static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        [BeforeLevelLoad]
        public static void OnLevelStart()
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
            new MenuOptions(harmony);
        }

        private static void SaveSettingsOnFile(object sender, System.ComponentModel.PropertyChangedEventArgs args)
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
