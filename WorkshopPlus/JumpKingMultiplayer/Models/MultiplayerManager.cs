using EntityComponent;
using JumpKing.GameManager;
using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpKingMultiplayer.Extensions;
using JumpKing;
using JumpKingMultiplayer.Helpers;
using JumpKingMultiplayer.Models.Infos;

namespace JumpKingMultiplayer.Models
{
    public enum PlayerSpriteEffect
    {
        None,
        FlipH,
        FlipV,
    }

    public enum PlayerSpriteState
    {
        JumpBounce,
        JumpUp,
        JumpFall,
        JumpSplat,
        Idle,
        WalkOne,
        WalkSmear,
        WalkTwo,
        JumpCharge,
        LookUp,
        StretchOne,
        StretchSmear,
        StretchTwo,
    }

    public static class Parser
    {
        public static T FromString<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string ToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

    public struct TrackData
    {
        public const int id = 1;
        public int screenIndex1 { get; set; }
        public float posX { get; set; }
        public float posY { get; set; }
        public ulong? levelId { get; set; }
        public int colorIdx { get; set; }
        public PlayerSpriteEffect flip { get; set; }
        public PlayerSpriteState sprite { get; set; }
        public int skippedFrames { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public float Diff(TrackData other)
        {
            float d = 0;
            d += Math.Abs(other.posX - this.posX);
            d += Math.Abs(other.posY - this.posY);
            if (other.sprite != this.sprite)
            {
                d += 10;
            }
            if (other.flip != this.flip)
            {
                d += 10;
            }
            if (other.levelId != this.levelId)
            {
                d += 10;
            }
            return d;
        }

        public override string ToString()
        {
            return Parser.ToString(this);
        }
    }

    public class MultiplayerManager
    {
        public MultiplayerManager() : base()
        {
        }

        public readonly CSteamID UserSteamId = SteamUser.GetSteamID();
        private bool _alreadyDrawedPlayers = false;

        public static void DrawPlayers()
        {
            if (MultiplayerManager.instance._alreadyDrawedPlayers) { return; }
            MultiplayerManager.instance.Players.ForEach(x => x.DrawFromOutside());
            MultiplayerManager.instance._alreadyDrawedPlayers = true;
        }

        public static MultiplayerManager instance { get; set; }

        internal List<GhostPlayer> Players = new List<GhostPlayer>();
        internal List<string> playersToAdd = new List<string>();

        internal TrackData lastTrackData = new TrackData { posX = 500, posY = 500, };

        public void SendUpdateToOthers()
        {
            if (!LobbyId.HasValue) { return; }
            var trackData = PlayerSpriteStateExtensions.FromPlayer(GameLoop.m_player);

            if (lastTrackData.Diff(trackData) > 1)
            {
                SendToAll(trackData);
                lastTrackData = trackData;
            }
        }

        public void SendToAll<T>(T message)
        {
            byte[] messageBytes;
            try
            {
                messageBytes = Encoding.ASCII.GetBytes(Parser.ToString(message));
            }
            catch (Exception e)
            {
                throw;
            }
            var len = (uint)messageBytes.Length;
            foreach (var sID in LobbyPlayers)
            {
                Steamworks.SteamNetworking.SendP2PPacket(sID, messageBytes, len, EP2PSend.k_EP2PSendReliableWithBuffering);
            }
        }

        public void GetUpdates()
        {
            if (!LobbyId.HasValue) { return; }
            if (SteamNetworking.IsP2PPacketAvailable(out uint size))
            {
                var buffer = new byte[size];
                if (SteamNetworking.ReadP2PPacket(buffer, size, out _, out CSteamID remoteId))
                {
                    var messageStr = System.Text.Encoding.ASCII.GetString(buffer);
                    try
                    {
                        var message = Parser.FromString<TrackData>(messageStr);
                        UpdatePlayerState(remoteId, message);

                    }
                    catch (Exception)
                    {
                        throw new NotImplementedException($"messageStr: {messageStr} \n remoteId: {remoteId} \n buffer: {buffer}");
                    }

                    // idk how to get the ping
                    //SteamNetworkingIdentity id = new SteamNetworkingIdentity();
                    //id.SetSteamID(remoteId);
                    //SteamNetworkingMessages.GetSessionConnectionInfo(ref id, out _, out SteamNetConnectionRealTimeStatus_t realTimeStatus);
                    //Debug.WriteLine($"[UPDATE] {remoteId} - {realTimeStatus.m_eState} {realTimeStatus.m_nPing} {realTimeStatus.m_flConnectionQualityLocal}");
                }
            }
        }

        internal LeaderBoard leaderBoard { get; set; }
        internal ProximityPlayers proximityPlayers { get; set; }
        internal InviteInfo inviteInfo { get; set; }
        internal EmptyLobbyInviteInfo emptyLobbyInviteInfo { get; set; }

        internal Callback<LobbyCreated_t> cb_lobbyCreated;
        internal Callback<LobbyInvite_t> cb_lobbyInvite;
        internal Callback<LobbyEnter_t> cb_lobbyEnter;
        internal Callback<LobbyDataUpdate_t> cb_lobbyUpdate;
        internal Callback<P2PSessionRequest_t> cb_p2p_session_request;
        internal Callback<LobbyChatUpdate_t> cb_lobby_update;

        public void CreateLobby()
        {
            var type = ModEntry.Preferences.LobbySettings.OpenToJoin 
                ? ELobbyType.k_ELobbyTypePublic : ELobbyType.k_ELobbyTypePrivate;
            SteamMatchmaking.CreateLobby(type, 8);
        }

        public void JoinLobby(Steamworks.CSteamID lobbyId)
        {
            LeaveLobby();
            SteamMatchmaking.JoinLobby(lobbyId);
        }

        public void LeaveLobby()
        {
            if (!LobbyId.HasValue) { return; }
            // if owner leaves its lobby,
            // give it to the 2nd person that joined instead of disconnecting everyone
            // update: this already is a feature by steam
            //if (AmILobbyOwner && LobbyPlayers.Count > 1)
            //{
            //    // does this work?
            //    // https://partner.steamgames.com/doc/api/ISteamMatchmaking#SetLobbyOwner
            //    SteamMatchmaking.SetLobbyOwner(LobbyId.Value, LobbyPlayers[1]);
            //}
            SteamMatchmaking.LeaveLobby(LobbyId.Value);
            Players.ForEach(x => x.IsDisposed = true);
            Players.Clear();
            LobbyPlayers.Clear();
            LobbyId = null;
        }

        public void UpdatePlayerState(CSteamID id, TrackData data)
        {
            var idx = Players.FindIndex(x => x.SteamId == id);
            if (idx == -1)
            {
                // this gives the name even if the target player is not our friend
                var name = SteamFriends.GetFriendPersonaName(id) ?? "";
                if (name.Trim() == "")
                {
                    name = Guid.NewGuid().ToString().Split('-')[0];
                }
                // custom color per player
                var color = data.colorIdx != 0 ? data.colorIdx : Players.Count() + 1;
                Players.Add(new GhostPlayer(name, id, color));
                idx = Players.Count - 1;
            }
            var player = Players[idx];
            player.ScreenIndex1 = data.screenIndex1;
            player.LevelId = data.levelId;
            player.tracker.Track(data);
        }

        public List<CSteamID> LobbyPlayers = new List<CSteamID>();
        public CSteamID? LobbyId;
        public CSteamID? LobbyOwner;
        public bool AmILobbyOwner => LobbyOwner == UserSteamId;

        public void UpdateLobbyMemberIds()
        {
            Debug.WriteLine($"[MP] Lobby members updated: {LobbyPlayers.Count} players");
            var _players = new List<CSteamID>();
            var memCnt = SteamMatchmaking.GetNumLobbyMembers(LobbyId.Value);
            for (var idx = 0; idx < memCnt; idx++)
            {
                var player = SteamMatchmaking.GetLobbyMemberByIndex(LobbyId.Value, idx);
                if (player != UserSteamId)
                {
                    Debug.WriteLine($" - {player.m_SteamID}");
                    _players.Add(player);
                }
            }
            LobbyPlayers = _players;
        }

        public void HandlePlayerLeft(CSteamID id)
        {
            Debug.WriteLine($"[MP] Player left {id}");
            UpdateLobbyMemberIds();
            Players.Where(x => x.SteamId == id).ToList().ForEach(x => x.IsDisposed = true);
            Players.RemoveAll(x => x.SteamId == id);
            LobbyPlayers.Remove(id);
        }

        public void HandlePlayerJoined(CSteamID id)
        {
            Debug.WriteLine($"[MP] Player joined {id}");
            UpdateLobbyMemberIds();
        }

        public void Init()
        {
            leaderBoard = new LeaderBoard();
            proximityPlayers = new ProximityPlayers();
            inviteInfo = new InviteInfo();
            emptyLobbyInviteInfo = new EmptyLobbyInviteInfo();

            cb_lobbyCreated = Callback<LobbyCreated_t>.Create((x) =>
            {
                Debug.WriteLine($"[MP] Lobby created {x.m_ulSteamIDLobby}");
                LobbyId = new Steamworks.CSteamID(x.m_ulSteamIDLobby);
                UpdateLobbyMemberIds();
            });

            cb_lobbyInvite = Callback<LobbyInvite_t>.Create((x) =>
            {
                Debug.WriteLine($"[MP] Invited to {x.m_ulSteamIDUser}'s lobby: {x.m_ulSteamIDLobby}");
                inviteInfo.AddInvite(new CSteamID(x.m_ulSteamIDUser), new CSteamID(x.m_ulSteamIDLobby));
            });

            cb_lobbyEnter = Callback<LobbyEnter_t>.Create((x) =>
            {
                Debug.WriteLine($"[MP] Lobby joined {x.m_ulSteamIDLobby}");
                LobbyId = new Steamworks.CSteamID(x.m_ulSteamIDLobby);
                UpdateLobbyMemberIds();
            });

            cb_lobbyUpdate = Callback<LobbyDataUpdate_t>.Create((x) =>
            {
                Debug.WriteLine($"[MP] Lobby updated {x.m_ulSteamIDLobby}");
                if (x.m_bSuccess != 1) { return; }
                LobbyOwner = SteamMatchmaking.GetLobbyOwner(LobbyId.Value);
            });

            cb_p2p_session_request = Callback<P2PSessionRequest_t>.Create((x) =>
            {
                if (LobbyPlayers.Contains(x.m_steamIDRemote))
                {
                    SteamNetworking.AcceptP2PSessionWithUser(x.m_steamIDRemote);
                }
                else
                {
                    Debug.WriteLine($"steam user {x.m_steamIDRemote.m_SteamID} wanted to create a p2p session but was not in the lobby");
                }
            });

            cb_lobby_update = Callback<LobbyChatUpdate_t>.Create((x) =>
            {
                Debug.WriteLine("Lobby update");
                var targetId = new CSteamID(x.m_ulSteamIDUserChanged);
                if (((Steamworks.EChatMemberStateChange)x.m_rgfChatMemberStateChange & (Steamworks.EChatMemberStateChange.k_EChatMemberStateChangeBanned | Steamworks.EChatMemberStateChange.k_EChatMemberStateChangeDisconnected | Steamworks.EChatMemberStateChange.k_EChatMemberStateChangeKicked | Steamworks.EChatMemberStateChange.k_EChatMemberStateChangeLeft)) != 0)
                {
                    HandlePlayerLeft(targetId);
                }
                else
                {
                    HandlePlayerJoined(targetId);
                }
            });

            var name = SteamFriends.GetPersonaName();
            Debug.WriteLine($"Player name is {name}");
            Debug.WriteLine($"Your steam id is : {Steamworks.SteamUser.GetSteamID()}");

            if (ModEntry.Preferences.LobbySettings.CreateLobbyOnLaunch)
            {
                MultiplayerManager.instance.CreateLobby();
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        public void Tick(float p_delta)
        {
            emptyLobbyInviteInfo.Tick();
            inviteInfo.Tick();
            SendUpdateToOthers();
            GetUpdates();
        }

        public void Draw()
        {
            proximityPlayers?.Draw();
            emptyLobbyInviteInfo.Draw();
            inviteInfo.Draw();
            MultiplayerManager.DrawPlayers();
            MultiplayerManager.instance._alreadyDrawedPlayers = false;
        }

        public static bool IsOnline()
        {
            return MultiplayerManager.instance.LobbyId.HasValue;
        }

        public static void ToggleOnline()
        {
            if (MultiplayerManager.IsOnline())
            {
                MultiplayerManager.instance.LeaveLobby();
            }
            else
            {
                MultiplayerManager.instance.CreateLobby();
            }
        }
    }
}
