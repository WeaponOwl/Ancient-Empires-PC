using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameReLauncher
{
    public partial class Form1 : Form
    {

        int w, h, l;
        bool f, m, a;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IniParser.FileIniDataParser parser = new IniParser.FileIniDataParser();
            IniParser.IniData data = parser.LoadFile("config.ini");

            w = Convert.ToInt32(data["Graphics"]["Width"]);
            h = Convert.ToInt32(data["Graphics"]["Height"]);
            f = Convert.ToBoolean(data["Graphics"]["Fullscreen"]);
            //cheatmode = Convert.ToBoolean(data["GamePlay"]["Cheats"]);
            m = Convert.ToBoolean(data["Music"]["Sounds"]);
            string lang = data["GamePlay"]["Lang"];
            l = 0;
            switch (lang)
            {
                case "Eng": l = 0; comboBox1.SelectedIndex = 0; break;
                case "Rus": l = 1; comboBox1.SelectedIndex = 1; break;
                //default: l = 0; break;
            }
            a = Convert.ToBoolean(data["Launch"]["BootLauncher"]);

            if (!a) StartPlay();

            textBox1.Text = w.ToString();
            textBox2.Text = h.ToString();
            checkBox2.Checked = f;
            checkBox1.Checked = m;
            checkBox3.Checked = a;

            if (l == 0)
            {
                label1.Text = "Width";
                label2.Text = "Height";
                checkBox1.Text = "Music";
                checkBox2.Text = "FullScreen";
                checkBox3.Text = "Show me again";
                button1.Text = "Play";
            }
            if (l == 1)
            {
                label1.Text = "Ширина";
                label2.Text = "Высота";
                checkBox1.Text = "Музыка";
                checkBox2.Text = "Полный экран";
                checkBox3.Text = "Показывать снова";
                button1.Text = "Играть";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string lang = comboBox1.Items[comboBox1.SelectedIndex].ToString();
            l = 0;
            switch (lang)
            {
                case "English": l = 0; break;
                case "Русский": l = 1; break;
                //default: l = 0; break;
            }
            if (l == 0)
            {
                label1.Text = "Width";
                label2.Text = "Height";
                checkBox1.Text = "Music";
                checkBox2.Text = "FullScreen";
                checkBox3.Text = "Show me again";
                button1.Text = "Play";
            }
            if (l == 1)
            {
                label1.Text = "Ширина";
                label2.Text = "Высота";
                checkBox1.Text = "Музыка";
                checkBox2.Text = "Полный экран";
                checkBox3.Text = "Показывать снова";
                button1.Text = "Играть";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IniParser.FileIniDataParser parser = new IniParser.FileIniDataParser();
            IniParser.IniData data = parser.LoadFile("config.ini");

            data["Graphics"]["Width"]=w.ToString();
            data["Graphics"]["Height"]=h.ToString();
            data["Graphics"]["Fullscreen"]=f.ToString();
            data["Music"]["Sounds"]=m.ToString();
            switch (l)
            {
                case 0: data["GamePlay"]["Lang"]="Eng"; break;
                case 1: data["GamePlay"]["Lang"]="Rus"; break;
                default: data["GamePlay"]["Lang"] = "Eng"; break;
            }
            data["Launch"]["BootLauncher"] = a.ToString();

            parser.SaveFile("config.ini", data);

            StartPlay();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                w = Convert.ToInt32(textBox1.Text);
            }
            catch { ;}
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
               h = Convert.ToInt32(textBox2.Text);
            }
            catch { ;}
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            f = checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            m = checkBox1.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            a = checkBox3.Checked;
        }

        void StartPlay()
        {
            try
            {
                System.Diagnostics.Process Game = new System.Diagnostics.Process();
                Game.StartInfo.FileName = "Ancient empires.exe";
                Game.Start();
            }
            catch { ;}
            Application.Exit();
        }
    }
}
