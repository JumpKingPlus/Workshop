using HarmonyLib;
using JumpKing.MiscSystems.Achievements;
using JumpKing.SaveThread.SaveComponents;
using LanguageJK;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DiscordRPC;
using System.Diagnostics;
using JumpKing;
using DiscordRPC.Logging;
using EntityComponent;
using System.Reflection;
using JumpKing.MiscSystems.LocationText;
using System.Reflection.Emit;
using JumpKingDiscordRPC.Extensions;
using JumpKingDiscordRPC.Models;
using JumpKingDiscordRPC;
using Steamworks;
using JumpKing.Workshop;

namespace JumpKingDiscordRPC.Models
{
    public class RPCWrapper
    {
        const string RPC_ID = "726077029195448430";
        internal DiscordRpcClient Client { get; private set; }
        private RichPresence RichPresence { get; set; }
        private System.Timers.Timer timer;

        private bool customImage = false;

        internal bool IsInGameLoop { get; set; } = false;

        private static PlayerStats CurrentStats
        {
            get
            {
                // JumpKing.MiscSystems.Achievements.AchievementManager:instance
                var instance = AccessTools.Field("JumpKing.MiscSystems.Achievements.AchievementManager:instance");
                var instanceTraverse = Traverse.Create(instance.GetValue(null));

                // .GetCurrentStats();
                var stats = instanceTraverse.Method("GetCurrentStats").GetValue<PlayerStats>();

                return stats;
            }
        }

        private static string GetLevelName()
        {
            var instance = AccessTools.Field("JumpKing.PauseMenu.PauseManager:instance");
            var instanceTraverse = Traverse.Create(instance.GetValue(null));

            var method = instanceTraverse.Field("m_factory").Method("GetLevelTitle");
            return method.GetValue<string>();
        }

        private static Location GetLocation(bool is_custom_game)
        {
            var location = Location.Empty;

            if (!is_custom_game)
            {
                var locations1 = FixedJKLocations.Locations
                    .Where(x => x.Start <= Camera.CurrentScreenIndex1 &&
                                x.End >= Camera.CurrentScreenIndex1);

                if (locations1.Any())
                {
                    location = locations1.First();
                }
            }
            else
            {
                //MethodInfo method = typeof(EntityManager).GetMethod(nameof(EntityManager.Find));
                //var ltm_type = AccessTools.TypeByName("JumpKing.MiscSystems.LocationText.LocationTextManager");

                //// EntityManager.Find<LocationTextManager>
                //MethodInfo generic = method.MakeGenericMethod(ltm_type);

                //// EntityManager.instance.Find<LocationTextManager>
                //var x = generic.Invoke(EntityManager.instance, new object[] { });

                //var traverse = Traverse.Create(x);
                //var type = AccessTools.TypeByName("");

                var instance = AccessTools.Property(/*type, */"JumpKing.MiscSystems.LocationText.LocationTextManager:SETTINGS");
                var instanceTraverse = Traverse.Create(instance.GetValue(null));

                var settings = instanceTraverse.GetValue<LocationSettings>();

                var locations = settings.locations
                    .Where(x => x.start <= Camera.CurrentScreenIndex1 &&
                                x.end >= Camera.CurrentScreenIndex1);

                // this could need some polishing
                location.Name = locations.Any() ? locations.First().name : "Unknown Location";
            }

            return location;
        }

        private static Button GetExtensionButton = new Button()
        {
            Label = "Get extension",
            Url = "steam://url/CommunityFilePage/3190590114"
        };

        private static Button GetMapButton(ulong mapId) => new Button()
        {
            Label = "Get level",
            Url = $"steam://url/CommunityFilePage/{mapId}"
        };

        public RPCWrapper(Harmony harmony)
        {
            Start();

            try
            {
                harmony.Patch(
                    AccessTools.Method("JumpKing.Game1:OnExiting"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(RPCWrapper), nameof(OnExit)))
                );

                harmony.Patch(
                    AccessTools.Method("JumpKing.SaveThread.SaveComponents.FullRunSave:set_fullRunSave"),
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(RPCWrapper), nameof(SetBootsAndRingImages)))
                );
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void Start()
        {
            Client = new DiscordRpcClient(RPC_ID, -1, null, autoEvents: false);
            Client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
            Client.RegisterUriScheme(JumpKing.Program.GAME_ID.ToString());
            Client.Initialize();

            timer = new System.Timers.Timer();
            timer.Elapsed += OnTimerElapsed;
            timer.Interval = 150d;
            timer.AutoReset = true;
            timer.Enabled = true;
            // timer.Start(); // setting enabled to true does it
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        internal void SetFallbackImage()
        {
            customImage = true;
        }

        public void Update()
        {
            // no need to update if it's disposed
            if (Client.IsDisposed) return;

            Client.Invoke();

            AssignRichPresence();

            Client.SetPresence(RichPresence);
        }

        /// <summary>
        /// I wanted to make it more smart 
        /// (so it does the changes only when necessary but it's too complicated for me @ 11pm.
        /// it's fine as is, JK+ had this solution nonetheless)<br></br>
        /// Plus, <see cref="DiscordRpcClient.SkipIdenticalPresence"/> exists
        /// </summary>
        private void AssignRichPresence()
        {
            // back in menu
            if (!IsInGameLoop)
            {
                RichPresence = new RichPresence()
                {
                    Buttons = new Button[] { GetExtensionButton },
                    Details = "Main Menu",
                    Assets = new Assets()
                    {
                        SmallImageKey = "jkplus",
                        SmallImageText = "Discord RPC Mod",
                        LargeImageKey = "jklogo"
                    },
                    // warning
                    // this will reset everytime, make it stop!!
                    Timestamps = Timestamps.Now
                };
                return;
            }

            #region harmony funny code
            // EntityManager.Find
            var findMethod = typeof(EntityManager).GetMethod(nameof(EntityManager.Find));
            // <JumpKing.PauseMenu.PauseManager>
            var tMethod = findMethod.MakeGenericMethod(AccessTools.TypeByName("JumpKing.PauseMenu.PauseManager"));
            // EntityManager.instance.Find<JumpKing.PauseMenu.PauseManager>()
            var pauseManager = tMethod.Invoke(EntityManager.instance, new object[0]);
            #endregion

            // cant access so use traverse to get isPaused value
            var isPaused = Traverse.Create(pauseManager).Property("IsPaused").GetValue<bool>();
            var isCustomLevel = Game1.instance.contentManager.root != "Content";

            // TIL 0.017f is somewhat the right choice here, 1/60 should have been more correct but it's not.
            // update: who needs gameTime when there is a literal timeSpan in CurrentStats LULW
            //var gameTime = TimeSpan.FromSeconds((int)Math.Round(CurrentStats._ticks * 0.017f));
            var attempts = CurrentStats.attempts;
            var sessions = CurrentStats.session;
            
            var falls = CurrentStats.falls;
            var fallsText = string.Concat(falls, " ", (falls == 1) ? "fall" : "falls"); 

            RichPresence presence = RichPresence;

            // disable timer if paused (looks janky on discord)
            if (isPaused)
            {
                presence.WithTimestamps(new Timestamps());
            }
            else
            {
                presence.WithTimestamps(new Timestamps(DateTime.UtcNow - CurrentStats.timeSpan));
            }

            // get location name properly
            var location = GetLocation(isCustomLevel);
            var locationName = location.Name.TryGetResource();

            if (Game1.instance.contentManager.root != null && customImage)
            {
                // todo: remember 256b limit!
                // large image (URLs work!) "https://phoenixx.design/content/images/thex-white.png"
                var id = Game1.instance.contentManager.level.ID;
                if (SteamPreviewGrabber.PreviewDictionary.ContainsKey(id))
                {
                    presence.Assets.LargeImageKey = SteamPreviewGrabber.PreviewDictionary[id];
                    presence.Assets.LargeImageText = GetLevelName();
                }
            } else
            {
                presence.Assets.LargeImageKey = location.ImageKey ?? "unknown";
                presence.Assets.LargeImageText = locationName;
            }


            // switch for each RPC type
            switch (ModEntry.Preferences.Preset)
            {
                case ERPCPresets.BabeAndLocation:
                    presence.WithDetails(GetLevelName());
                    presence.WithState(locationName);
                    break;

                case ERPCPresets.LocationAndFalls:
                    presence.WithDetails(locationName);
                    presence.WithState(fallsText);
                    break;

                case ERPCPresets.SessionAndFalls:
                    presence.WithDetails($"Attempt n.°{sessions}");
                    presence.WithState(fallsText);
                    break;

                default:
                    throw new InvalidOperationException("The preset for Discord RPC cannot be found");
            }

            if (isCustomLevel)
            {
                presence.Buttons = new Button[]
                { 
                    GetMapButton(Game1.instance.contentManager.level.ID),
                    GetExtensionButton
                };
            }

            RichPresence = presence;
        }

        public void Stop()
        {
            timer.Enabled = false;
            timer.Elapsed -= OnTimerElapsed;
            timer.Dispose();

            Client.Dispose();
        }

        /// <summary>
        /// Disposes the client correctly.
        /// </summary>
        public static void OnExit()
        {
            ModEntry.Client.Stop();
        }

        #region boots n ring
        public static void SetBootsAndRingImages(FullRunSave value)
        {
            ModEntry.Client.SetBootsAndRing(value);
        }
        public void SetBootsAndRing(FullRunSave runSave)
        {
            // both
            if (runSave.wear_snake_ring && runSave.wear_giant_boots)
            {
                RichPresence.Assets.SmallImageKey = "shoes_and_ring";
                RichPresence.Assets.SmallImageText = language.ITEMNAMEUTIL_GIANT_BOOTS;
                return;
            }

            // snake ring
            if (runSave.wear_snake_ring)
            {
                RichPresence.Assets.SmallImageKey = "ring";
                RichPresence.Assets.SmallImageText = language.ITEMNAMEUTIL_SNAKERING;
                return;
            }

            // giant boots
            if (runSave.wear_giant_boots)
            {
                RichPresence.Assets.SmallImageKey = "shoes_iron";
                RichPresence.Assets.SmallImageText = language.ITEMNAMEUTIL_GIANT_BOOTS + " + " + language.ITEMNAMEUTIL_SNAKERING;
                return;
            }

            // nada
            RichPresence.Assets.SmallImageKey = string.Empty;
            RichPresence.Assets.SmallImageText = string.Empty;
        }
        #endregion
    }
}
