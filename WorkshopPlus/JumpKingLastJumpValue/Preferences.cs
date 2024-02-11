using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JumpKingLastJumpValue
{
    public class Preferences : INotifyPropertyChanged
    {
        public bool IsEnabled
        {
            get => Properties.Settings.Default.IsEnabled;
            set
            {
                Properties.Settings.Default.IsEnabled = value;
                OnPropertyChanged();
            }
        }

        public ELastJumpDisplayType DisplayType
        {
            get => Properties.Settings.Default.DisplayType;
            set
            {
                Properties.Settings.Default.DisplayType = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
