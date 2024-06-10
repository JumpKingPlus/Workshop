using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JumpKingLastJumpValue
{
    public class Preferences : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private ELastJumpDisplayType _displayType = ELastJumpDisplayType.Percentage;
        private bool _showGauge = false;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public ELastJumpDisplayType DisplayType
        {
            get => _displayType;
            set
            {
                _displayType = value;
                OnPropertyChanged();
            }
        }

        public bool ShowGauge
        {
            get => _showGauge;
            set
            {
                _showGauge = value;
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
}
