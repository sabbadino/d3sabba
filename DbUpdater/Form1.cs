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
				var dbUpdate = new DbUpdate(textBox_cnstring.Text, textBox_folder.Text);
				dbUpdate.Doupdate();
				MessageBox.Show("Done");
			}
			catch (Exception ex) {
				MessageBox.Show(ex.ToString());}
		}
	}
}
