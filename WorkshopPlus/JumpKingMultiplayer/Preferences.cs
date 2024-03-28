using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingMultiplayer
{
    public class Preferences : INotifyPropertyChanged
    {
        private bool _isLeaderboardEnabled = true;
        private bool _isProximityPlayersEnabled = true;
        private EGhostPlayerDisplayType _ghostPlayerDisplayType = EGhostPlayerDisplayType.GhostPlayerAndLabel;
        private LobbySettings lobbySettings = LobbySettings.Default;
        private Tips tips = Tips.Default;
        private float _ghostPlayerOpacity = 0.6f;

        /// <summary>
        /// This is for disabling the leaderboard completely.
        /// </summary>
        public bool IsLeaderboardEnabled
        {
            get => _isLeaderboardEnabled;
            set
            {
                _isLeaderboardEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsProximityPlayersEnabled 
        { 
            get => _isProximityPlayersEnabled; 
            set
            {
                _isProximityPlayersEnabled = value;
                OnPropertyChanged();
            } 
        }

        public EGhostPlayerDisplayType GhostPlayerDisplayType
        {
            get => _ghostPlayerDisplayType;
            set
            {
                _ghostPlayerDisplayType = value;
                OnPropertyChanged();
            }
        }

        public float GhostPlayerOpacity
        {
            get => _ghostPlayerOpacity;
            set
            {
                _ghostPlayerOpacity = value;
                OnPropertyChanged();
            }
        }

        public LobbySettings LobbySettings
        {
            get => lobbySettings;
            set
            {
                lobbySettings = value;
                OnPropertyChanged();
            }
        }

        public Tips Tips
        {
            get => tips;
            set
            {
                tips = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public struct Tips
    {
        public bool InviteF4Tip { get; set; }

        public static Tips Default = new Tips
        {
            InviteF4Tip = true
        };
    }

    public struct LobbySettings
    {
        public bool OpenToJoin { get; set; }
        public bool CreateLobbyOnLaunch { get; set; }
        public int PersonalColor { get; set; }

        public static LobbySettings Default = new LobbySettings
        {
            OpenToJoin = true,
            CreateLobbyOnLaunch = false,
            PersonalColor = 0
        };
    }
}
