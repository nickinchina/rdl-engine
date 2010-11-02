using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace rdlTestApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

        }

        private void Form2_ResizeEnd(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Bitmap bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            Rdl.Engine.Chart.RayTrace.Engine rtEngine = new Rdl.Engine.Chart.RayTrace.Engine();
            rtEngine.GetScene().InitScene();
            rtEngine.SetTarget(bm);
            rtEngine.InitRender();
            rtEngine.Render();

            pictureBox1.Image = bm;
            this.Cursor = Cursors.Default;
        }
    }
}