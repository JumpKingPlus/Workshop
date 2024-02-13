using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JumpKingSaveStates
{
    public class Preferences : INotifyPropertyChanged
    {
        private float x = 231f;
        private float y = 302f;
        private uint screen = 1;

        public void SetSaveState(float x, float y, int screen)
        {
            PositionX = x;
            PositionY = y;

            if (uint.TryParse(screen.ToString(), out uint uScreen))
                Screen = uScreen;

            OnPropertyChanged();
        }

        public float PositionX
        {
            get => x;
            set
            {
                x = value;
            }
        }

        public float PositionY
        {
            get => y;
            set
            {
                y = value;
            }
        }

        public uint Screen
        {
            get => screen;
            set
            {
                screen = value;
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