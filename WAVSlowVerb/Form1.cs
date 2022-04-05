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
            label1.Text = "Slowed Successful";
        }
    }
}
