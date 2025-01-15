using HarmonyLib;
using JumpKing.Mods;
using JumpKing.Workshop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace JumpKingAdditionalAnimations
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "Phoenixx19.JumpKingAdditionalAnimations";
        const string HARMONY_IDENTIFIER = "Phoenixx19.JumpKingAdditionalAnimations.Harmony";
        const string XML_FILE = "additional_animations.xml";

        public static SplatAddedFrames Splat { get; private set; }
        public static ChargeAddedFrames Charge { get; private set; }

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

            Splat = new SplatAddedFrames(harmony);
            Charge = new ChargeAddedFrames(harmony);

            // 1. loop around all items (WorkshopManager.instance.reskins & collections)
            // 2. if the item has the setting in its item root then:
            //      1. check what classes are not null/empty
            //      2. add on each [...]AddedFrames class, a reference to the AdditionalAnimations given (in a list ofc)
            // 3. Fix reference from SetSprite() to deeper link (IKingSpriteGroup) (the hard part)
            // 4. Use the list in [...]AddedFrames class to loop around the textures to update


            // move this to each item.
            var xml_path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), XML_FILE);
            //XmlSerializerHelper.Serialize(xml_path, new AdditionalAnimationsSettings());
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
            // Your code here
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
