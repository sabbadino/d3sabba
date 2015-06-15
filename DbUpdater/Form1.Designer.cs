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
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(14, 156);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Go";
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
			this.textBox_folder.Text = "C:\\ProgrammingStuff\\MyCode\\d3sabba\\DbUpdater\\scripts";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(11, 102);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Env";
			// 
			// textBox_Env
			// 
			this.textBox_Env.Location = new System.Drawing.Point(14, 118);
			this.textBox_Env.Name = "textBox_Env";
			this.textBox_Env.Size = new System.Drawing.Size(472, 20);
			this.textBox_Env.TabIndex = 5;
			this.textBox_Env.Text = "PRJ5-UNITY";
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
			this.textBox_conffile.Text = "C:\\ProgrammingStuff\\MyCode\\d3sabba\\DbUpdater\\scripts\\DBScriptRunnerFolder.cfg";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(516, 261);
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
	}
}

