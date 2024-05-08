using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JumpKingManager
{
    public partial class Manager : Form
    {
        private static string TitleBarText = "Jump King Manager v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        private float Px, Py;
        private Screen PScreen;
        private bool stillHolding = false;
        private Thread keyThread;

        public Manager()
        {
            InitializeComponent();

            Text = TitleBarText;
            foreach (Screen type in Enum.GetValues(typeof(Screen)))
            {
                if (type != Screen.Unknown1 && type != Screen.Unknown2 && type != Screen.Unknown3)
                {
                    cboSpecificLevel.Items.Add(type);
                }
            }

            keyThread = new Thread(CheckKeys);
            keyThread.IsBackground = true;
            keyThread.Start();
        }

        private const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.Style &= ~WS_SYSMENU;
                return myCp;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetAsyncKeyState(Keys vkey);
        private static bool IsKeyDown(Keys key)
        {
            return (GetAsyncKeyState(key) >> 15 & 1) == 1;
        }

        private void CheckKeys()
        {
            while (true)
            {
                try
                {
                    if (IsKeyDown(Keys.ControlKey) && IsKeyDown(Keys.S))
                    {
                        if (!stillHolding)
                        {
                            btnSave_Click(null, null);
                            stillHolding = true;
                        }
                    }
                    else if (IsKeyDown(Keys.ControlKey) && IsKeyDown(Keys.G))
                    {
                        if (!stillHolding)
                        {
                            btnLoadPosition_Click(null, null);
                            stillHolding = true;
                        }
                    }
                    else if (IsKeyDown(Keys.ControlKey) && IsKeyDown(Keys.X))
                    {
                        if (!stillHolding)
                        {
                            btnSpecificLevel_Click(null, null);
                            stillHolding = true;
                        }
                    }
                    else
                    {
                        stillHolding = false;
                    }
                }
                catch
                {
                }
                Thread.Sleep(16);
            }
        }

        private void TeleportToLevel(Screen screen)
        {
            try
            {
                switch (screen)
                {
                    case Screen.RedcrownWoods1: ModEntry.Behaviour.TeleportPlayer(screen, 231, 302); break;
                    case Screen.RedcrownWoods2: ModEntry.Behaviour.TeleportPlayer(screen, 340, -90); break;
                    case Screen.RedcrownWoods3: ModEntry.Behaviour.TeleportPlayer(screen, 205, -442); break;
                    case Screen.RedcrownWoods4: ModEntry.Behaviour.TeleportPlayer(screen, 139, -786); break;
                    case Screen.RedcrownWoods5: ModEntry.Behaviour.TeleportPlayer(screen, 148, -1154); break;
                    case Screen.ColossalDrain1: ModEntry.Behaviour.TeleportPlayer(screen, 251, -1498); break;
                    case Screen.ColossalDrain2: ModEntry.Behaviour.TeleportPlayer(screen, 412, -1842); break;
                    case Screen.ColossalDrain3: ModEntry.Behaviour.TeleportPlayer(screen, 211, -2202); break;
                    case Screen.ColossalDrain4: ModEntry.Behaviour.TeleportPlayer(screen, 70, -2610); break;
                    case Screen.ColossalDrain5: ModEntry.Behaviour.TeleportPlayer(screen, 382, -2938); break;
                    case Screen.FalseKingsKeep1: ModEntry.Behaviour.TeleportPlayer(screen, 340, -3282); break;
                    case Screen.FalseKingsKeep2: ModEntry.Behaviour.TeleportPlayer(screen, 352, -3642); break;
                    case Screen.FalseKingsKeep3: ModEntry.Behaviour.TeleportPlayer(screen, 414, -4002); break;
                    case Screen.FalseKingsKeep4: ModEntry.Behaviour.TeleportPlayer(screen, 255, -4442); break;
                    case Screen.Bargainburg1: ModEntry.Behaviour.TeleportPlayer(screen, 150, -4738); break;
                    case Screen.Bargainburg2: ModEntry.Behaviour.TeleportPlayer(screen, 55, -5114); break;
                    case Screen.Bargainburg3: ModEntry.Behaviour.TeleportPlayer(screen, 425, -5474); break;
                    case Screen.Bargainburg4: ModEntry.Behaviour.TeleportPlayer(screen, 43, -5850); break;
                    case Screen.Bargainburg5: ModEntry.Behaviour.TeleportPlayer(screen, 410, -6194); break;
                    case Screen.GreatFrontier1: ModEntry.Behaviour.TeleportPlayer(screen, 222, -6594); break;
                    case Screen.GreatFrontier2: ModEntry.Behaviour.TeleportPlayer(screen, 268, -6914); break;
                    case Screen.GreatFrontier3: ModEntry.Behaviour.TeleportPlayer(screen, 256, -7266); break;
                    case Screen.GreatFrontier4: ModEntry.Behaviour.TeleportPlayer(screen, 158, -7626); break;
                    case Screen.GreatFrontier5: ModEntry.Behaviour.TeleportPlayer(screen, 260, -7994); break;
                    case Screen.GreatFrontier6: ModEntry.Behaviour.TeleportPlayer(screen, 100, -8362); break;
                    case Screen.WindsweptBluff1: ModEntry.Behaviour.TeleportPlayer(screen, 216, -8714); break;
                    case Screen.StormwallPass1: ModEntry.Behaviour.TeleportPlayer(screen, 223, -9074); break;
                    case Screen.StormwallPass2: ModEntry.Behaviour.TeleportPlayer(screen, 370, -9466); break;
                    case Screen.StormwallPass3: ModEntry.Behaviour.TeleportPlayer(screen, 152, -9778); break;
                    case Screen.StormwallPass4: ModEntry.Behaviour.TeleportPlayer(screen, 169, -10130); break;
                    case Screen.StormwallPass5: ModEntry.Behaviour.TeleportPlayer(screen, 394, -10498); break;
                    case Screen.StormwallPass6: ModEntry.Behaviour.TeleportPlayer(screen, 38, -10858); break;
                    case Screen.ChapelPerilous1: ModEntry.Behaviour.TeleportPlayer(screen, 426, -11202); break;
                    case Screen.ChapelPerilous2: ModEntry.Behaviour.TeleportPlayer(screen, 202, -11618); break;
                    case Screen.ChapelPerilous3: ModEntry.Behaviour.TeleportPlayer(screen, 144, -11922); break;
                    case Screen.ChapelPerilous4: ModEntry.Behaviour.TeleportPlayer(screen, 312, -12282); break;
                    case Screen.BlueRuin1: ModEntry.Behaviour.TeleportPlayer(screen, 410, -12658); break;
                    case Screen.BlueRuin2: ModEntry.Behaviour.TeleportPlayer(screen, 445, -13026); break;
                    case Screen.BlueRuin3: ModEntry.Behaviour.TeleportPlayer(screen, 360, -13378); break;
                    case Screen.TheTower1: ModEntry.Behaviour.TeleportPlayer(screen, 435, -13722); break;
                    case Screen.TheTower2: ModEntry.Behaviour.TeleportPlayer(screen, 340, -14082); break;
                    case Screen.TheTower3: ModEntry.Behaviour.TeleportPlayer(screen, 125, -14442); break;
                    case Screen.MainBabe: ModEntry.Behaviour.TeleportPlayer(screen, 150, -14802); break;
                    case Screen.Bargainburg6: ModEntry.Behaviour.TeleportPlayer(screen, 171, -15570); break;
                    case Screen.Bargainburg7: ModEntry.Behaviour.TeleportPlayer(screen, 93, -15898); break;
                    case Screen.BrightcrownWoods1: ModEntry.Behaviour.TeleportPlayer(screen, 377, -16274); break;
                    case Screen.BrightcrownWoods2: ModEntry.Behaviour.TeleportPlayer(screen, 155, -16650); break;
                    case Screen.BrightcrownWoods3: ModEntry.Behaviour.TeleportPlayer(screen, 255, -17002); break;
                    case Screen.BrightcrownWoods4: ModEntry.Behaviour.TeleportPlayer(screen, 269, -17338); break;
                    case Screen.BrightcrownWoods5: ModEntry.Behaviour.TeleportPlayer(screen, 146, -17714); break;
                    case Screen.BrightcrownWoods6: ModEntry.Behaviour.TeleportPlayer(screen, 38, -18074); break;
                    case Screen.ColossalDungeon1: ModEntry.Behaviour.TeleportPlayer(screen, 295, -18426); break;
                    case Screen.ColossalDungeon2: ModEntry.Behaviour.TeleportPlayer(screen, 60, -18754); break;
                    case Screen.ColossalDungeon3: ModEntry.Behaviour.TeleportPlayer(screen, 440, -19146); break;
                    case Screen.ColossalDungeon4: ModEntry.Behaviour.TeleportPlayer(screen, 305, -19482); break;
                    case Screen.ColossalDungeon5: ModEntry.Behaviour.TeleportPlayer(screen, 405, -19882); break;
                    case Screen.ColossalDungeon6: ModEntry.Behaviour.TeleportPlayer(screen, 78, -20218); break;
                    case Screen.ColossalDungeon7: ModEntry.Behaviour.TeleportPlayer(screen, 105, -20586); break;
                    case Screen.FalseKingsCastle1: ModEntry.Behaviour.TeleportPlayer(screen, 105, -20922); break;
                    case Screen.FalseKingsCastle2: ModEntry.Behaviour.TeleportPlayer(screen, 242, -21370); break;
                    case Screen.FalseKingsCastle3: ModEntry.Behaviour.TeleportPlayer(screen, 220, -21690); break;
                    case Screen.FalseKingsCastle4: ModEntry.Behaviour.TeleportPlayer(screen, 210, -22042); break;
                    case Screen.Underburg1: ModEntry.Behaviour.TeleportPlayer(screen, 415, -22362); break;
                    case Screen.Underburg2: ModEntry.Behaviour.TeleportPlayer(screen, 370, -22746); break;
                    case Screen.Underburg3: ModEntry.Behaviour.TeleportPlayer(screen, 30, -23082); break;
                    case Screen.Underburg4: ModEntry.Behaviour.TeleportPlayer(screen, 64, -23474); break;
                    case Screen.Underburg5: ModEntry.Behaviour.TeleportPlayer(screen, 400, -23810); break;
                    case Screen.Underburg6: ModEntry.Behaviour.TeleportPlayer(screen, 115, -24194); break;
                    case Screen.Underburg7: ModEntry.Behaviour.TeleportPlayer(screen, 50, -24522); break;
                    case Screen.LostFrontier1: ModEntry.Behaviour.TeleportPlayer(screen, 242, -24898); break;
                    case Screen.LostFrontier2: ModEntry.Behaviour.TeleportPlayer(screen, 412, -25274); break;
                    case Screen.LostFrontier3: ModEntry.Behaviour.TeleportPlayer(screen, 94, -25602); break;
                    case Screen.LostFrontier4: ModEntry.Behaviour.TeleportPlayer(screen, 280, -26002); break;
                    case Screen.LostFrontier5: ModEntry.Behaviour.TeleportPlayer(screen, 40, -26354); break;
                    case Screen.LostFrontier6: ModEntry.Behaviour.TeleportPlayer(screen, 355, -26714); break;
                    case Screen.LostFrontier7: ModEntry.Behaviour.TeleportPlayer(screen, 435, -27042); break;
                    case Screen.HiddenKingdom1: ModEntry.Behaviour.TeleportPlayer(screen, 360, -27402); break;
                    case Screen.HiddenKingdom2: ModEntry.Behaviour.TeleportPlayer(screen, 266, -27778); break;
                    case Screen.HiddenKingdom3: ModEntry.Behaviour.TeleportPlayer(screen, 170, -28122); break;
                    case Screen.HiddenKingdom4: ModEntry.Behaviour.TeleportPlayer(screen, 327, -28506); break;
                    case Screen.HiddenKingdom5: ModEntry.Behaviour.TeleportPlayer(screen, 260, -28842); break;
                    case Screen.HiddenKingdom6: ModEntry.Behaviour.TeleportPlayer(screen, 254, -29266); break;
                    case Screen.BlackSanctum1: ModEntry.Behaviour.TeleportPlayer(screen, 210, -29578); break;
                    case Screen.BlackSanctum2: ModEntry.Behaviour.TeleportPlayer(screen, 278, -29922); break;
                    case Screen.BlackSanctum3: ModEntry.Behaviour.TeleportPlayer(screen, 190, -30298); break;
                    case Screen.BlackSanctum4: ModEntry.Behaviour.TeleportPlayer(screen, 276, -30642); break;
                    case Screen.BlackSanctum5: ModEntry.Behaviour.TeleportPlayer(screen, 38, -31034); break;
                    case Screen.BlackSanctum6: ModEntry.Behaviour.TeleportPlayer(screen, 30, -31362); break;
                    case Screen.DeepRuin1: ModEntry.Behaviour.TeleportPlayer(screen, 170, -31786); break;
                    case Screen.DeepRuin2: ModEntry.Behaviour.TeleportPlayer(screen, 320, -32090); break;
                    case Screen.DeepRuin3: ModEntry.Behaviour.TeleportPlayer(screen, 340, -32450); break;
                    case Screen.DeepRuin4: ModEntry.Behaviour.TeleportPlayer(screen, 138, -32858); break;
                    case Screen.DeepRuin5: ModEntry.Behaviour.TeleportPlayer(screen, 106, -33210); break;
                    case Screen.TheDarkTower1: ModEntry.Behaviour.TeleportPlayer(screen, 288, -33554); break;
                    case Screen.TheDarkTower2: ModEntry.Behaviour.TeleportPlayer(screen, 310, -33882); break;
                    case Screen.TheDarkTower3: ModEntry.Behaviour.TeleportPlayer(screen, 412, -34258); break;
                    case Screen.TheDarkTower4: ModEntry.Behaviour.TeleportPlayer(screen, 230, -34626); break;
                    case Screen.TheDarkTower5: ModEntry.Behaviour.TeleportPlayer(screen, 198, -34962); break;
                    case Screen.NewBabe: ModEntry.Behaviour.TeleportPlayer(screen, 140, -35314); break;
                    case Screen.PhilosophersForest1: ModEntry.Behaviour.TeleportPlayer(screen, 16, -55714); break;
                    case Screen.PhilosophersForest2: ModEntry.Behaviour.TeleportPlayer(screen, 289, -55834); break;
                    case Screen.PhilosophersForest3: ModEntry.Behaviour.TeleportPlayer(screen, 255, -56258); break;
                    case Screen.PhilosophersForest4: ModEntry.Behaviour.TeleportPlayer(screen, 108, -56570); break;
                    case Screen.PhilosophersForest5: ModEntry.Behaviour.TeleportPlayer(screen, 313, -56938); break;
                    case Screen.Hole1: ModEntry.Behaviour.TeleportPlayer(screen, 32, -58434); break;
                    case Screen.Hole2: ModEntry.Behaviour.TeleportPlayer(screen, 227, -58324); break;
                    case Screen.Hole3: ModEntry.Behaviour.TeleportPlayer(screen, 226, -57947); break;
                    case Screen.Hole4: ModEntry.Behaviour.TeleportPlayer(screen, 237, -57290); break;
                    case Screen.Bog7: ModEntry.Behaviour.TeleportPlayer(screen, 415, -38226); break;
                    case Screen.Bog6: ModEntry.Behaviour.TeleportPlayer(screen, 232, -37882); break;
                    case Screen.Bog5: ModEntry.Behaviour.TeleportPlayer(screen, 378, -37514); break;
                    case Screen.Bog4: ModEntry.Behaviour.TeleportPlayer(screen, 83, -37130); break;
                    case Screen.Bog3: ModEntry.Behaviour.TeleportPlayer(screen, 50, -36802); break;
                    case Screen.Bog2: ModEntry.Behaviour.TeleportPlayer(screen, 156, -36402); break;
                    case Screen.Bog1: ModEntry.Behaviour.TeleportPlayer(screen, 79, -36074); break;
                    case Screen.MouldingManor1: ModEntry.Behaviour.TeleportPlayer(screen, 183, -38602); break;
                    case Screen.MouldingManor2: ModEntry.Behaviour.TeleportPlayer(screen, 187, -38970); break;
                    case Screen.MouldingManor3: ModEntry.Behaviour.TeleportPlayer(screen, 280, -39298); break;
                    case Screen.MouldingManor4: ModEntry.Behaviour.TeleportPlayer(screen, 295, -39658); break;
                    case Screen.MouldingManor5: ModEntry.Behaviour.TeleportPlayer(screen, 205, -40026); break;
                    case Screen.MouldingManor6: ModEntry.Behaviour.TeleportPlayer(screen, 239, -40370); break;
                    case Screen.MouldingManor7: ModEntry.Behaviour.TeleportPlayer(screen, 140, -40730); break;
                    case Screen.MouldingManor8: ModEntry.Behaviour.TeleportPlayer(screen, 310, -41082); break;
                    case Screen.Bugstalk1: ModEntry.Behaviour.TeleportPlayer(screen, 163, -41442); break;
                    case Screen.Bugstalk2: ModEntry.Behaviour.TeleportPlayer(screen, 300, -41810); break;
                    case Screen.Bugstalk3: ModEntry.Behaviour.TeleportPlayer(screen, 278, -42242); break;
                    case Screen.Bugstalk4: ModEntry.Behaviour.TeleportPlayer(screen, 195, -42522); break;
                    case Screen.Bugstalk5: ModEntry.Behaviour.TeleportPlayer(screen, 210, -42898); break;
                    case Screen.Bugstalk6: ModEntry.Behaviour.TeleportPlayer(screen, 178, -43290); break;
                    case Screen.Bugstalk7: ModEntry.Behaviour.TeleportPlayer(screen, 285, -43634); break;
                    case Screen.HouseOfNineLives1: ModEntry.Behaviour.TeleportPlayer(screen, 329, -43970); break;
                    case Screen.HouseOfNineLives2: ModEntry.Behaviour.TeleportPlayer(screen, 340, -44330); break;
                    case Screen.HouseOfNineLives3: ModEntry.Behaviour.TeleportPlayer(screen, 365, -44674); break;
                    case Screen.HouseOfNineLives4: ModEntry.Behaviour.TeleportPlayer(screen, 233, -45042); break;
                    case Screen.HouseOfNineLives5: ModEntry.Behaviour.TeleportPlayer(screen, 151, -45434); break;
                    case Screen.HouseOfNineLives6: ModEntry.Behaviour.TeleportPlayer(screen, 198, -45810); break;
                    case Screen.HouseOfNineLives7: ModEntry.Behaviour.TeleportPlayer(screen, 153, -46130); break;
                    case Screen.PhantomTower1: ModEntry.Behaviour.TeleportPlayer(screen, 333, -46530); break;
                    case Screen.PhantomTower2: ModEntry.Behaviour.TeleportPlayer(screen, 300, -46906); break;
                    case Screen.PhantomTower3: ModEntry.Behaviour.TeleportPlayer(screen, 179, -47226); break;
                    case Screen.PhantomTower4: ModEntry.Behaviour.TeleportPlayer(screen, 182, -47570); break;
                    case Screen.PhantomTower5: ModEntry.Behaviour.TeleportPlayer(screen, 245, -47946); break;
                    case Screen.PhantomTower6: ModEntry.Behaviour.TeleportPlayer(screen, 137, -48290); break;
                    case Screen.PhantomTower7: ModEntry.Behaviour.TeleportPlayer(screen, 264, -48642); break;
                    case Screen.PhantomTower8: ModEntry.Behaviour.TeleportPlayer(screen, 159, -49002); break;
                    case Screen.PhantomTower9: ModEntry.Behaviour.TeleportPlayer(screen, 444, -49362); break;
                    case Screen.HaltedRuin1: ModEntry.Behaviour.TeleportPlayer(screen, 204, -49794); break;
                    case Screen.HaltedRuin2: ModEntry.Behaviour.TeleportPlayer(screen, 32, -50130); break;
                    case Screen.HaltedRuin3: ModEntry.Behaviour.TeleportPlayer(screen, 293, -50514); break;
                    case Screen.HaltedRuin4: ModEntry.Behaviour.TeleportPlayer(screen, 302, -50898); break;
                    case Screen.HaltedRuin5: ModEntry.Behaviour.TeleportPlayer(screen, 166, -51234); break;
                    case Screen.HaltedRuin6: ModEntry.Behaviour.TeleportPlayer(screen, 27, -51562); break;
                    case Screen.HaltedRuin7: ModEntry.Behaviour.TeleportPlayer(screen, 32, -51954); break;
                    case Screen.HaltedRuin8: ModEntry.Behaviour.TeleportPlayer(screen, 177, -52274); break;
                    case Screen.TowerOfAntumbra1: ModEntry.Behaviour.TeleportPlayer(screen, 292, -52626); break;
                    case Screen.TowerOfAntumbra2: ModEntry.Behaviour.TeleportPlayer(screen, 373, -53010); break;
                    case Screen.TowerOfAntumbra3: ModEntry.Behaviour.TeleportPlayer(screen, 366, -53346); break;
                    case Screen.TowerOfAntumbra4: ModEntry.Behaviour.TeleportPlayer(screen, 383, -53602); break;
                    case Screen.TowerOfAntumbra5: ModEntry.Behaviour.TeleportPlayer(screen, 93, -54130); break;
                    case Screen.TowerOfAntumbra6: ModEntry.Behaviour.TeleportPlayer(screen, 165, -54522); break;
                    case Screen.GhostBabe: ModEntry.Behaviour.TeleportPlayer(screen, 302, -54770); break;
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString(), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        #region buttons
        private void btnRedcrownWoods_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.RedcrownWoods1);
        }
        private void btnColossalDrain_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.ColossalDrain1);
        }
        private void btnFalseKingKeep_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.FalseKingsKeep1);
        }
        private void btnBargainburg_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.Bargainburg1);
        }
        private void btnGreatFrontier_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.GreatFrontier1);
        }
        private void btnStormwallPass_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.WindsweptBluff1);
        }
        private void btnChapelPerilous_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.ChapelPerilous1);
        }
        private void btnBlueRuin_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.BlueRuin1);
        }
        private void btnTower_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.TheTower1);
        }
        private void btnBrightcrownWoods_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.BrightcrownWoods1);
        }
        private void btnColossalDungeon_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.ColossalDungeon1);
        }
        private void btnFalseKingsCastle_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.FalseKingsCastle1);
        }
        private void btnUnderburg_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.Underburg1);
        }
        private void btnLostFrontier_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.LostFrontier1);
        }
        private void btnHiddenKingdom_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.HiddenKingdom1);
        }
        private void btnBlackSanctum_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.BlackSanctum1);
        }
        private void btnDeepRuin_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.DeepRuin1);
        }
        private void btnDarkTower_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.TheDarkTower1);
        }
        private void btnPhilosophersForest_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.PhilosophersForest2);
        }
        private void btnBog_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.Bog1);
        }
        private void btnMouldingManor_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.MouldingManor1);
        }
        private void btnBugstalk_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.Bugstalk1);
        }
        private void btnHouseOfNineLives_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.HouseOfNineLives1);
        }
        private void btnPhantomTower_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.PhantomTower1);
        }
        private void btnHaltedRuin_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.HaltedRuin1);
        }
        private void btnTowerOfAntumbra_Click(object sender, EventArgs e)
        {
            TeleportToLevel(Screen.TowerOfAntumbra1);
        }
        #endregion

        private void btnSpecificLevel_Click(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((EventHandler)btnSpecificLevel_Click);
                return;
            }

            if (cboSpecificLevel.SelectedItem != null)
            {
                TeleportToLevel((Screen)cboSpecificLevel.SelectedItem);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((EventHandler)btnSave_Click);
                    return;
                }

                Px = ModEntry.Behaviour.PlayerX();
                Py = ModEntry.Behaviour.PlayerY();
                PScreen = ModEntry.Behaviour.PlayerScreen();
                lblSavedPosition.Text = $"{PScreen.ToString()}\r\n({Px.ToString("0.00")}, {Py.ToString("0.00")})";
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btnCoordinates_Click(object sender, EventArgs eargs)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((EventHandler)btnCoordinates_Click);
                    return;
                }

                float x = (float)Convert.ToDouble(xAxis.Text);
                bool xValid = float.TryParse(xAxis.Text.ToString(), out x);
                float y = (float)Convert.ToDouble(yAxis.Text);
                bool yValid = float.TryParse(yAxis.Text.ToString(), out y);
                if (x == 0f && y == 0f) { return; }

                if (xValid && yValid)
                {
                    int screen = (int)Math.Ceiling(y / -360);
                    ModEntry.Behaviour.TeleportPlayer((Screen)screen, x, y);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btnLoadPosition_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((EventHandler)btnLoadPosition_Click);
                    return;
                }

                if (Px == 0 && Py == 0) { return; }
                ModEntry.Behaviour.TeleportPlayer(PScreen, Px, Py);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}
