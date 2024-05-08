using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JumpKingManager
{
    public class Preferences : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set 
            { 
                _isEnabled = value; 
                OnPropertyChanged(nameof(IsEnabled));
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