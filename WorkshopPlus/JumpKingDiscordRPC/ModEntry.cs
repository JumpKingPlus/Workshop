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
using System.Text.RegularExpressions;
using JumpKing;

namespace JumpKingDiscordRPC
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        public const string IDENTIFIER = "Phoenixx19.JumpKingDiscordRPC";
        const string HARMONY_IDENTIFIER = "Phoenixx19.JumpKingDiscordRPC.Harmony";

        const string FLAG_REGEX = "^ShowPreviewBy=(None|Location|Screen)";
        internal static Regex PreviewImageRegEx = new Regex(FLAG_REGEX);

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
            new SteamPreviewGrabber(harmony);
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

            // custom images?
            if (HasImageTag(out ERPCPreview preview))
            {
                //
                SetImageTags(preview);
                return;
            }

            SetImageTags(ERPCPreview.Fallback);
        }

        private static void SetImageTags(ERPCPreview fallback)
        {
            Client.SetFallbackImage();
        }

        private static bool HasImageTag(out ERPCPreview imagePreviewType)
        {
            imagePreviewType = ERPCPreview.None;

            // nre
            if (!(Game1.instance.contentManager?.level?.Info.Tags is string[] tags))
            {
                return false;
            }
            foreach (string tag in tags)
            {
                // not found... yet
                if (!PreviewImageRegEx.IsMatch(tag))
                {
                    continue;
                }

                // found it??
                string[] strings = tag.Split('=');
                if (strings.Length != 2 || !Enum.TryParse(strings[1], out imagePreviewType))
                {
                    // no good
                    continue;
                }

                // found it!
                return true;
            }

            // well, it tried I guess
            return false;
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
