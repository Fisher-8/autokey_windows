using System.Drawing;
using System.Windows.Forms;

namespace AutoKey_Windows
{
    public partial class Overlay1 : Form
    {
        private Label label1;
        public Overlay1()
        {
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(5, 5);
            this.AllowTransparency = true;
            this.BackColor = System.Drawing.Color.LimeGreen;
            this.TransparencyKey = System.Drawing.Color.LimeGreen;
            //label1
            label1 = new Label();
            label1.AutoSize = false;
            label1.Size = new Size(60, 20);
            label1.Margin = new Padding(0);
            label1.Padding = new Padding(0, 0, 0, 1);
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Text = "ACTIVE";
            label1.Font = new Font("Verdana", 9, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.BackColor = Color.Red;
            this.Controls.Add(label1);
        }
    }
}
