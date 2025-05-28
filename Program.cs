using System;
using System.Windows.Forms;

namespace AutoKey_Windows
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fProgram());
        }
    }
}
