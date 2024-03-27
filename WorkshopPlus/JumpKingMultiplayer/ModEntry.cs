using JumpKing.Mods;
using JumpKing.PauseMenu.BT;
using JumpKing.PauseMenu;
using JumpKingMultiplayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using JumpKingMultiplayer.Menu;
using HarmonyLib;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using JumpKing.Util;
using JumpKing;
using Microsoft.Xna.Framework.Graphics;
using JumpKingMultiplayer.Menu.DisplayOptions;

namespace JumpKingMultiplayer
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "sahandevs.JumpKingMultiplayer";
        const string HARMONY_IDENTIFIER = "sahandevs.JumpKingMultiplayer.Harmony";
        const string SETTINGS_FILE = "sahandevs.JumpKingMultiplayer.Settings.xml";

        private static string AssemblyPath { get; set; }
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
            var harmony = new Harmony(HARMONY_IDENTIFIER);
            new MultiplayerPatches(harmony);

            MultiplayerManager.instance = new MultiplayerManager();
            MultiplayerManager.instance.Init();
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

        #region Options
        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static CreateLeaveLobbyButton Toggle(object factory, GuiFormat format)
        {
            return new CreateLeaveLobbyButton();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static InviteToLobbyButton InviteToLobby(object factory, GuiFormat format)
        {
            return new InviteToLobbyButton();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleOpenLobby OpenLobby(object factory, GuiFormat format)
        {
            return new ToggleOpenLobby();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static ToggleCreateLobbyOnLaunch CreateOnLaunch(object factory, GuiFormat format)
        {
            return new ToggleCreateLobbyOnLaunch();
        }

        [PauseMenuItemSetting]
        [MainMenuItemSetting]
        public static TextButton DisplaySettings(object factory, GuiFormat format)
        {
            var drawables = Traverse.Create(factory).Field("m_drawables");

            MenuSelector menuSelector = new MenuSelector(format);
            menuSelector.AddChild(new PersonalColorChangeOption());
            menuSelector.AddChild(new ToggleLeaderboard());
            menuSelector.AddChild(new ToggleProximityPlayers());
            menuSelector.AddChild(new GhostPlayerDisplayOption());
            menuSelector.AddChild(new GhostPlayerOpacityOption());
            menuSelector.Initialize();

            drawables.GetValue<List<IDrawable>>().Add(menuSelector);

            return new TextButton("Display Options", menuSelector);
        }
        #endregion

        /// <summary>
        /// Called by Jump King when the level unloads
        /// </summary>
        [OnLevelUnload]
        public static void OnLevelUnload()
        {
            // Your code here
        }

        internal static Texture2D LeaderboardHeader { get; private set; }
        internal static Texture2D HostFlag { get; private set; }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            LeaderboardHeader = 
                Game1.instance.contentManager.Load<Texture2D>($@"{AssemblyPath}\Content\leaderboard_header");
            HostFlag = Game1.instance.contentManager.Load<Texture2D>($@"{AssemblyPath}\Content\host");
        }

        /// <summary>
        /// Called by Jump King when the Level Ends
        /// </summary>
        [OnLevelEnd]
        public static void OnLevelEnd()
        {
            // Your code here
        }
    }
}
