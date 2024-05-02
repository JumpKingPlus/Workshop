using JumpKing.Controller;
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
        [XmlIgnore]
        const int MAX_QUEUE_SIZE = 4;

        private bool _isEnabled = false;
        private ConcurrentDropoutQueue<SaveState> saveStates = 
            new ConcurrentDropoutQueue<SaveState>(MAX_QUEUE_SIZE, SaveState.Default);

        private Dictionary<EBinding, int[]> keyBinds = new Dictionary<EBinding, int[]>()
        {
            { EBinding.SavePos, new int[] { (int)Keys.Insert } },
            { EBinding.LoadPos, new int[] { (int)Keys.Home } },
            { EBinding.DeletePos, new int[] { (int)Keys.Delete } },
        };

        public bool TryAddSaveState(float x, float y, int screen)
        {
            if (!uint.TryParse(screen.ToString(), out uint uScreen))
                return false;

            var saveState = new SaveState(x, y, uScreen);
            if (!SaveStates.IsEmpty)
            {
                if (SaveStates.Last().Equals(saveState))
                    return false;
            }

            SaveStates.Enqueue(saveState);
            OnPropertyChanged();
            return true;
        }

        public bool DeleteRecentSaveState()
        {
            var has_deleted_smth = SaveStates.Count >= 1;
            //SaveStates.ToList().RemoveAt(SaveStates.ToList().Count - 1);
            if (has_deleted_smth)
            {
                var temp_list = SaveStates.ToList();
                temp_list.RemoveAt(SaveStates.Count - 1);
                SaveStates = new ConcurrentDropoutQueue<SaveState>(MAX_QUEUE_SIZE, temp_list.ToArray());
            }
            return has_deleted_smth;
        }

        public void ForceUpdate() => OnPropertyChanged();

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

        [XmlIgnore]
        public ConcurrentDropoutQueue<SaveState> SaveStates
        {
            get => saveStates;
            set
            {
                saveStates = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
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

        public SaveState[] Saves
        {
            get => saveStates.ToArray();
            set
            {
                saveStates = new ConcurrentDropoutQueue<SaveState>(MAX_QUEUE_SIZE, value);
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

    public struct SaveState
    {
        public SaveState(float x, float y, uint screen)
        {
            X = x;
            Y = y;
            Screen = screen;
        }

        public float X;
        public float Y;
        public uint Screen;

        public static SaveState Default => new SaveState(231, 302, 1);
    }

    public struct Binding
    {
        public Binding(KeyValuePair<EBinding, int[]> kvp) : this(kvp.Key, kvp.Value) { }
        public Binding(EBinding keyName, int[] actualKeys)
        {
            Bind = keyName;
            Keys = actualKeys;
        }

        public EBinding Bind;
        public int[] Keys;
    }
}