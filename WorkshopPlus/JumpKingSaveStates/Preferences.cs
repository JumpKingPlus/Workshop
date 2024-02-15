using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace JumpKingSaveStates
{
    public class Preferences : INotifyPropertyChanged
    {
        private float x = 231f;
        private float y = 302f;
        private uint screen = 1;
        private Dictionary<EBinding, int[]> keyBinds = new Dictionary<EBinding, int[]>()
        {
            { EBinding.SavePos, new int[] { (int)Keys.Insert } },
            { EBinding.LoadPos, new int[] { (int)Keys.Home } }
        };

        public void SetSaveState(float x, float y, int screen)
        {
            PositionX = x;
            PositionY = y;

            if (uint.TryParse(screen.ToString(), out uint uScreen))
                Screen = uScreen;

            OnPropertyChanged();
        }

        public void ForceUpdate() => OnPropertyChanged();

        public float PositionX
        {
            get => x;
            set => x = value;
        }

        public float PositionY
        {
            get => y;
            set => y = value;
        }

        public uint Screen
        {
            get => screen;
            set => screen = value;
        }

        [XmlIgnore]
        public Dictionary<EBinding, int[]> KeyBindings
        {
            get => keyBinds;
            set
            {
                keyBinds = value;
                OnPropertyChanged();
            }
        }

        public Binding[] Bindings
        {
            get
            {
                // ugly ass code
                List<Binding> bindings = new List<Binding>();
                keyBinds.ToList().ForEach((kvp) =>
                {
                    bindings.Add(new Binding(kvp));
                });
                return bindings.ToArray();
            }
            set => keyBinds = value.ToDictionary(x => x.Bind, x => x.Keys);
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public struct Binding
    {
        public Binding(KeyValuePair<EBinding, int[]> kvp)
        {
            Bind = kvp.Key;
            Keys = kvp.Value;
        }

        public EBinding Bind;
        public int[] Keys;
    }
}