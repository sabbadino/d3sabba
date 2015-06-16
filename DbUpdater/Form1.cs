using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
				var dbUpdate = new DbUpdateByVersion(radioButton_F.Checked ? UpdateStrategy.Full : UpdateStrategy.WaterMark,textBox_Env.Text, textBox_conffile.Text, textBox_folder.Text);
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
				var dbUpdate = new DbUpdateByDate(radioButton_F.Checked ? UpdateStrategy.Full : UpdateStrategy.WaterMark, textBox_Env.Text, textBox_conffile.Text, textBox_scripbydate.Text);
				dbUpdate.Doupdate();
				MessageBox.Show("Done");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
	}
}
