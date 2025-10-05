using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace AutoKey_Windows
{
    public partial class fProgram : Form
    {
        [DllImport("user32")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vKey);
        [DllImport("user32")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32")]
        private static extern short GetAsyncKeyState(int vKey);
        [DllImport("user32")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        private const int INPUT_KEYBOARD = 1, KEYEVENTF_EXTENDEDKEY = 0x0001, KEYEVENTF_KEYUP = 0x0002, KEYEVENTF_UNICODE = 0x0004, KEYEVENTF_SCANCODE = 0x0008;
        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk, wScan; public uint dwFlags, time; public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx, dy; public uint mouseData, dwFlags, time; public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg; public ushort wParamL, wParamH;
        }
        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(8)]
            public KEYBDINPUT ki;
            [FieldOffset(8)]
            public MOUSEINPUT mi;
            [FieldOffset(8)]
            public HARDWAREINPUT hi;
        }

        private List<Control> controls;
        private UserSettings userSettings;
        private CancellationTokenSource cToken;
        private bool bConfig = false, isRunning = false;
        private const short WM_HOTKEY = 0x0312, HOTKEY_ID_CONFIG = 31196, HOTKEY_ID_ACTIVE = 31197;
        private short i, iKey1, iKey2, iKey3, iKey4, iKey5, iKey6;
        private ushort cKey1, cKey2, cKey3, cKey4, cKey5, cKey6, cHKey;

        //Init
        public fProgram()
        {
            InitializeComponent();
            this.Load += (s, e) => RegisterHotKey(this.Handle, HOTKEY_ID_CONFIG, 0, (ushort)Keys.F1); LoadUserSetting();
            this.FormClosing += (s, e) => UnregisterHotKey(this.Handle, HOTKEY_ID_CONFIG);
            controls = new List<Control> { fUse1, fUse2, fUse3, fUse4, fUse5, fUse6, fKey1, fKey2, fKey3, fKey4, fKey5, fKey6, fDelay1, fDelay2, fDelay3, fDelay4, fDelay5, fDelay6, fToggleKey };
        }

        //Form Input
        private void fKey_KeyPress(object sender, KeyPressEventArgs e) { e.Handled = !(e.KeyChar == Convert.ToChar(Keys.Back) || Regex.IsMatch(e.KeyChar.ToString(), @"^[0-9A-Z]")); }
        private void fDigit_KeyPress(object sender, KeyPressEventArgs e) { e.Handled = !(e.KeyChar == Convert.ToChar(Keys.Back) || char.IsDigit(e.KeyChar)); }
        private void fButton_Click(object sender, EventArgs e) { SetConfig(bConfig); }

        //UserSettings
        private static bool CheckKey(string s) { return Regex.IsMatch(s, @"^[0-9A-Z]") && s.Length == 1; }
        private static bool CheckDelay(string s, short l) { return s.All(Char.IsDigit) && s.Length <= l; }
        private void LoadUserSetting()
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
            if (CheckKey(userSettings.sHotKey)) { fToggleKey.Text = userSettings.sHotKey; } else { fToggleKey.Text = "E"; }
        }
        private void SaveUserSetting()
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
            userSettings.sHotKey = fToggleKey.Text;
            userSettings.Save();
        }

        //Form
        private void GetUserSttings()
        {
            if (fUse1.Checked && fDelay1.Text.Length > 0 && short.TryParse(fDelay1.Text, out i)) { iKey1 = i; }
            if (fUse2.Checked && fDelay2.Text.Length > 0 && short.TryParse(fDelay2.Text, out i)) { iKey2 = i; }
            if (fUse3.Checked && fDelay3.Text.Length > 0 && short.TryParse(fDelay3.Text, out i)) { iKey3 = i; }
            if (fUse4.Checked && fDelay4.Text.Length > 0 && short.TryParse(fDelay4.Text, out i)) { iKey4 = i; }
            if (fUse5.Checked && fDelay5.Text.Length > 0 && short.TryParse(fDelay5.Text, out i)) { iKey5 = i; }
            if (fUse6.Checked && fDelay6.Text.Length > 0 && short.TryParse(fDelay6.Text, out i)) { iKey6 = i; }
            if (fUse1.Checked && fKey1.Text.Length == 1) { cKey1 = fKey1.Text.ToUpper().ToCharArray()[0]; }
            if (fUse2.Checked && fKey2.Text.Length == 1) { cKey2 = fKey2.Text.ToUpper().ToCharArray()[0]; }
            if (fUse3.Checked && fKey3.Text.Length == 1) { cKey3 = fKey3.Text.ToUpper().ToCharArray()[0]; }
            if (fUse4.Checked && fKey4.Text.Length == 1) { cKey4 = fKey4.Text.ToUpper().ToCharArray()[0]; }
            if (fUse5.Checked && fKey5.Text.Length == 1) { cKey5 = fKey5.Text.ToUpper().ToCharArray()[0]; }
            if (fUse6.Checked && fKey6.Text.Length == 1) { cKey6 = fKey6.Text.ToUpper().ToCharArray()[0]; }
            if (fToggleKey.Text.Length == 1) { cHKey = fToggleKey.Text.ToUpper().ToCharArray()[0]; }
        }
        private void SetControls(bool b)
        {
            foreach (Control ctl in controls) { ctl.Enabled = b; }
            if (b) { fButton.Text = "Set (F1)"; } else { fButton.Text = "Unset (F1)"; }
        }

        //Config
        private void SetConfig(bool b)
        {
            if (b)
            {
                SetControls(b);
                UnregisterHotKey(this.Handle, HOTKEY_ID_ACTIVE);
                bConfig = false;
            }
            else
            {
                bConfig = true;
                SaveUserSetting();
                GetUserSttings();
                RegisterHotKey(this.Handle, HOTKEY_ID_ACTIVE, 0, (ushort)(Keys)Enum.ToObject(typeof(Keys), cHKey));
                SetControls(b);
            }
        }

        //Input
        private void KDown(ushort vk)
        {
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_KEYBOARD;
            input[0].ki.wVk = vk;
            input[0].ki.dwFlags = 0;
            SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
        }
        private void KUp(ushort vk)
        {
            INPUT[] input = new INPUT[1];
            input[0].type = INPUT_KEYBOARD;
            input[0].ki.wVk = vk;
            input[0].ki.dwFlags = KEYEVENTF_KEYUP;
            SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
        }
        private void KTap(ushort vk)
        {
            KDown(vk);
            KUp(vk);
        }

        //Automation
        private async void CheckAutoKey(CancellationToken t)
        {
            while (!t.IsCancellationRequested)
            {
                if ((GetAsyncKeyState(cHKey) & 0x8000) == 0)
                {
                    StopAutoKey(); break;
                }
                await Task.Delay(100, t);
            }
        }
        private async void StartAutoKey()
        {
            if (isRunning) { return; }
            cToken?.Dispose();
            cToken = new CancellationTokenSource();
            var t = cToken.Token;
            isRunning = true;
            await Task.Run(() => CheckAutoKey(t));
            try
            {
                while (!t.IsCancellationRequested)
                {
                    if (fUse1.Checked) { KTap((ushort)cKey1); await Task.Delay(iKey1, t); }
                    if (fUse2.Checked) { KTap((ushort)cKey2); await Task.Delay(iKey2, t); }
                    if (fUse3.Checked) { KTap((ushort)cKey3); await Task.Delay(iKey3, t); }
                    if (fUse4.Checked) { KTap((ushort)cKey4); await Task.Delay(iKey4, t); }
                    if (fUse5.Checked) { KTap((ushort)cKey5); await Task.Delay(iKey5, t); }
                    if (fUse6.Checked) { KTap((ushort)cKey6); await Task.Delay(iKey6, t); }
                }
            }
            catch (TaskCanceledException) { }
            finally { isRunning = false; }
        }
        private void StopAutoKey()
        {
            isRunning = false;
            cToken?.Cancel();
        }

        //Hotkey
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == HOTKEY_ID_CONFIG) { SetConfig(bConfig); }
                else if (m.WParam.ToInt32() == HOTKEY_ID_ACTIVE) { StartAutoKey(); }
            }
            base.WndProc(ref m);
        }
    }
}
