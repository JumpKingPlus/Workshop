using HarmonyLib;
using HitboxResizer.Patches;
using JumpKing;
using JumpKing.GameManager;
using JumpKing.Mods;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HitboxResizer
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "Phoenixx19.HitboxResizer";
        const string HARMONY_IDENTIFIER = "Phoenixx19.HitboxResizer.Harmony";

        const string FLAG_REGEX = "^KingCustomHitbox(Width|Height)=([0-9]+)$";
        internal static Regex HitboxResizerRegEx = new Regex(FLAG_REGEX);

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            var harmony = new Harmony(HARMONY_IDENTIFIER);
#if DEBUG
            Debugger.Launch();
            Harmony.DEBUG = true;
#endif
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Called by Jump King when the Level Starts
        /// </summary>
        [OnLevelStart]
        public static void OnLevelStart()
        {
            // custom hitboxes time
            PrefixPlayerEntityDraw.IsCustomHitbox = HasAnyHitboxResizerTags(
                out var width,
                out var height);
            if (PrefixPlayerEntityDraw.IsCustomHitbox)
            {
                SetHitbox(width, height);
            }
        }

        /// <summary>
        /// Checks the level tags for the occurence of our tag.
        /// </summary>
        /// <param name="width">Vanilla width or tag specified width.</param>
        /// <param name="height">Vanilla height or tag specified heigth.</param>
        /// <returns>True if at least one resize tag has been found, false otherwise.</returns>
        private static bool HasAnyHitboxResizerTags(out int width, out int height)
        {
            // fallback values
            width = PlayerValues.PLAYER_WIDTH;
            height = PlayerValues.PLAYER_HEIGHT;

            // null check
            var tags = Game1.instance.contentManager?.level?.Info.Tags;
            if (tags is null)
            {
                return false;
            }

            bool isCustom = false;
            foreach (string tag in tags)
            {
                var match = HitboxResizerRegEx.Match(tag);
                // no match found! (most probably its a different flag)
                if (!match.Success)
                {
                    continue;
                }
                isCustom = true;
                // If the regex matches we have captured three groups.
                // groups[0]: Full tag.
                // groups[1]: Either Width or Height.
                // groups[2]: New size.
                var groups = match.Groups;
                var value = int.Parse(groups[2].Value);
                if (groups[1].Value == "Width")
                {
                    width = value;
                }
                else
                {
                    height = value;
                }
            }
            return isCustom;
        }

        private static void SetHitbox(int width = PlayerValues.PLAYER_WIDTH, int height = PlayerValues.PLAYER_HEIGHT)
        {
            PrefixPlayerEntityDraw.Width = width;
            PrefixPlayerEntityDraw.Height = height;

            var tBody = Traverse.Create(GameLoop.m_player.m_body);
            tBody.Field("m_width").SetValue(PrefixPlayerEntityDraw.Width);
            tBody.Field("m_height").SetValue(PrefixPlayerEntityDraw.Height);
        }
    }
}
