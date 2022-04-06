using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;

namespace WAVSlowVerb
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Functions functions = new Functions();
        private void button1_Click(object sender, EventArgs e)
        {
            functions.Open();
            textBox1.Text = functions.path;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @functions.path;
            player.Play();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            functions.Save();
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @functions.new_path;
            player.Play();
            label1.Text = "Saved at " + functions.new_path;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            functions.Slow();
            functions.addReverb();
            label1.Text = "Slowed Successful";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label3.Text = (trackBar1.Value * 10) + "%";
            functions.slowRate = trackBar1.Value;
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            label7.Text = trackBar4.Value + "%";
            functions.mix = (trackBar4.Value / 100);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label8.Text = Convert.ToString(((float)trackBar2.Value) / 100);
            functions.decay = trackBar2.Value / 100;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label9.Text = trackBar3.Value + "ms";
            functions.delay = trackBar3.Value;
        }
    }
}
