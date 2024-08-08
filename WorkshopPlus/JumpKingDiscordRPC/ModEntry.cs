using JumpKing.Mods;
using System;
using System.Collections.Generic;
using System.Text;
using DiscordRPC;
using HarmonyLib;
using System.Diagnostics;
using JumpKingDiscordRPC.Models;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using JumpKingDiscordRPC.Menu;
using JumpKing.PauseMenu;

namespace JumpKingDiscordRPC
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        public const string IDENTIFIER = "Phoenixx19.JumpKingDiscordRPC";
        const string HARMONY_IDENTIFIER = "Phoenixx19.JumpKingDiscordRPC.Harmony";
        const string SETTINGS_FILE = "Phoenixx19.JumpKingDiscordRPC.Settings.xml";

        internal static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }

        public static RPCWrapper Client { get; set; }
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
            var harmony = new Harmony(HARMONY_IDENTIFIER);

            Client = new RPCWrapper(harmony);
        }

        /// <summary>
        /// Called by Jump King when the level unloads
        /// </summary>
        [OnLevelUnload]
        public static void OnLevelUnload()
        {
            // Your code here
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            Client.IsInGameLoop = true;
        }

        /// <summary>
        /// Called by Jump King when the Level Ends
        /// </summary>
        [OnLevelEnd]
        public static void OnLevelEnd()
        {
            Client.IsInGameLoop = false;
        }

        #region Menu Items
        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleDiscordRPC Toggle(object factory, GuiFormat format)
        {
            return new ToggleDiscordRPC();
        }

        [PauseMenuItemSetting]
        public static PresetOption Option(object factory, GuiFormat format)
        {
            return new PresetOption();
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
