using BehaviorTree;
using HarmonyLib;
using JumpKing.PauseMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using JumpKingMultiplayer.Menu.DisplayFrames;
using JumpKingMultiplayer.Menu;
using JumpKingMultiplayer.Menu.DisplayFrames;
using JumpKingMultiplayer.Helpers;
using JumpKingMultiplayer.Extensions;

namespace JumpKingMultiplayer.Helpers
{
    public class DynamicDisplayFrame : RunnableDisplayFrame, INotifyPropertyChanged
    {
        public DynamicDisplayFrame(GuiFormat p_format, BTresult p_static_result, float p_alpha)
            : base(p_format, p_static_result, Color.White, p_alpha)
        {
        }

        public override BTresult Run(TickData p_data)
        {
            var bt = base.Run(p_data);
            Initialize();
            GetBounds();
            return bt;
        }

        private void GetBounds()
        {
            var tFrame = Traverse.Create(this);
            var new_bounds = tFrame.Field("m_bounds").GetValue<Rectangle>();
            
            if (new_bounds != null)
            {
                var format = tFrame.Field("m_format").GetValue<GuiFormat>();
                new_bounds.Width -= format.all_margin * 2;
                
                if (new_bounds != last_bounds)
                {
                    Debug.WriteLine($"[SIZE] [FRAME] {new_bounds.Width} {new_bounds.Height} {new_bounds.X} {new_bounds.Y}");
                    Bounds = new_bounds;
                }
            }
        }

        private Rectangle last_bounds;

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public Rectangle Bounds
        {
            get => last_bounds;
            set
            {
                last_bounds = value;
                OnPropertyChanged();
            }
        }
    }
}
