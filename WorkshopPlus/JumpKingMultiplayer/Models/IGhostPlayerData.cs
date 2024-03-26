using JumpKing.Player;
using Microsoft.Xna.Framework;
using static JumpKingMultiplayer.Models.GhostPlayer;

namespace JumpKingMultiplayer.Models
{
    public interface IGhostPlayerData
    {
        int ScreenIndex1 { get; set; }
        Vector2 RelativePosition { get; set; }
        Vector2 AbsolutePosition { get; set; }
        ulong? LevelId { get; set; }
        string SteamName { get; set; }
        Steamworks.CSteamID SteamId { get; set; }
        bool IsDisposed { get; set; }
        Color Color { get; set; }
    }
}