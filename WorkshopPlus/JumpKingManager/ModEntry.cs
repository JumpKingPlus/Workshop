using JumpKing.Mods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace JumpKingManager
{
    [JumpKingMod("DevilSquirrel.JumpKingManager")]
    public static class ModEntry
    {
        static Manager JKManager;

        /// <summary>
        /// Called by Jump King before the level loads
        /// </summary>
        [BeforeLevelLoad]
        public static void BeforeLevelLoad()
        {
#if DEBUG
            Debugger.Launch();
#endif
            try
            {
                System.Windows.Forms.Application.EnableVisualStyles();

                JKManager = new Manager();
                JKManager.Show();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
