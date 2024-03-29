using JumpKing.Mods;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace DependencyDealerTest
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "Phoenixx19.DependencyDealerTest";

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
            DependencyDealer.InstallDependenciesIfMissing(IDENTIFIER, (PublishedFileId_t)3169568082uL);
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
