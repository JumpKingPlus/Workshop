using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingDiscordRPC
{
    internal enum ERPCPreview
    {
        /// <summary>
        /// The preview is completely removed.
        /// </summary>
        None,
        
        /// <summary>
        /// The preview will follow what's given for each location.
        /// </summary>
        Location,

        /// <summary>
        /// The preview will follow what's given for each screen.
        /// </summary>
        Screen,

        /// <summary>
        /// The preview will be pulled from Steam's preview (if it can do that).
        /// </summary>
        Fallback
    }
}
