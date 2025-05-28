using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace AutoKey_Windows
{
    public partial class fProgram : Form
    {
        private static bool aFlag = false, rFlag = false;
        private static int aDelay1, aDelay2, aDelay3, aDelay4, aDelay5, aDelay6, rDelay;
        private static System.Timers.Timer rTimer, pTimer;
        private static Int32 processHandle;
        private UserSettings usersettings;

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
        }

        [DllImport("user32")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vKey);

        [DllImport("user32")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32")]
        public static extern IntPtr GetForegroundWindow();

        //Form Input
        private void fKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == Convert.ToChar(Keys.Back) || Regex.IsMatch(e.KeyChar.ToString(), @"^[0-9a-z]")))
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

        private void fToggle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar == Convert.ToChar(Keys.Back) || Regex.IsMatch(e.KeyChar.ToString(), @"^[A-Z]")))
            {
                e.Handled = true;
            }
        }

        private void fButton_Click(object sender, EventArgs e)
        {
            if (!aFlag)
            {
                //Save
                SaveProperties();
                //RegisterHotKey
                Enum.TryParse<Keys>(fToggleKey.Text, true, out Keys keyCode);
                RegisterHotKey(this.Handle, 31197, 0, (int)keyCode);
                //Flag
                aFlag = true;
                //Values
                if (fUse1.Checked) { int.TryParse(fDelay1.Text, out aDelay1); }
                if (fUse2.Checked) { int.TryParse(fDelay2.Text, out aDelay2); }
                if (fUse3.Checked) { int.TryParse(fDelay3.Text, out aDelay3); }
                if (fUse4.Checked) { int.TryParse(fDelay4.Text, out aDelay4); }
                if (fUse5.Checked) { int.TryParse(fDelay5.Text, out aDelay5); }
                if (fUse6.Checked) { int.TryParse(fDelay6.Text, out aDelay6); }
                int.TryParse(fRepeatDelay.Text, out rDelay);
                //Timer Interval
                rTimer.Interval = (aDelay1 + aDelay2 + aDelay3 + aDelay4 + aDelay5 + aDelay6 + rDelay);
                //Control
                SetControl(false);
            }
            else
            {
                //UnRegisterHotKey
                UnregisterHotKey(this.Handle, 31197);
                //Flag
                aFlag = false;
                //Values
                aDelay1 = default;
                aDelay2 = default;
                aDelay3 = default;
                aDelay4 = default;
                aDelay5 = default;
                aDelay6 = default;
                rDelay = default;
                //Control
                SetControl(true);
            }
        }

        //Check Properties
        private void CheckProperties()
        {
            usersettings = UserSettings.Load();
            fUse1.Checked = usersettings.Use1;
            fUse2.Checked = usersettings.Use2;
            fUse3.Checked = usersettings.Use3;
            fUse4.Checked = usersettings.Use4;
            fUse5.Checked = usersettings.Use5;
            fUse6.Checked = usersettings.Use6;
            if (CheckKey(usersettings.Input1)) { fKey1.Text = usersettings.Input1; } else { fKey1.Text = "1"; }
            if (CheckKey(usersettings.Input2)) { fKey2.Text = usersettings.Input2; } else { fKey2.Text = "2"; }
            if (CheckKey(usersettings.Input3)) { fKey3.Text = usersettings.Input3; } else { fKey3.Text = "3"; }
            if (CheckKey(usersettings.Input4)) { fKey4.Text = usersettings.Input4; } else { fKey4.Text = "4"; }
            if (CheckKey(usersettings.Input5)) { fKey5.Text = usersettings.Input5; } else { fKey5.Text = "5"; }
            if (CheckKey(usersettings.Input6)) { fKey6.Text = usersettings.Input6; } else { fKey6.Text = "6"; }
            if (CheckDelay(usersettings.Delay1)) { fDelay1.Text = usersettings.Delay1; } else { fDelay1.Text = "50"; }
            if (CheckDelay(usersettings.Delay2)) { fDelay2.Text = usersettings.Delay2; } else { fDelay2.Text = "50"; }
            if (CheckDelay(usersettings.Delay3)) { fDelay3.Text = usersettings.Delay3; } else { fDelay3.Text = "50"; }
            if (CheckDelay(usersettings.Delay4)) { fDelay4.Text = usersettings.Delay4; } else { fDelay4.Text = "50"; }
            if (CheckDelay(usersettings.Delay5)) { fDelay5.Text = usersettings.Delay5; } else { fDelay5.Text = "50"; }
            if (CheckDelay(usersettings.Delay6)) { fDelay6.Text = usersettings.Delay6; } else { fDelay6.Text = "50"; }
            if (CheckRepeat(usersettings.RepeatDelay)) { fRepeatDelay.Text = usersettings.RepeatDelay; } else { fRepeatDelay.Text = "1000"; }
            if (CheckToggle(usersettings.HotKey)) { fToggleKey.Text = usersettings.HotKey; } else { fToggleKey.Text = "E"; }
        }

        private static bool CheckKey(string s)
        {
            if (Regex.IsMatch(s, @"^[0-9a-z]") && s.Length == 1)
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

        private static bool CheckToggle(string s)
        {
            if (Regex.IsMatch(s, @"^[A-Z]") && s.Length == 1)
            {
                return true;
            }
            return false;

        }

        //Save Properties
        private void SaveProperties()
        {
            usersettings.Use1 = fUse1.Checked;
            usersettings.Use2 = fUse2.Checked;
            usersettings.Use3 = fUse3.Checked;
            usersettings.Use4 = fUse4.Checked;
            usersettings.Use5 = fUse5.Checked;
            usersettings.Use6 = fUse6.Checked;
            usersettings.Input1 = fKey1.Text;
            usersettings.Input2 = fKey2.Text;
            usersettings.Input3 = fKey3.Text;
            usersettings.Input4 = fKey4.Text;
            usersettings.Input5 = fKey5.Text;
            usersettings.Input6 = fKey6.Text;
            usersettings.Delay1 = fDelay1.Text;
            usersettings.Delay2 = fDelay2.Text;
            usersettings.Delay3 = fDelay3.Text;
            usersettings.Delay4 = fDelay4.Text;
            usersettings.Delay5 = fDelay5.Text;
            usersettings.Delay6 = fDelay6.Text;
            usersettings.RepeatDelay = fRepeatDelay.Text;
            usersettings.HotKey = fToggleKey.Text;
            usersettings.Save();
        }

        //Timer
        private void SetTimer()
        {
            rTimer = new System.Timers.Timer();
            rTimer.Elapsed += new ElapsedEventHandler(AutoKey);
            rTimer.AutoReset = true;
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
                fButton.Text = "Unset";
            }
        }

        //Get Active Process Handle
        private Int32 GetHandle()
        {
            IntPtr i = GetForegroundWindow();
            return i.ToInt32();
        }

        //Job Control
        private void SetActive(bool b)
        {
            if (b)
            {
                //Flag
                rFlag = true;
                //Get Active Process Handle
                processHandle = GetHandle();
                //Form
                fButton.Enabled = false;
                fButton.Text = "RUNNING";
                //Instant
                AutoKey(null, null);
                //Repeat
                rTimer.Enabled = true;
            }
            else
            {
                //Stop
                rTimer.Enabled = false;
                //Form
                fButton.Enabled = true;
                fButton.Text = "Unset";
                //Flag
                rFlag = false;
            }
        }

        //Job
        private void AutoKey(object sender, ElapsedEventArgs e)
        {
            if (GetHandle() == processHandle)
            {
                //Sendkey
                if (fUse1.Checked) { SendKeys.SendWait(fKey1.Text); Task.Delay(aDelay1).Wait(); }
                if (fUse2.Checked) { SendKeys.SendWait(fKey2.Text); Task.Delay(aDelay2).Wait(); }
                if (fUse3.Checked) { SendKeys.SendWait(fKey3.Text); Task.Delay(aDelay3).Wait(); }
                if (fUse4.Checked) { SendKeys.SendWait(fKey4.Text); Task.Delay(aDelay4).Wait(); }
                if (fUse5.Checked) { SendKeys.SendWait(fKey5.Text); Task.Delay(aDelay5).Wait(); }
                if (fUse6.Checked) { SendKeys.SendWait(fKey6.Text); Task.Delay(aDelay6).Wait(); }
            }
            else
            {
                SetActive(false);
            }
        }

        //Hotkey
        protected override void WndProc(ref Message m)
        {
            if (m.WParam.ToInt32() == 31197 && !rFlag)
            {
                SetActive(true);
            }
            else if (m.WParam.ToInt32() == 31197 && rFlag)
            {
                SetActive(false);
            }
            base.WndProc(ref m);
        }
    }
}
