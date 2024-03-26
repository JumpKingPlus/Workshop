using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer
{
    public enum EGhostPlayerDisplayType
    {
        [Description("Player ghost only")]
        GhostPlayerOnly,

        [Description("Player ghost + name")]
        GhostPlayerAndLabel,
        
        [Description("Tinted player ghost")]
        GhostPlayerColored,
        
        [Description("Tinted player ghost + name")]
        GhostPlayerColoredAndLabel
    }
}
