using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Configuration;

namespace Example
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblKeyPressed;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblKeyPressed = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblKeyPressed
            // 
            this.lblKeyPressed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblKeyPressed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeyPressed.Location = new System.Drawing.Point(0, 0);
            this.lblKeyPressed.Name = "lblKeyPressed";
            this.lblKeyPressed.Size = new System.Drawing.Size(298, 64);
            this.lblKeyPressed.TabIndex = 0;
            this.lblKeyPressed.Text = "Press a key...";
            this.lblKeyPressed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(298, 64);
            this.Controls.Add(this.lblKeyPressed);
            this.Enabled = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "Keyboard Listener";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.ResumeLayout(false);

		}
		#endregion

		private static TimeSpan startBoundary;
		private static TimeSpan endBoundary ;
		private static bool IsAudioPlaying = false;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			var form = new Form1();
			form.Visible = false;
			var startBoundaryHour = Convert.ToInt32( ConfigurationManager.AppSettings.Get("startBoundaryHour"));
			var startBoundaryMinutes = Convert.ToInt32(ConfigurationManager.AppSettings.Get("startBoundaryMinutes"));
			var endBoundaryHour = Convert.ToInt32(ConfigurationManager.AppSettings.Get("endBoundaryHour"));
			var endBoundaryMinutes = Convert.ToInt32(ConfigurationManager.AppSettings.Get("endBoundaryMinutes"));
			startBoundary = TimeSpan.FromHours(startBoundaryHour).Add(TimeSpan.FromMinutes(startBoundaryMinutes));
			endBoundary = TimeSpan.FromHours(endBoundaryHour).Add(TimeSpan.FromMinutes(endBoundaryMinutes));
			Application.Run(form);
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			// Watch for keyboard activity
			KeyboardListener.s_KeyEventHandler += new EventHandler(KeyboardListener_s_KeyEventHandler);
		}

		private void KeyboardListener_s_KeyEventHandler(object sender, EventArgs e)
		{
			var nowTime = DateTime.Now.TimeOfDay;
            if (nowTime > startBoundary && DateTime.Now.TimeOfDay < endBoundary)
            {
                foreach (var process in Process.GetProcessesByName("chrome"))
                {
                    process.Kill();
                }
				Task.Run(() => PlayAudio());
			}

        }

		private void PlayAudio()
        {
			if (IsAudioPlaying == false)
			{
				IsAudioPlaying = true;
				System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Recording.wav");
				player.PlaySync();
				IsAudioPlaying = false;
			}
		}

        private void Form1_Shown(object sender, EventArgs e)
        {
			this.Hide();
        }
    }
}
