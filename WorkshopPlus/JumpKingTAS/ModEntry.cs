using EntityComponent;
using HarmonyLib;
using JumpKing.Mods;
using JumpKing.PauseMenu;
using JumpKing.Player;
using JumpKingTAS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using TASStudio;

namespace JumpKingTAS
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "DevilSquirrel.JumpKingTAS";
        const string HARMONY_IDENTIFIER = "DevilSquirrel.JumpKingTAS.Harmony";
        const string SETTINGS_FILE = "DevilSquirrel.JumpKingTAS.Settings.xml";

        private static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }
        internal static TASBehaviour Behaviour { get; set; }

        static Studio JKStudio;

        #region Menu Items
        [PauseMenuItemSetting]
        public static ToggleTAS Toggle(object factory, GuiFormat format)
        {
            return new ToggleTAS();
        }
        #endregion

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

            // enables win-based style (not legacy win xp look)
            try
            {
                System.Windows.Forms.Application.EnableVisualStyles();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[ERROR] [{IDENTIFIER}] {e.Message}");
            }

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

            // toggle tas
            Preferences.PropertyChanged += ToggleTAS;

            // add save on property changed
            Preferences.PropertyChanged += SaveSettingsOnFile;

            var harmony = new Harmony(HARMONY_IDENTIFIER);

            new GameUpdate(harmony);
            new PadStateGet(harmony);
            new GameLoopDraw(harmony);

            Behaviour = new TASBehaviour();
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

        private static void ToggleTAS(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Preferences.IsEnabled))
                return;

            if (!bool.TryParse(((Preferences)sender).IsEnabled.ToString(), out bool isEnabled))
                return;

            PlayerEntity player = EntityManager.instance.Find<PlayerEntity>();
            if (player is null)
                return;

            if (isEnabled)
            {
                player.m_body.RegisterBehaviour(Behaviour);
                JKStudio = new Studio();
                JKStudio.Show();
            }
            else
            {
                player.m_body.RemoveBehaviour(Behaviour);
                JKStudio.Hide();
                JKStudio = null;
            }
        }

        [OnLevelStart]
        public static void OnLevelStart()
        {
            // enable behaviour if it is enabled (on boot time)
            PlayerEntity player = EntityManager.instance.Find<PlayerEntity>();
            if (Preferences.IsEnabled)
            {
                player?.m_body.RegisterBehaviour(Behaviour);
                if (JKStudio == null)
                {
                    JKStudio = new Studio();
                    JKStudio.Show();
                }
            }
        }
    }
}
