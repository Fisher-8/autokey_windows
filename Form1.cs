using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AutoKey_Windows
{
    public partial class fProgram : Form
    {
        [DllImport("user32")]
        static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport("user32")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vKey);

        [DllImport("user32")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32")]
        static extern void PostMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        private bool bConfig = false, bActive = false;
        private int i, iDelay1, iDelay2, iDelay3, iDelay4, iDelay5, iDelay6, iRDelay;
        private char cKey1, cKey2, cKey3, cKey4, cKey5, cKey6, cTKey;
        private const int hHotKey = 0x0312, iHotKey1 = 31196, iHotKey2 = 31197;
        private System.Timers.Timer timerRepeat;
        private IntPtr iHandle;
        private UserSettings userSettings;
        private Overlay1 overlay1;

        public fProgram()
        {
            InitializeComponent();
        }

        private void fProgram_Load(object sender, EventArgs e)
        {
            CheckProperties();
            SetTimer();
            //RegisterHotKey(F1)
            RegisterHotKey(this.Handle, iHotKey1, 0, 0x70);
            //Overlay
            overlay1 = new Overlay1();
            overlay1.Owner = this;
        }

        private void fProgram_Close(object sender, EventArgs e)
        {
            //UnRegisterHotKey(F1)
            UnregisterHotKey(this.Handle, iHotKey1);
        }

        //Input
        private void fKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(e.KeyChar == Convert.ToChar(Keys.Back) || Regex.IsMatch(e.KeyChar.ToString(), @"^[0-9A-Z]"));
        }

        private void fDigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(e.KeyChar == Convert.ToChar(Keys.Back) || char.IsDigit(e.KeyChar));
        }

        private void fButton_Click(object sender, EventArgs e)
        {
            SetConfig(bConfig);
        }

        //Check
        private static bool CheckBehavior(string s)
        {
            return Regex.IsMatch(s, @"^[0-2]") && s.Length == 1;
        }

        private static bool CheckKey(string s)
        {
            return Regex.IsMatch(s, @"^[0-9A-Z]") && s.Length == 1;
        }

        private static bool CheckDelay(string s, int l)
        {
            return s.All(Char.IsDigit) && s.Length <= l;
        }

        private void CheckProperties()
        {
            userSettings = UserSettings.Load();
            fUse1.Checked = userSettings.bUse1;
            fUse2.Checked = userSettings.bUse2;
            fUse3.Checked = userSettings.bUse3;
            fUse4.Checked = userSettings.bUse4;
            fUse5.Checked = userSettings.bUse5;
            fUse6.Checked = userSettings.bUse6;
            if (CheckKey(userSettings.sInput1)) { fKey1.Text = userSettings.sInput1; } else { fKey1.Text = "1"; }
            if (CheckKey(userSettings.sInput2)) { fKey2.Text = userSettings.sInput2; } else { fKey2.Text = "2"; }
            if (CheckKey(userSettings.sInput3)) { fKey3.Text = userSettings.sInput3; } else { fKey3.Text = "3"; }
            if (CheckKey(userSettings.sInput4)) { fKey4.Text = userSettings.sInput4; } else { fKey4.Text = "4"; }
            if (CheckKey(userSettings.sInput5)) { fKey5.Text = userSettings.sInput5; } else { fKey5.Text = "5"; }
            if (CheckKey(userSettings.sInput6)) { fKey6.Text = userSettings.sInput6; } else { fKey6.Text = "6"; }
            if (CheckDelay(userSettings.sDelay1, 4)) { fDelay1.Text = userSettings.sDelay1; } else { fDelay1.Text = "50"; }
            if (CheckDelay(userSettings.sDelay2, 4)) { fDelay2.Text = userSettings.sDelay2; } else { fDelay2.Text = "50"; }
            if (CheckDelay(userSettings.sDelay3, 4)) { fDelay3.Text = userSettings.sDelay3; } else { fDelay3.Text = "50"; }
            if (CheckDelay(userSettings.sDelay4, 4)) { fDelay4.Text = userSettings.sDelay4; } else { fDelay4.Text = "50"; }
            if (CheckDelay(userSettings.sDelay5, 4)) { fDelay5.Text = userSettings.sDelay5; } else { fDelay5.Text = "50"; }
            if (CheckDelay(userSettings.sDelay6, 4)) { fDelay6.Text = userSettings.sDelay6; } else { fDelay6.Text = "50"; }
            if (CheckDelay(userSettings.sDelayRepeat, 5)) { fRepeatDelay.Text = userSettings.sDelayRepeat; } else { fRepeatDelay.Text = "1000"; }
            if (CheckKey(userSettings.sHotKey)) { fToggleKey.Text = userSettings.sHotKey; } else { fToggleKey.Text = "E"; }
            if (CheckBehavior(userSettings.sBehavior)) { fBehavBox.SelectedIndex = int.Parse(userSettings.sBehavior); } else { fBehavBox.SelectedIndex = 0; }
        }

        //Save
        private void SaveProperties()
        {
            userSettings.bUse1 = fUse1.Checked;
            userSettings.bUse2 = fUse2.Checked;
            userSettings.bUse3 = fUse3.Checked;
            userSettings.bUse4 = fUse4.Checked;
            userSettings.bUse5 = fUse5.Checked;
            userSettings.bUse6 = fUse6.Checked;
            userSettings.sInput1 = fKey1.Text;
            userSettings.sInput2 = fKey2.Text;
            userSettings.sInput3 = fKey3.Text;
            userSettings.sInput4 = fKey4.Text;
            userSettings.sInput5 = fKey5.Text;
            userSettings.sInput6 = fKey6.Text;
            userSettings.sDelay1 = fDelay1.Text;
            userSettings.sDelay2 = fDelay2.Text;
            userSettings.sDelay3 = fDelay3.Text;
            userSettings.sDelay4 = fDelay4.Text;
            userSettings.sDelay5 = fDelay5.Text;
            userSettings.sDelay6 = fDelay6.Text;
            userSettings.sDelayRepeat = fRepeatDelay.Text;
            userSettings.sHotKey = fToggleKey.Text;
            userSettings.sBehavior = fBehavBox.SelectedIndex.ToString();
            userSettings.Save();
        }

        //Form
        private void SetControl(bool b)
        {
            fUse1.Enabled = b;
            fUse2.Enabled = b;
            fUse3.Enabled = b;
            fUse4.Enabled = b;
            fUse5.Enabled = b;
            fUse6.Enabled = b;
            fKey1.Enabled = b;
            fKey2.Enabled = b;
            fKey3.Enabled = b;
            fKey4.Enabled = b;
            fKey5.Enabled = b;
            fKey6.Enabled = b;
            fDelay1.Enabled = b;
            fDelay2.Enabled = b;
            fDelay3.Enabled = b;
            fDelay4.Enabled = b;
            fDelay5.Enabled = b;
            fDelay6.Enabled = b;
            fRepeatDelay.Enabled = b;
            fToggleKey.Enabled = b;
            fBehavBox.Enabled = b;
            if (b) { fButton.Text = "Set"; } else { fButton.Text = "Unset"; }
        }

        //Config
        private void SetConfig(bool b)
        {
            if (b)
            {
                //Stop
                SetActive(b);
                //Control
                SetControl(b);
                //Overlay
                overlay1.OverlayControl(2);
                //UnRegisterHotKey
                UnregisterHotKey(this.Handle, iHotKey2);
                //Flag
                bConfig = false;
            }
            else
            {
                //Save
                SaveProperties();
                //Flag
                bConfig = true;
                //Values
                if (fUse1.Checked && fDelay1.Text.Length > 0 && int.TryParse(fDelay1.Text, out i)) { iDelay1 = i; }
                if (fUse2.Checked && fDelay2.Text.Length > 0 && int.TryParse(fDelay2.Text, out i)) { iDelay2 = i; }
                if (fUse3.Checked && fDelay3.Text.Length > 0 && int.TryParse(fDelay3.Text, out i)) { iDelay3 = i; }
                if (fUse4.Checked && fDelay4.Text.Length > 0 && int.TryParse(fDelay4.Text, out i)) { iDelay4 = i; }
                if (fUse5.Checked && fDelay5.Text.Length > 0 && int.TryParse(fDelay5.Text, out i)) { iDelay5 = i; }
                if (fUse6.Checked && fDelay6.Text.Length > 0 && int.TryParse(fDelay6.Text, out i)) { iDelay6 = i; }
                if (fRepeatDelay.Text.Length > 0 && int.TryParse(fRepeatDelay.Text, out i)) { iRDelay = i; }
                if (fUse1.Checked && fKey1.Text.Length == 1) { cKey1 = fKey1.Text.ToCharArray()[0]; }
                if (fUse2.Checked && fKey2.Text.Length == 1) { cKey2 = fKey2.Text.ToCharArray()[0]; }
                if (fUse3.Checked && fKey3.Text.Length == 1) { cKey3 = fKey3.Text.ToCharArray()[0]; }
                if (fUse4.Checked && fKey4.Text.Length == 1) { cKey4 = fKey4.Text.ToCharArray()[0]; }
                if (fUse5.Checked && fKey5.Text.Length == 1) { cKey5 = fKey5.Text.ToCharArray()[0]; }
                if (fUse6.Checked && fKey6.Text.Length == 1) { cKey6 = fKey6.Text.ToCharArray()[0]; }
                if (fToggleKey.Text.Length == 1) { cTKey = fToggleKey.Text.ToCharArray()[0]; }
                //RegisterHotKey
                RegisterHotKey(this.Handle, iHotKey2, 0, (uint)(Keys)Enum.ToObject(typeof(Keys), cTKey));
                //Timer
                timerRepeat.Interval = iDelay1 + iDelay2 + iDelay3 + iDelay4 + iDelay5 + iDelay6 + iRDelay;
                //Overlay
                overlay1.OverlayControl(0);
                //Control
                SetControl(b);
            }
        }

        //Active
        private void SetActive(bool b)
        {
            if (b)
            {
                //Stop
                timerRepeat.Enabled = false;
                //Form
                fButton.Enabled = true;
                fButton.Text = "Unset";
                overlay1.OverlayControl(0);
                //Flag
                bActive = false;
            }
            else
            {
                //Flag
                bActive = true;
                //Get Active Process Handle
                iHandle = GetHandle();
                //Form
                fButton.Enabled = false;
                fButton.Text = "RUNNING";
                overlay1.OverlayControl(1);
                //Start
                timerRepeat.Enabled = true;
            }
        }

        //Timer
        private void SetTimer()
        {
            timerRepeat = new System.Timers.Timer();
            switch (fBehavBox.SelectedIndex)
            {
                case 0: timerRepeat.Elapsed += new ElapsedEventHandler(AutoKey1); break;
                case 1: timerRepeat.Elapsed += new ElapsedEventHandler(AutoKey2); break;
                case 2: timerRepeat.Elapsed += new ElapsedEventHandler(AutoKey3); break;
            }
            timerRepeat.AutoReset = true;
        }

        //Handle
        private IntPtr GetHandle()
        {
            if (FindWindowEx(GetForegroundWindow(), IntPtr.Zero, null, "")  != IntPtr.Zero)
            {
                return FindWindowEx(GetForegroundWindow(), IntPtr.Zero, null, "");
            }
            else
            {
                return GetForegroundWindow();
            }
        }

        //Job
        private async void AutoKey1(object sender, ElapsedEventArgs e)
        {
            if (fUse1.Checked) { PostChar(iHandle, cKey1, false); await Task.Delay(iDelay1); }
            if (fUse2.Checked) { PostChar(iHandle, cKey2, false); await Task.Delay(iDelay2); }
            if (fUse3.Checked) { PostChar(iHandle, cKey3, false); await Task.Delay(iDelay3); }
            if (fUse4.Checked) { PostChar(iHandle, cKey4, false); await Task.Delay(iDelay4); }
            if (fUse5.Checked) { PostChar(iHandle, cKey5, false); await Task.Delay(iDelay5); }
            if (fUse6.Checked) { PostChar(iHandle, cKey6, false); await Task.Delay(iDelay6); }
        }
        private async void AutoKey2(object sender, ElapsedEventArgs e)
        {
            if (fUse1.Checked) { PostChar(iHandle, cKey1, true); await Task.Delay(iDelay1); }
            if (fUse2.Checked) { PostChar(iHandle, cKey2, true); await Task.Delay(iDelay2); }
            if (fUse3.Checked) { PostChar(iHandle, cKey3, true); await Task.Delay(iDelay3); }
            if (fUse4.Checked) { PostChar(iHandle, cKey4, true); await Task.Delay(iDelay4); }
            if (fUse5.Checked) { PostChar(iHandle, cKey5, true); await Task.Delay(iDelay5); }
            if (fUse6.Checked) { PostChar(iHandle, cKey6, true); await Task.Delay(iDelay6); }
        }
        private async void AutoKey3(object sender, ElapsedEventArgs e)
        {
            if (fUse1.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey1.Text); await Task.Delay(iDelay1); }
            if (fUse2.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey2.Text); await Task.Delay(iDelay2); }
            if (fUse3.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey3.Text); await Task.Delay(iDelay3); }
            if (fUse4.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey4.Text); await Task.Delay(iDelay4); }
            if (fUse5.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey5.Text); await Task.Delay(iDelay5); }
            if (fUse6.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey6.Text); await Task.Delay(iDelay6); }
        }

        //PostMessage
        static void PostChar(IntPtr i, char k, bool b)
        {
            if (IsWindow(i)) { PostMessage(i, 0x0100, k, IntPtr.Zero); if (b) { PostMessage(i, 0x0101, k, IntPtr.Zero); } }
        }

        //Hotkey
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == hHotKey)
            {
                if (m.WParam.ToInt32() == iHotKey1) { SetConfig(bConfig); }
                else if (m.WParam.ToInt32() == iHotKey2) { SetActive(bActive); }
            }
            base.WndProc(ref m);
        }
    }
}
