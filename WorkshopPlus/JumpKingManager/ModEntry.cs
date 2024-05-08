using EntityComponent;
using JumpKing.Mods;
using JumpKing.PauseMenu;
using JumpKing.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace JumpKingManager
{
    [JumpKingMod("DevilSquirrel.JumpKingManager")]
    public static class ModEntry
    {
        const string IDENTIFIER = "DevilSquirrel.JumpKingManager";
        const string HARMONY_IDENTIFIER = "DevilSquirrel.JumpKingManager.Harmony";
        const string SETTINGS_FILE = "DevilSquirrel.JumpKingManager.Settings.xml";

        private static string AssemblyPath { get; set; }
        public static Preferences Preferences { get; private set; }
        internal static ManagerBehaviour Behaviour { get; set; }
        
        static Manager JKManager;

        #region Menu Items
        [PauseMenuItemSetting]
        public static ToggleManager Toggle(object factory, GuiFormat format)
        {
            return new ToggleManager();
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

            // toggle manager
            Preferences.PropertyChanged += ToggleManager;
            
            // add save on property changed
            Preferences.PropertyChanged += SaveSettingsOnFile;

            Behaviour = new ManagerBehaviour();
        }

        private static void ToggleManager(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                JKManager = new Manager();
                JKManager.Show();
            } else
            {
                player.m_body.RemoveBehaviour(Behaviour);
                JKManager.Hide();
                JKManager = null;
            }
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

        [OnLevelStart]
        public static void OnLevelStart()
        {
            // enable behaviour if it is enabled (on boot time)
            PlayerEntity player = EntityManager.instance.Find<PlayerEntity>();
            if (Preferences.IsEnabled)
            {
                player?.m_body.RegisterBehaviour(Behaviour);
                if (JKManager == null)
                {
                    JKManager = new Manager();
                    JKManager.Show();
                }
            }
        }
    }
}
