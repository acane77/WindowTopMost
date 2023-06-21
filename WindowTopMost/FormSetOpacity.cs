using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowTopMost
{
    public partial class FormSetOpacity : Form
    {
        public FormSetOpacity()
        {
            InitializeComponent();
            trackBar1.Value = 100;
        }

        private void txtTransparency_Leave(object sender, EventArgs e)
        {
            string valueStr = txtTransparency.Text.Replace("%", "").Trim();
            try
            {
                trackBar1.Value = int.Parse(valueStr);
            }
            catch {  }
            trackBar1_Scroll(null, null);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            txtTransparency.Text = trackBar1.Value + "%";
        }

        private void FormSetOpacity_Load(object sender, EventArgs e)
        {
            float scale = DpiInfo.GetScale(this.Handle, false);
            float scale_delta = scale - 1.0f;
            scale -= scale_delta;
            Scale(new SizeF(scale, scale));
            trackBar1_Scroll(null, null);
        }

        public bool TransparenctChanged = false;
        public int TransparencyValue
        {
            get => trackBar1.Value; 
        }

        public void SetInitlalValue(byte alpha)
        {
            try
            {
                trackBar1.Value = (int)Math.Ceiling(alpha / 255.0 * 100);
                trackBar1_Scroll(null, null);
            }
            catch { }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            TransparenctChanged = true;
            txtTransparency_Leave(null, null);
            Close();
        }
    }
}
