using HarmonyLib;
using JumpKing.GameManager;
using JumpKing.Player;
using JumpKingMultiplayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer
{
    public class MultiplayerPatches
    {
        public MultiplayerPatches(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(GameLoop), "OnNewRun"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPatches), nameof(InitMultiplayer)))
            );
            harmony.Patch(
                AccessTools.Method(typeof(PlayerEntity), "Update"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPatches), nameof(TrackPlayer)))
            );
            harmony.Patch(
                typeof(GameLoop).GetMethod(nameof(GameLoop.Draw)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPatches), nameof(DrawLeaderboard)))
            );
            harmony.Patch(
                AccessTools.Method(typeof(GameLoop), "MyRun"),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPatches), nameof(RunNodes)))
            );
            harmony.Patch(
                AccessTools.Method("JumpKing.Props.RaymanWall.RaymanWallEntity:Draw"),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MultiplayerPatches), nameof(DrawPlayers)))
            );
        }

        static void RunNodes(BehaviorTree.TickData p_data)
        {
            MultiplayerManager.instance.emptyLobbyInviteInfo?.Run(p_data);
            MultiplayerManager.instance.inviteInfo?.Run(p_data);
            MultiplayerManager.instance.leaderBoard?.Run(p_data);
        }

        static bool DrawPlayers()
        {
            MultiplayerManager.DrawPlayers();
            return true;
        }

        static void DrawLeaderboard(GameLoop __instance)
        {
            if (Traverse.Create(__instance).Field("m_pause_manager").Property("IsPaused").GetValue<bool>())
                return;

            if (MultiplayerManager.IsOnline())
            {
                MultiplayerManager.instance.leaderBoard?.DrawBoard();
            }
            MultiplayerManager.instance.Draw();
        }

        static void InitMultiplayer()
        {
            //MultiplayerManager.instance = new MultiplayerManager();
            //MultiplayerManager.instance.Init();
        }

        static void TrackPlayer(float p_delta)
        {
            MultiplayerManager.instance.Tick(p_delta);
        }

        // JumpKing/EntityComponent/EntityManager.cs
        // https://github.com/JumpKingPlus/JumpKing/commit/1417cced51636c3986f67913eb6e6975aedc010b#diff-d0a85485bf022b60d6a2052eb13f5b5f534f7bc3385b0086ba84fdd27e6c8e8c
    }
}
