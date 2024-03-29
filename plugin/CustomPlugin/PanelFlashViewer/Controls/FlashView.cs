using System;
using System.Text;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using AxShockwaveFlashObjects;
using PluginCore.Localization;
using PluginCore.Managers;
using System.IO;
using System.Xml;

namespace PanelFlashViewer.Controls
{
    public class FlashView : UserControl
    {
        private String moviePath;
        private AxShockwaveFlash flashMovie;

        public FlashView(String file)
        {
            this.moviePath = file;
            this.InitializeComponent();
        }

        #region Component Designer Generated Code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FlashView));
            this.flashMovie = new AxShockwaveFlashObjects.AxShockwaveFlash();
            ((System.ComponentModel.ISupportInitialize)(this.flashMovie)).BeginInit();
            this.SuspendLayout();
            // 
            // flashMovie
            // 
            this.flashMovie.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flashMovie.Enabled = true;
            this.flashMovie.Location = new System.Drawing.Point(0, 0);
            this.flashMovie.Name = "flashMovie";
            this.flashMovie.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("flashMovie.OcxState")));
            this.flashMovie.Size = new System.Drawing.Size(571, 339);
            this.flashMovie.TabIndex = 0;
            // 
            // FlashView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flashMovie);
            this.Name = "FlashView";
            this.Size = new System.Drawing.Size(571, 339);
            this.Load += new System.EventHandler(this.FlashViewLoad);
            ((System.ComponentModel.ISupportInitialize)(this.flashMovie)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        #region Methods And Event Handlers

        /// <summary>
        /// Accessor for the movie file path
        /// </summary>
        public String MoviePath
        {
            get { return this.moviePath; }
            set 
            { 
                this.moviePath = value;
                this.flashMovie.LoadMovie(0, this.moviePath); 
            }
        }

        /// <summary>
        /// Accessor for the flash movie
        /// </summary>
        public AxShockwaveFlash FlashMovie
        {
            get { return this.flashMovie; }
        }

        /// <summary>
        /// Initializes the control
        /// </summary>
        private void FlashViewLoad(object sender, EventArgs e)
        {
            this.flashMovie.FSCommand += this.FlashMovieFSCommand;
            this.flashMovie.FlashCall += this.FlashMovieFlashCall;
            if (this.moviePath != null)
            {
                this.flashMovie.LoadMovie(0, this.moviePath);
            }
        }

        /// <summary>
        /// Handles the FSCommand event
        /// </summary>
        private void FlashMovieFSCommand(Object sender, _IShockwaveFlashEvents_FSCommandEvent e)
        {

            try
            {
                if (e.command == "trace")
                {
                    TraceManager.Add(e.args, 1);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        private void FlashMovieFlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(new StringReader(e.request));
                reader.WhitespaceHandling = WhitespaceHandling.Significant;
                reader.MoveToContent();

                if (reader.Name == "invoke" && reader.GetAttribute("name") == "trace")
                {
                    reader.Read();
                    if (reader.Name == "arguments")
                    {
                        reader.Read();
                        if (reader.Name == "string")
                        {
                            reader.Read();
                            TraceManager.Add(reader.Value, 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        #endregion

    }

}
