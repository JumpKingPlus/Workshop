using JumpKing.MiscSystems.LocationText;
using JumpKing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKingDiscordRPC;

namespace JumpKingDiscordRPC.Extensions
{
    public static class LanguageJKExtensions
    {
        public static string TryGetResource(this string try_resource)
        {
            var @string = LanguageJK.language.ResourceManager.GetString(try_resource);
            if (@string != null)
            {
                return @string;
            }
            return try_resource;
        }
    }
}
