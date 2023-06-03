using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KG2
{
     
    public partial class FormMain : Form
    {
        private Color brush = Color.DarkOliveGreen;

        public FormMain()
        {
            InitializeComponent();
        }

        private void bPaint_Click(object sender, EventArgs e)
        {
            Scene s = new Scene(pictureBoxView.Height, pictureBoxView.Width);
            s.brush = brush;
            s.addCylinder(Double.Parse(textBoxR.Text), Double.Parse(textBoxH.Text));
            s.addCamera(new Camera(new Point(textBoxCamPos.Text), new Point(textBoxCamDir.Text).minus(new Point(textBoxCamPos.Text)), (double)udAngle.Value));
            s.lightPoint = new Point(textBoxLights.Text);
            s.Render();
            pictureBoxView.Image = s.pic;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            brush = colorDialog.Color;
            bPaint_Click(sender, e);
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            bPaint_Click(sender, e);
        }

        private void pictureBoxView_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void pictureBoxView_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void textBoxR_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
