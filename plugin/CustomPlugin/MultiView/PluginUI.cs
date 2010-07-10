using System;
using System.Collections;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore;

namespace MultiView
{
	public class PluginUI : UserControl
    {
        private RichTextBox richTextBox;
        private Button button1;
		private PluginMain pluginMain;
        
		public PluginUI(PluginMain pluginMain)
		{
			this.InitializeComponent();
			this.pluginMain = pluginMain;
		}

        /// <summary>
        /// Accessor to the RichTextBox
        /// </summary>
        public RichTextBox Output
        {
            get { return this.richTextBox; }
        }
		
		#region Windows Forms Designer Generated Code

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() 
        {
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.DetectUrls = false;
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(280, 352);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // PluginUI
            // 
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.ResumeLayout(false);

		}

		#endregion

        private void button1_Click(object sender, EventArgs e)
        {
            pluginMain.CloneCurrentDocument();
        }
				
 	}

}
