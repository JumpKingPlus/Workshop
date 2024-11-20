using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingDiscordRPC.Models
{
    public class SteamPreviewGrabber
    {
        public SteamPreviewGrabber(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method("JumpKing.Workshop.JKSteamUtils:OnUGCQueryReturn"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(SteamPreviewGrabber), nameof(OnUGCQueryReturned)))
            );
        }

        static bool OnUGCQueryReturned(SteamUGCQueryCompleted_t handle)
        {
            for (uint i = 0; i < handle.m_unNumResultsReturned; i++)
            {
                var hasResult = SteamUGC.GetQueryUGCResult(handle.m_handle, i, out SteamUGCDetails_t details);
                
                if (!hasResult)
                    continue;

                if (SteamUGC.GetQueryUGCPreviewURL(handle.m_handle, i, out string url, 1024))
                {
                    PreviewDictionary.Add((ulong)details.m_nPublishedFileId, url);
                }
            }
            return true;
        }

        public static Dictionary<ulong, string> PreviewDictionary = new Dictionary<ulong, string>();
    }
}
