using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace AutoKey_Windows
{
    public partial class fProgram : Form
    {
        [DllImport("user32")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vKey);

        [DllImport("user32")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32")]
        static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport("user32")]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32")]
        static extern void PostMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        private static bool flagActive = false, flagRun = false;
        private static int intervalDelay1, intervalDelay2, intervalDelay3, intervalDelay4, intervalDelay5, intervalDelay6, repeatDelay;
        private static IntPtr iHandle;
        private static System.Timers.Timer timerRepeat;
        private UserSettings userSettings;
        private Overlay1 overlay1;

        public fProgram()
        {
            InitializeComponent();
        }

        private void fProgram_Load(object sender, EventArgs e)
        {
            //Load Properties
            CheckProperties();
            //Timer
            SetTimer();
            //RegisterHotKey(F1)
            RegisterHotKey(this.Handle, 31196, 0, 0x70);
            //Overlay
            overlay1 = new Overlay1();
            overlay1.Owner = this;
        }

        private void fProgram_Close(object sender, EventArgs e)
        {
            //UnRegisterHotKey(F1)
            UnregisterHotKey(this.Handle, 31196);
        }

        //Form Input
        private void fKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == Convert.ToChar(Keys.Back) || Regex.IsMatch(e.KeyChar.ToString(), @"^[0-9a-zA-Z]")))
            {
                e.Handled = true;
            }
        }

        private void fDigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == Convert.ToChar(Keys.Back) || char.IsDigit(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void fButton_Click(object sender, EventArgs e)
        {
            if (!flagActive)
            {
                //Save
                SaveProperties();
                //RegisterHotKey(Userset)
                RegisterHotKey(this.Handle, 31197, 0, (int)(Keys)Enum.ToObject(typeof(Keys), fToggleKey.Text[0]));
                //Flag
                flagActive = true;
                //Values
                if (fUse1.Checked) { int.TryParse(fDelay1.Text, out intervalDelay1); }
                if (fUse2.Checked) { int.TryParse(fDelay2.Text, out intervalDelay2); }
                if (fUse3.Checked) { int.TryParse(fDelay3.Text, out intervalDelay3); }
                if (fUse4.Checked) { int.TryParse(fDelay4.Text, out intervalDelay4); }
                if (fUse5.Checked) { int.TryParse(fDelay5.Text, out intervalDelay5); }
                if (fUse6.Checked) { int.TryParse(fDelay6.Text, out intervalDelay6); }
                int.TryParse(fRepeatDelay.Text, out repeatDelay);
                //Timer Interval
                timerRepeat.Interval = (intervalDelay1 + intervalDelay2 + intervalDelay3 + intervalDelay4 + intervalDelay5 + intervalDelay6 + repeatDelay);
                //Control
                SetControl(false);
                //Overlay
                overlay1.OverlayControl(0);
            }
            else
            {
                //UnRegisterHotKey(Userset)
                UnregisterHotKey(this.Handle, 31197);
                //Flag
                flagActive = false;
                //Values
                intervalDelay1 = default;
                intervalDelay2 = default;
                intervalDelay3 = default;
                intervalDelay4 = default;
                intervalDelay5 = default;
                intervalDelay6 = default;
                repeatDelay = default;
                //Control
                SetControl(true);
                //Overlay
                overlay1.OverlayControl(2);
            }
        }

        //Check Properties
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
            if (CheckDelay(userSettings.sDelay1)) { fDelay1.Text = userSettings.sDelay1; } else { fDelay1.Text = "50"; }
            if (CheckDelay(userSettings.sDelay2)) { fDelay2.Text = userSettings.sDelay2; } else { fDelay2.Text = "50"; }
            if (CheckDelay(userSettings.sDelay3)) { fDelay3.Text = userSettings.sDelay3; } else { fDelay3.Text = "50"; }
            if (CheckDelay(userSettings.sDelay4)) { fDelay4.Text = userSettings.sDelay4; } else { fDelay4.Text = "50"; }
            if (CheckDelay(userSettings.sDelay5)) { fDelay5.Text = userSettings.sDelay5; } else { fDelay5.Text = "50"; }
            if (CheckDelay(userSettings.sDelay6)) { fDelay6.Text = userSettings.sDelay6; } else { fDelay6.Text = "50"; }
            if (CheckRepeat(userSettings.sDelayRepeat)) { fRepeatDelay.Text = userSettings.sDelayRepeat; } else { fRepeatDelay.Text = "1000"; }
            if (CheckKey(userSettings.sHotKey)) { fToggleKey.Text = userSettings.sHotKey; } else { fToggleKey.Text = "E"; }
            if (CheckBehavior(userSettings.sBehavior)) { fBehavBox.SelectedIndex = int.Parse(userSettings.sBehavior); } else { fBehavBox.SelectedIndex = 0; }
        }

        private static bool CheckKey(string s)
        {
            if (Regex.IsMatch(s, @"^[0-9a-zA-Z]") && s.Length == 1)
            {
                return true;
            }
            return false;
        }

        private static bool CheckDelay(string s)
        {
            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                {
                    return false;
                }
            }
            if (s.Length > 4)
            {
                return false;
            }
            return true;
        }

        private static bool CheckRepeat(string s)
        {
            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                {
                    return false;
                }
            }
            if (s.Length > 5)
            {
                return false;
            }
            return true;
        }

        private static bool CheckBehavior(string s)
        {
            if (Regex.IsMatch(s, @"^[0-2]") && s.Length == 1)
            {
                return true;
            }
            return false;
        }

        //Save Properties
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

        //Form Control
        private void SetControl(bool b)
        {
            if (b)
            {
                fUse1.Enabled = true;
                fUse2.Enabled = true;
                fUse3.Enabled = true;
                fUse4.Enabled = true;
                fUse5.Enabled = true;
                fUse6.Enabled = true;
                fKey1.Enabled = true;
                fKey2.Enabled = true;
                fKey3.Enabled = true;
                fKey4.Enabled = true;
                fKey5.Enabled = true;
                fKey6.Enabled = true;
                fDelay1.Enabled = true;
                fDelay2.Enabled = true;
                fDelay3.Enabled = true;
                fDelay4.Enabled = true;
                fDelay5.Enabled = true;
                fDelay6.Enabled = true;
                fRepeatDelay.Enabled = true;
                fToggleKey.Enabled = true;
                fBehavBox.Enabled = true;
                fButton.Text = "Set";
            }
            else
            {
                fUse1.Enabled = false;
                fUse2.Enabled = false;
                fUse3.Enabled = false;
                fUse4.Enabled = false;
                fUse5.Enabled = false;
                fUse6.Enabled = false;
                fKey1.Enabled = false;
                fKey2.Enabled = false;
                fKey3.Enabled = false;
                fKey4.Enabled = false;
                fKey5.Enabled = false;
                fKey6.Enabled = false;
                fDelay1.Enabled = false;
                fDelay2.Enabled = false;
                fDelay3.Enabled = false;
                fDelay4.Enabled = false;
                fDelay5.Enabled = false;
                fDelay6.Enabled = false;
                fRepeatDelay.Enabled = false;
                fToggleKey.Enabled = false;
                fBehavBox.Enabled = false;
                fButton.Text = "Unset";
            }
        }

        //Get Process Handle
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

        //Job
        private void AutoKey1(object sender, ElapsedEventArgs e)
        {
            if (fUse1.Checked) { PostChar1(iHandle, fKey1.Text.ToCharArray()[0]); Task.Delay(intervalDelay1).Wait(); }
            if (fUse2.Checked) { PostChar1(iHandle, fKey2.Text.ToCharArray()[0]); Task.Delay(intervalDelay2).Wait(); }
            if (fUse3.Checked) { PostChar1(iHandle, fKey3.Text.ToCharArray()[0]); Task.Delay(intervalDelay3).Wait(); }
            if (fUse4.Checked) { PostChar1(iHandle, fKey4.Text.ToCharArray()[0]); Task.Delay(intervalDelay4).Wait(); }
            if (fUse5.Checked) { PostChar1(iHandle, fKey5.Text.ToCharArray()[0]); Task.Delay(intervalDelay5).Wait(); }
            if (fUse6.Checked) { PostChar1(iHandle, fKey6.Text.ToCharArray()[0]); Task.Delay(intervalDelay6).Wait(); }
        }
        private void AutoKey2(object sender, ElapsedEventArgs e)
        {
            if (fUse1.Checked) { PostChar2(iHandle, fKey1.Text.ToCharArray()[0]); Task.Delay(intervalDelay1).Wait(); }
            if (fUse2.Checked) { PostChar2(iHandle, fKey2.Text.ToCharArray()[0]); Task.Delay(intervalDelay2).Wait(); }
            if (fUse3.Checked) { PostChar2(iHandle, fKey3.Text.ToCharArray()[0]); Task.Delay(intervalDelay3).Wait(); }
            if (fUse4.Checked) { PostChar2(iHandle, fKey4.Text.ToCharArray()[0]); Task.Delay(intervalDelay4).Wait(); }
            if (fUse5.Checked) { PostChar2(iHandle, fKey5.Text.ToCharArray()[0]); Task.Delay(intervalDelay5).Wait(); }
            if (fUse6.Checked) { PostChar2(iHandle, fKey6.Text.ToCharArray()[0]); Task.Delay(intervalDelay6).Wait(); }
        }
        private void AutoKey3(object sender, ElapsedEventArgs e)
        {
            if (fUse1.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey1.Text); Task.Delay(intervalDelay1).Wait(); }
            if (fUse2.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey2.Text); Task.Delay(intervalDelay2).Wait(); }
            if (fUse3.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey3.Text); Task.Delay(intervalDelay3).Wait(); }
            if (fUse4.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey4.Text); Task.Delay(intervalDelay4).Wait(); }
            if (fUse5.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey5.Text); Task.Delay(intervalDelay5).Wait(); }
            if (fUse6.Checked && iHandle == GetForegroundWindow()) { SendKeys.SendWait(fKey6.Text); Task.Delay(intervalDelay6).Wait(); }
        }

        //KeyEvent
        static void PostChar1(IntPtr id, char key)
        {
            //WM_KEYDOWN
            if (IsWindow(id)) { PostMessage(id, 0x0100, key, IntPtr.Zero); }
        }
        static void PostChar2(IntPtr id, char key)
        {
            //WM_KEYDOWN + WM_KEYUP
            if (IsWindow(id)) { PostMessage(id, 0x0100, key, IntPtr.Zero); PostMessage(id, 0x0101, key, IntPtr.Zero); }
        }

        //Active
        private void SetActive(bool b)
        {
            if (b)
            {
                //Flag
                flagRun = true;
                //Get Active Process Handle
                iHandle = GetHandle();
                //Form
                fButton.Enabled = false;
                fButton.Text = "RUNNING";
                overlay1.OverlayControl(1);
                //Repeat
                timerRepeat.Enabled = true;
            }
            else
            {
                //Stop
                timerRepeat.Enabled = false;
                //Form
                fButton.Enabled = true;
                fButton.Text = "Unset";
                overlay1.OverlayControl(0);
                //Flag
                flagRun = false;
            }
        }

        //Hotkey
        protected override void WndProc(ref Message m)
        {
            if (m.WParam.ToInt32() == 31196 && !flagRun)
            {
                fButton_Click(null, null);
            }
            if (m.WParam.ToInt32() == 31196 && flagRun)
            {
                SetActive(false);
                fButton_Click(null, null);
            }
            else if (m.WParam.ToInt32() == 31197 && !flagRun)
            {
                SetActive(true);
            }
            else if (m.WParam.ToInt32() == 31197 && flagRun)
            {
                SetActive(false);
            }
            base.WndProc(ref m);
        }
    }
}
