using BehaviorTree;
using EntityComponent.BT;
using HarmonyLib;
using JumpKing;
using JumpKing.PauseMenu.BT;
using JumpKing.Util;
using JumpKingMultiplayer.Helpers;
using JumpKingMultiplayer.Models.Infos;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JumpKingMultiplayer.Models;

namespace JumpKingMultiplayer.Models.Infos
{
    internal class EmptyLobbyInviteInfo : Info
    {
        protected int STEPS = 4;
        protected float ON_SCREEN = 5f;
        protected float FADE_OUT = 1f;

        private BTsequencor sequence;
        private PauseNode m_pause;
        private FadeNode m_fade_out;

        void InitSequence()
        {
            m_fade_out = new FadeNode(FADE_OUT, FadeNode.Mode.FadeOut);
            m_pause = new PauseNode(ON_SCREEN);

            sequence = new BTsequencor();
            sequence.AddChild(m_pause);
            sequence.AddChild(m_fade_out);
        }

        public EmptyLobbyInviteInfo() : base()
        {
            InitSequence();
        }

        protected TextInfo text;
        protected Traverse textColorTraverse;

        protected override void Initialize()
        {
            var text_format = format;
            text_format.anchor_bounds.Width = 160;

            text = TextInfo.CreateOneFittedInfo(
                text_format,
                "To invite friends to your lobby, click F4!",
                Color.White * alpha,
                Game1.instance.contentManager.font.MenuFontSmall
            );
            textColorTraverse = Traverse.Create(text).Field("m_color");

            frame = new RunnableDisplayFrame(format, BTresult.Running, Color.Blue, alpha);
            frame.AddChild(text);
            frame.Initialize();
        }

        public void Tick()
        {
            if (!MultiplayerManager.IsOnline()) return;
            if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                SteamFriends.ActivateGameOverlayInviteDialog(MultiplayerManager.instance.LobbyId.Value);
                if (ModEntry.Preferences.Tips.InviteF4Tip)
                {
                    var tips = ModEntry.Preferences.Tips;
                    tips.InviteF4Tip = !tips.InviteF4Tip;
                    ModEntry.Preferences.Tips = tips;
                }
            }
        }

        public override void Draw()
        {
            if (!MultiplayerManager.IsOnline()) return;
            if (!ModEntry.Preferences.Tips.InviteF4Tip) return;

            byte alpha;
            if (m_pause.IsRunning())
            {
                alpha = byte.MaxValue;
            }
            else if (m_fade_out.IsRunning())
            {
                alpha = m_fade_out.AlphaByte;
            }
            else
            {
                return;
            }

            alpha = (byte)(((int)alpha * (int)(this.alpha * byte.MaxValue)) / (int)byte.MaxValue);

            // damn private field
            var color = textColorTraverse.GetValue<Color>();
            color.A = alpha;
            textColorTraverse.SetValue(color);

            frame.Alpha = (float)alpha / (float)byte.MaxValue;
            base.Draw();
        }

        public override void Run(TickData data)
        {
            // if you are in a lobby
            // AND
            // you havent already pressed F4 once (add a setting to preferences)

            if (!MultiplayerManager.IsOnline()) return;
            if (!ModEntry.Preferences.Tips.InviteF4Tip) return;
            if (sequence.last_result == BTresult.Success) return;

            sequence.Run(data);
            base.Run(data);
        }
    }
}
