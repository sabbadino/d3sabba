namespace DbUpdater
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox_folder = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox_Env = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textBox_conffile = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox_scripbydate = new System.Windows.Forms.TextBox();
			this.radioButton_F = new System.Windows.Forms.RadioButton();
			this.radioButton_WM = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 98);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Go Ver";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "scripts folder";
			// 
			// textBox_folder
			// 
			this.textBox_folder.Location = new System.Drawing.Point(14, 72);
			this.textBox_folder.Name = "textBox_folder";
			this.textBox_folder.Size = new System.Drawing.Size(472, 20);
			this.textBox_folder.TabIndex = 3;
			this.textBox_folder.Text = "C:\\Unity\\SOA\\CountriesService\\develop\\sqldeploy";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 192);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Env";
			// 
			// textBox_Env
			// 
			this.textBox_Env.Location = new System.Drawing.Point(15, 208);
			this.textBox_Env.Name = "textBox_Env";
			this.textBox_Env.Size = new System.Drawing.Size(472, 20);
			this.textBox_Env.TabIndex = 5;
			this.textBox_Env.Text = "LOCAL";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(52, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "config file";
			// 
			// textBox_conffile
			// 
			this.textBox_conffile.Location = new System.Drawing.Point(14, 23);
			this.textBox_conffile.Name = "textBox_conffile";
			this.textBox_conffile.Size = new System.Drawing.Size(472, 20);
			this.textBox_conffile.TabIndex = 7;
			this.textBox_conffile.Text = "C:\\ProgrammingStuff\\MyCode\\d3sabba\\DbUpdater\\DBScriptRunnerFolder.cfg";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(15, 164);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 9;
			this.button2.Text = "Go Date";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 124);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "scripts folder";
			// 
			// textBox_scripbydate
			// 
			this.textBox_scripbydate.Location = new System.Drawing.Point(15, 138);
			this.textBox_scripbydate.Name = "textBox_scripbydate";
			this.textBox_scripbydate.Size = new System.Drawing.Size(472, 20);
			this.textBox_scripbydate.TabIndex = 10;
			this.textBox_scripbydate.Text = "C:\\ProgrammingStuff\\MyCode\\d3sabba\\DbUpdater\\scripts_bydate";
			// 
			// radioButton_F
			// 
			this.radioButton_F.AutoSize = true;
			this.radioButton_F.Checked = true;
			this.radioButton_F.Location = new System.Drawing.Point(15, 234);
			this.radioButton_F.Name = "radioButton_F";
			this.radioButton_F.Size = new System.Drawing.Size(109, 17);
			this.radioButton_F.TabIndex = 12;
			this.radioButton_F.TabStop = true;
			this.radioButton_F.Text = "Update Mode Full";
			this.radioButton_F.UseVisualStyleBackColor = true;
			// 
			// radioButton_WM
			// 
			this.radioButton_WM.AutoSize = true;
			this.radioButton_WM.Location = new System.Drawing.Point(159, 234);
			this.radioButton_WM.Name = "radioButton_WM";
			this.radioButton_WM.Size = new System.Drawing.Size(145, 17);
			this.radioButton_WM.TabIndex = 13;
			this.radioButton_WM.Text = "Update mode WaterMark";
			this.radioButton_WM.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(516, 278);
			this.Controls.Add(this.radioButton_WM);
			this.Controls.Add(this.radioButton_F);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox_scripbydate);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBox_conffile);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBox_Env);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox_folder);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox_folder;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox_Env;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBox_conffile;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox_scripbydate;
		private System.Windows.Forms.RadioButton radioButton_F;
		private System.Windows.Forms.RadioButton radioButton_WM;
	}
}

