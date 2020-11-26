using NAudio.Wave;
using Radiotor.Radio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Radiotor
{
    public partial class Form1 : Form
    {
        private WaveOutEvent wo;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)       
        {
            this.label1.Text = "Buffering";

            string url = ((ComboBoxItem)this.comboBox1.SelectedItem).Value;

            var mf = new MediaFoundationReader(url);
          
            this.wo.Init(mf);
            this.wo.Play();

            this.button1.Enabled = false;
            this.button2.Enabled = true;

            this.comboBox1.Enabled = false;

            this.label1.Text = this.wo.PlaybackState.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.wo.Stop();

            this.button1.Enabled = true;
            this.button2.Enabled = false;

            this.comboBox1.Enabled = true;

            this.label1.Text = this.wo.PlaybackState.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // tray
            this.notifyIcon1.Icon = Properties.Resources.Icon1;

            wo = new WaveOutEvent();

            wo.PlaybackStopped += Wo_PlaybackStopped;

            this.button1.Enabled = true;
            this.button2.Enabled = false;

            XmlSerializer xml = new XmlSerializer(typeof(List<UrlItem>), new XmlRootAttribute("UrlItems"));
            FileStream xmlStream = new FileStream("stations.xml", FileMode.Open);
            List<UrlItem> result = (List<UrlItem>)xml.Deserialize(xmlStream);

            foreach (UrlItem urlItem in result)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Text = urlItem.Name;
                item.Value = urlItem.Url;
                comboBox1.Items.Add(item);
            }

            comboBox1.SelectedIndex = 0;

            this.label1.Text = this.wo.PlaybackState.ToString();
        }

        private void Wo_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            this.label1.Text = this.wo.PlaybackState.ToString();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipTitle = "Minimize to Tray App";
            notifyIcon1.BalloonTipText = "You have successfully minimized your form.";

            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }
    }
}
