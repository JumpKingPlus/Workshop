using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JumpKingLevelPercentage
{
    public class Preferences : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private ELevelPercentageDisplayType _displayType = ELevelPercentageDisplayType.Percentage;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public ELevelPercentageDisplayType DisplayType
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
