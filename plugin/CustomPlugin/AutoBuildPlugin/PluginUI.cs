using System;
using System.Collections;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore;
using FlashDevelop.Managers;
using System.Drawing;
using FlashDevelop;
using AutoBuildPlugin.Resources;

namespace AutoBuildPlugin
{
	public class PluginUI : UserControl
    {
        //public Button StartButton;
        //public Button StopButton;
        //private PictureBox icon;
        //private Panel panel1;
        //public Label label1;
		private PluginMain pluginMain;
        //public ToolStrip menuBar;

        public ToolStripButton ToggleBuildButton;
        public ToolStripButton ReloadProjectButton;
        public ToolStripLabel ProjectNameLabel;

		public PluginUI(PluginMain pluginMain)
		{
			this.InitializeComponent();
			this.pluginMain = pluginMain;
		}

		#region Windows Forms Designer Generated Code

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() 
        {
            ToggleBuildButton = new ToolStripButton(Get("127|11|5|4").Img);
            ToggleBuildButton.ToolTipText = "Auto Build";
            ToggleBuildButton.Click += new EventHandler(ToggleBuildButtonClick);

            ProjectNameLabel = new ToolStripLabel();
            ProjectNameLabel.Text = "";

            ReloadProjectButton = new ToolStripButton(Get("66").Img);
            ReloadProjectButton.ToolTipText = "Reload Project";
            ReloadProjectButton.Click += new EventHandler(ReloadProjectButtonClick);

            //menuBar = new ToolStrip();
            //menuBar.Renderer = new DockPanelStripRenderer(false);
            //menuBar.Padding = new Padding(0, 1, 0, 0);
            //menuBar.Stretch = true;

            //this.StartButton = new System.Windows.Forms.Button();
            //this.StopButton = new System.Windows.Forms.Button();
            /*
            this.icon = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.icon)).BeginInit();
             * */
            //this.SuspendLayout();

            //PluginBase.MainForm.ToolStrip.Items.Add(new ToolStripSeparator());

            
            //menuBar.Items.Add(ToggleBuildButton);

            //PluginBase.MainForm.ToolStrip.Items.Add(ToggleBuildButton);

            
            //menuBar.Items.Add(ProjectNameLabel);

            //PluginBase.MainForm.ToolStrip.Items.Add(ProjectNameLabel);

            
            //menuBar.Items.Add(ReloadProjectButton);

            //PluginBase.MainForm.ToolStrip.Items.Add(ReloadProjectButton);

            //menuBar.Items.Add(new ToolStripSeparator());

            // 
            // StartButton
            // 
            /*
            this.StartButton.Location = new System.Drawing.Point(29, 26);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(110, 26);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(75, 23);
            this.StopButton.TabIndex = 2;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            
            
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(286, 324);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            */
            // 
            // PluginUI
            // 

            //if (mainForm == null) mainForm = (MainForm)Globals.MainForm;
            //mainForm.ToolStripPanel.Controls.Add(menuBar);

            /*
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.icon);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.StartButton);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(193, 57);
            ((System.ComponentModel.ISupportInitialize)(this.icon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            */
		}

        public string projectName
        {
            set { ProjectNameLabel.Text = value; }
        }

        public UIState state;

        public void changeState(UIState state)
        {
            switch (state)
            {
                case UIState.Standby:
                    this.ToggleBuildButton.Image = Get("127|11|5|4").Img;
                    this.ToggleBuildButton.Enabled = true;
                    ReloadProjectButton.Enabled = true;
                    break;
                case UIState.Running:
                    this.ToggleBuildButton.Image = Get("127|23|5|4").Img;
                    this.ToggleBuildButton.Enabled = true;
                    ReloadProjectButton.Enabled = false;
                    break;
                case UIState.Disabled:
                    this.ToggleBuildButton.Image = Get("127|11|5|4").Img;
                    this.ToggleBuildButton.Enabled = false;
                    ReloadProjectButton.Enabled = false;
                    break;
                default:

                    break;
            }
            this.state = state;
        }

        private void ReloadProjectButtonClick(object sender, System.EventArgs e)
        {
            pluginMain.ReloadProject();
        }

        private void ToggleBuildButtonClick(object sender, System.EventArgs e)
        {
            switch (state)
            {
                case UIState.Standby:
                    pluginMain.startWatcher();
                    break;
                case UIState.Running:
                    pluginMain.stopWatcher();
                    break;
                case UIState.Disabled:
                    break;
                default:
                    break;
            }
        }

		#endregion

        private MainForm mainForm;

        public FDImage Get(string data)
        {
            if(mainForm == null) mainForm = (MainForm)Globals.MainForm;
            Image image = (mainForm != null) ? mainForm.FindImage(data) : new Bitmap(16, 16);
            return new FDImage(image);
        }

 	}

    public class FDImage
    {
        public readonly Image Img;

        public FDImage(Image img)
        {
            Img = img;
        }

        public Icon Icon { get { return Icon.FromHandle((Img as Bitmap).GetHicon()); } }
    }

    public enum UIState
    {
        Disabled,
        Standby,
        Running
    }

}
