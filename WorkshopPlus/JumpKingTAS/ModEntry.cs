using HarmonyLib;
using JumpKing.Mods;
using JumpKingTAS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TASStudio;

namespace JumpKingTAS
{
    [JumpKingMod(IDENTIFIER)]
    public static class ModEntry
    {
        const string IDENTIFIER = "DevilSquirrel.JumpKingTAS";
        const string HARMONY_IDENTIFIER = "DevilSquirrel.JumpKingTAS.Harmony";

        static Studio JKStudio;

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

            var harmony = new Harmony(HARMONY_IDENTIFIER);

            new GameUpdate(harmony);
            new PadStateGet(harmony);
            new GameLoopDraw(harmony);

            try
            {
                System.Windows.Forms.Application.EnableVisualStyles();

                JKStudio = new Studio();
                JKStudio.Show();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
