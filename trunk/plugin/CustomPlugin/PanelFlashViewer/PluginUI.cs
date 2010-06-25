using System;
using System.Collections;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore;
using System.Text;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Collections.Generic;
using System.ComponentModel;
using AxShockwaveFlashObjects;
using PluginCore.Localization;
using PluginCore.Managers;
using System.IO;
using System.Xml;
using PanelFlashViewer.Controls;
using FlashDevelop;
using PluginCore.Helpers;
using ProjectManager.Projects;
using FlashDevelop.Dialogs;
using AS3Context.Compiler;
using PanelFlashViewer.Resources;
using PanelFlashViewer;
using FlashTools;

namespace PanelFlashViewer
{
    public class PluginUI : UserControl
    {
        private PluginMain pluginMain;
        private AxShockwaveFlash flashMovie;
        private String moviePath;
        private ToolStrip toolStrip1;
        private ToolStripButton reloadButton;
        private ToolStripButton loadButton;
        private ToolStripButton unloadButton;
        private ToolStripButton openButton;
        private ToolStripButton resizeButton;
        private ToolStripButton settingButton;
        private ToolStripLabel fileNameLabel;
        private ToolStripNumericUpDown widthTextBox;
        private ToolStripNumericUpDown heightTextBox;
        private Panel panel;
        private Settings settingObject;
        private MainForm mainForm;
        private Boolean isFit = false;
        public bool IsSWFPlaying = false;
        public string file = "";
        private int defaultSWFWidth;
        private int defaultSWFHeight;

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

        public PluginUI(PluginMain pluginMain, Settings setting)
        {
            this.pluginMain = pluginMain;
            settingObject = setting;
            this.InitializeComponent();
        }

        /// <summary>
        /// Accessor to the RichTextBox
        /// </summary>
        /// 

        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.reloadButton = new System.Windows.Forms.ToolStripButton();
            this.loadButton = new System.Windows.Forms.ToolStripButton();
            this.fileNameLabel = new System.Windows.Forms.ToolStripLabel();
            this.unloadButton = new System.Windows.Forms.ToolStripButton();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.resizeButton = new System.Windows.Forms.ToolStripButton();
            //this.savesizeButton = new System.Windows.Forms.ToolStripButton();
            this.settingButton = new System.Windows.Forms.ToolStripButton();
            this.widthTextBox = new ToolStripNumericUpDown();
            this.heightTextBox = new ToolStripNumericUpDown();

            ToolStripLabel w = new ToolStripLabel();
            ToolStripLabel h = new ToolStripLabel();

            panel = new Panel();

            this.toolStrip1.SuspendLayout();

            this.SuspendLayout();
            
            panel.AutoScroll = true;
            panel.Location = new System.Drawing.Point(1, 25);

            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.reloadButton,
                this.loadButton,
                this.unloadButton,
                this.openButton,
                new ToolStripSeparator(),
                this.fileNameLabel,
                this.heightTextBox,
                h,
                this.widthTextBox,
                w,
                this.resizeButton,
                this.settingButton
            });

            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Size = new System.Drawing.Size(571, 25);
            this.toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(1, 1, 2, 1);
 
            this.reloadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.reloadButton.Image = IGet("66").Img;
            this.reloadButton.ToolTipText = LocaleHelper.GetString("Label.Reload");
            this.reloadButton.Size = new System.Drawing.Size(23, 22);
            this.reloadButton.Click += new System.EventHandler(this.reloadButtonClick);

            this.loadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.loadButton.Image = IGet("300").Img;
            this.loadButton.ToolTipText = LocaleHelper.GetString("Label.Load");
            this.loadButton.Size = new System.Drawing.Size(23, 22);
            this.loadButton.Click += new System.EventHandler(this.loadButtonClick);

            this.unloadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.unloadButton.Image = IGet("153").Img;
            this.unloadButton.Size = new System.Drawing.Size(23, 22);
            this.unloadButton.ToolTipText = LocaleHelper.GetString("Label.Unload");
            this.unloadButton.Click += new System.EventHandler(this.uploadButtonClick);

            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openButton.Image = IGet("214").Img;
            this.openButton.Size = new System.Drawing.Size(23, 22);
            this.openButton.ToolTipText = LocaleHelper.GetString("Label.Open");
            this.openButton.Click += new System.EventHandler(this.openButtonClick);

            this.fileNameLabel.Size = new System.Drawing.Size(81, 22);
            this.fileNameLabel.Text = "";

            this.resizeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.resizeButton.Image = IGet("489").Img;
            this.resizeButton.Size = new System.Drawing.Size(23, 22);
            this.resizeButton.ToolTipText = LocaleHelper.GetString("Label.Resize");
            this.resizeButton.Alignment = ToolStripItemAlignment.Right;
            this.resizeButton.Click += new System.EventHandler(this.changeDisplayState);
            
            this.settingButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingButton.Image = IGet("54").Img;
            this.settingButton.Size = new System.Drawing.Size(23, 22);
            this.settingButton.ToolTipText = LocaleHelper.GetString("Label.Setting");
            this.settingButton.Alignment = ToolStripItemAlignment.Right;
            this.settingButton.Click += new System.EventHandler(openSetting);            

            this.widthTextBox.Control.Width = 55;
            this.widthTextBox.TextAlign = HorizontalAlignment.Right;
            this.widthTextBox.Alignment = ToolStripItemAlignment.Right;
            this.widthTextBox.Maximum = 99999;
            this.widthTextBox.KeyUp += new KeyEventHandler(textBoxEnter);

            this.heightTextBox.Control.Width = 55;
            this.heightTextBox.TextAlign = HorizontalAlignment.Right;
            this.heightTextBox.Alignment = ToolStripItemAlignment.Right;
            this.heightTextBox.Maximum = 99999;
            this.heightTextBox.KeyUp += new KeyEventHandler(textBoxEnter);

            w.Text = " W:";
            w.Alignment = ToolStripItemAlignment.Right;
            h.Text = " H:";
            h.Alignment = ToolStripItemAlignment.Right;

            // 
            // PluginUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel);

            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(571, 339);
            this.Resize += new System.EventHandler(this.ResizeHandler);

            this.toolStrip1.ResumeLayout();
            this.ResumeLayout(false);

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(PluginUI_DragEnter);
            this.DragDrop += new DragEventHandler(PluginUI_DragDrop);

            
        }

        private void PluginUI_DragDrop(object sender, DragEventArgs e)
        {
            // ドロップされたファイルの名称が配列で渡される
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            // 最初に受け取った（0番目の）画像を表示（※この方法だとファイルロック状態になるらしい）
            //PictureBox1.Picture = Image.FromFile(fileName[0]);

            OpenSWF(fileName[0]);
        }

        private void PluginUI_DragEnter(object sender, DragEventArgs e)
        {
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            // ファイルがコントロール上にきたら
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && fileName[0].EndsWith(".swf"))
                // 該当ファイルをコピー（+のアイコン状態）で受け取る
                e.Effect = DragDropEffects.Copy;
            else
                // ファイルでなかったら（文字列とかだと）拒否マークで受け付けない
                e.Effect = DragDropEffects.None;
        }

        private void textBoxEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != 13) return; 
            if (IsNumeric(this.widthTextBox.Text) && IsNumeric(this.heightTextBox.Text))
            {
                flashMovie.Width = int.Parse(this.widthTextBox.Text);
                flashMovie.Height = int.Parse(this.heightTextBox.Text);

                settingObject.SWFWidth = flashMovie.Width;
                settingObject.SWFHeight = flashMovie.Height;
            }            
        }

        public static bool IsNumeric(string stTarget)
        {
            double dNullable;

            return double.TryParse(
                stTarget,
                System.Globalization.NumberStyles.Any,
                null,
                out dNullable
            );
        }

        /*
        private void saveSize()
        {
            if (flashMovie!=null)
            {
                this.lastWidth = flashMovie.Width;
                this.lastHeight = flashMovie.Height;

                settingObject.SWFWidth = flashMovie.Width;
                settingObject.SWFHeight = flashMovie.Height;
            }
        }
        */

        private void openSetting(object sender, EventArgs e)
        {
            SettingDialog.Show("PanelFlashViewer","");
        }

        private void changeDisplayState(object sender, EventArgs e)
        {
            switch (settingObject.DisplayState)
            {
                case DisplayState.NO_SCALE:
                    SetDisplayState(DisplayState.EXACT_FIT);
                    break;
                case DisplayState.EXACT_FIT:
                    SetDisplayState(DisplayState.FREE);
                    break;
                case DisplayState.FREE:
                    SetDisplayState(DisplayState.NO_SCALE);
                    break;
            }
        }

        public void SetDisplayState(DisplayState displayState)
        {
            switch (displayState)
            {
                case DisplayState.NO_SCALE:
                    Console.WriteLine("NO_SCALE");

                    isFit = false;

                    this.resizeButton.Image = IGet("488").Img;
                    if (flashMovie != null)
                    {
                        this.flashMovie.Width = defaultSWFWidth;
                        this.flashMovie.Height = defaultSWFHeight;

                    }
                    this.widthTextBox.Enabled = false;
                    this.heightTextBox.Enabled = false;

                    break;
                case DisplayState.EXACT_FIT:
                    Console.WriteLine("EXACT_FIT");
                    isFit = true;

                    this.resizeButton.Image = IGet("489").Img;
                    if (flashMovie != null)
                    {
                        this.flashMovie.Width = this.panel.Width;
                        this.flashMovie.Height = this.panel.Height;

                    }
                    this.widthTextBox.Enabled = false;
                    this.heightTextBox.Enabled = false;

                    break;
                case DisplayState.FREE:
                    Console.WriteLine("FREE");
                    isFit = false;

                    this.resizeButton.Image = IGet("61").Img;
                    if (flashMovie != null)
                    {
                        this.flashMovie.Width = settingObject.SWFWidth;
                        this.flashMovie.Height = settingObject.SWFHeight;

                    }
                    this.widthTextBox.Enabled = true;
                    this.heightTextBox.Enabled = true;
                    break;
            }

            this.settingObject.DisplayState = displayState;
        }
        

        private void setupView()
        {
            DataEvent de = new DataEvent(EventType.Command, "AS3Context.StartDebugger",null);
            EventManager.DispatchEvent(this, de);

            this.flashMovie = new AxShockwaveFlashObjects.AxShockwaveFlash();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginUI));

            ((System.ComponentModel.ISupportInitialize)(this.flashMovie)).BeginInit();

            this.flashMovie.Enabled = true;
            this.flashMovie.Location = new System.Drawing.Point(0, 0);
            this.flashMovie.Name = "flashMovie";
            this.flashMovie.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("flashMovie.OcxState")));
            this.flashMovie.FSCommand += this.FlashMovieFSCommand;
            this.flashMovie.FlashCall += this.FlashMovieFlashCall;

            this.flashMovie.Resize += new EventHandler(flashMovie_Resize);

            this.panel.Controls.Add(this.flashMovie);

            ((System.ComponentModel.ISupportInitialize)(this.flashMovie)).EndInit();

            SetDisplayState(settingObject.DisplayState);
        }

        private void flashMovie_Resize(object sender, EventArgs e)
        {
            this.widthTextBox.Text = flashMovie.Width.ToString();
            this.heightTextBox.Text = flashMovie.Height.ToString();
        }
        
        private void ResizeHandler(object sender, EventArgs e)
        {
            this.panel.Width = this.Width - 2;
            this.panel.Height = this.Height - 27;

            if (isFit && flashMovie != null)
            {
                this.flashMovie.Width = this.panel.Width;
                this.flashMovie.Height = this.panel.Height;
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

        public void unloadMovie()
        {
            fileNameLabel.Text = "";
            file = "";
            if (flashMovie != null)
            {
                IsSWFPlaying = false;
                flashMovie.Dispose();
                flashMovie = null;
            }
        }

        #endregion

        private void reloadButtonClick(object sender, EventArgs e)
        {
            OpenSWF(moviePath);
        }

        private void loadButtonClick(object sender, EventArgs e)
        {
            
            Project project = PluginBase.CurrentProject as Project;
            if (project != null)
            {
                OpenSWF(project.OutputPathAbsolute);
            }
            
        }

        public void OpenSWF(String path)
        {
            unloadMovie();

            if (System.IO.File.Exists(path))
            {
                SWFFile swfFile = new SWFFile(path);

                defaultSWFWidth = swfFile.FrameWidth / 20;
                defaultSWFHeight = swfFile.FrameHeight / 20;

                swfFile.Close();

                fileNameLabel.Text = System.IO.Path.GetFileName(path);
                file = path;
                setupView();
                MoviePath = path;

                IsSWFPlaying = true;
            }
            else
            {
                file = "";
                fileNameLabel.Text = "";
                ErrorManager.ShowInfo(LocaleHelper.GetString("Info.FileExists"));
            }

            
        }

        private void uploadButtonClick(object sender, EventArgs e)
        {
            unloadMovie();
        }

        private void openButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = PluginBase.CurrentProject.ProjectPath;
            ofd.Filter =
                "SWF file(*.swf)|*.swf";//|All files(*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenSWF(ofd.FileName);
            }


        }

        public FDImage IGet(string data)
        {
            
            if (mainForm == null) mainForm = (MainForm)Globals.MainForm;
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
}
