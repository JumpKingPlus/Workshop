using System.ComponentModel;
using System.Runtime.CompilerServices;
using JumpKing;

namespace JumpKingPlayerCoordinates
{
    public class Preferences : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private Coordinates _displayType = Coordinates.Absolute;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public Coordinates DisplayType
        {
            get => _displayType;
            set
            {
                _displayType = value;
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