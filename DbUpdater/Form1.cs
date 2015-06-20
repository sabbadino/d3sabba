using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DbUpdater
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				var dbUpdate = new DbUpdateByVersion(radioButton_F.Checked ? UpdateStrategy.Full : UpdateStrategy.WaterMark,comboBox_Env.Text, textBox_conffile.Text, textBox_folder.Text);
				dbUpdate.Doupdate();
				MessageBox.Show("Done");
			}
			catch (Exception ex) {
				MessageBox.Show(ex.ToString());}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				//var dbUpdate = new DbUpdateByDate(radioButton_F.Checked ? UpdateStrategy.Full : UpdateStrategy.WaterMark, textBox_Env.Text, textBox_conffile.Text, textBox_scripbydate.Text);
				//dbUpdate.Doupdate();
				//MessageBox.Show("Done");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			loadEnvCombo();
		}

		private void loadEnvCombo()
		{
			if (!File.Exists(textBox_conffile.Text)) return;
			comboBox_Env.Items.Clear();
			var conf = new XmlDocument();
			conf.Load(textBox_conffile.Text);
			foreach (XmlNode env in conf.SelectNodes("/config/environments/environment"))
			{
				comboBox_Env.Items.Add(env.Attributes["name"].Value);
			}
			if (comboBox_Env.Items.Count > 0)
				comboBox_Env.SelectedIndex = 0;
		}

		
	
			

		private void textBox_conffile_Leave(object sender, EventArgs e)
		{
			loadEnvCombo();
	
		}
	}
}
