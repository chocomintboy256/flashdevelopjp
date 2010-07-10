using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using MultiView.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using ScintillaNet;
using System.Collections.Generic;
using FlashDevelop;
using FlashDevelop.Docking;
using PluginCore.Controls;
using System.Text;

namespace MultiView
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "MultiView";
        private String pluginGuid = "42ac7fab-421b-1f38-a985-72a03534f731";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "MultiView for the new FlashDevelop 3.";
        private String pluginAuth = "matsumos";
        private String settingFilename;
        private Settings settingObject;
        //private DockContent pluginPanel;
        //private PluginUI pluginUI;
        private Image pluginImage;

	    #region Required Properties

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
		{
			get { return this.pluginName; }
		}

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
		{
			get { return this.pluginGuid; }
		}

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
		{
			get { return this.pluginAuth; }
		}

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
		{
			get { return this.pluginDesc; }
		}

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
		{
			get { return this.pluginHelp; }
		}

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }
		
		#endregion
		
		#region Required Methods
		
		/// <summary>
		/// Initializes the plugin
		/// </summary>
		public void Initialize()
		{
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
            this.InitLocalization();
            //this.CreatePluginPanel();
            this.CreateMenuItem();
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.SaveSettings();

            syncMaster = null;
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            switch (e.Type)
            {
                /*
                // Catches FileSwitch event and displays the filename it in the PluginUI.
                case EventType.FileSwitch:
                    string fileName = PluginBase.MainForm.CurrentDocument.FileName;
                    pluginUI.Output.Text += fileName + "\r\n";
                    TraceManager.Add("Switched to " + fileName); // tracing to output panel
                    break;

                // Catches Project change event and display the active project path
                case EventType.Command:
                    string cmd = (e as DataEvent).Action;
                    if (cmd == "ProjectManager.Project")
                    {
                        IProject project = PluginBase.CurrentProject;
                        if (project == null)
                            pluginUI.Output.Text += "Project closed.\r\n";
                        else
                            pluginUI.Output.Text += "Project open: " + project.ProjectPath + "\r\n";
                    }
                    break;
                */
                case EventType.FileSave:
                    FileSavedHandler();
                    break;
            }
		}
		
		#endregion

        #region Custom Methods
       
        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "SamplePlugin");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("100");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            // Set events you want to listen (combine as flags)
            EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.Command | EventType.FileSave);
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public void InitLocalization()
        {
            LocaleVersion locale = PluginBase.MainForm.Settings.LocaleVersion;
            switch (locale)
            {
                /*
                case LocaleVersion.fi_FI : 
                    // We have Finnish available... or not. :)
                    LocaleHelper.Initialize(LocaleVersion.fi_FI);
                    break;
                */
                default : 
                    // Plugins should default to English...
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
            this.pluginDesc = LocaleHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            /*
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            viewMenu.DropDownItems.Add(new ToolStripMenuItem(LocaleHelper.GetString("Label.ViewMenuItem"), this.pluginImage, new EventHandler(this.OpenPanel), this.settingObject.SampleShortcut));
            PluginBase.MainForm.IgnoredKeys.Add(this.settingObject.SampleShortcut);
            */

            ContextMenuStrip editorMenu = PluginBase.MainForm.EditorMenu;

            ToolStripMenuItem menuItem = new ToolStripMenuItem("CloneView", null, new EventHandler(delegate { CloneCurrentDocument(); }));
            menuItem.ShortcutKeys = this.settingObject.CloneViewShortcut;
            editorMenu.Items.Insert(4, menuItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        ///
        /*
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = LocaleHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockRight);
        }
        */
        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        /*
        public void OpenPanel(Object sender, System.EventArgs e)
        {
            this.pluginPanel.Show();
        }
        */
        private void setup()
        {
            documentList = new List<ITabbedDocument>();
            Globals.MainForm.DockPanel.ActiveDocumentChanged += new EventHandler(delegate { DockPanel_ActiveDocumentChanged(); });
            UITools.Manager.OnMarkerChanged += new UITools.LineEventHandler(Manager_OnMarkerChanged);
        }

        List<ITabbedDocument> documentList;
        ITabbedDocument syncMaster;
        List<ITabbedDocument> syncChildren = new List<ITabbedDocument>();
        
        public void CloneCurrentDocument()
        {
            if (documentList == null) setup();

            ITabbedDocument currentDocument = Globals.CurrentDocument;
            if (!documentList.Contains(currentDocument))
            {
                register(currentDocument);
            }

            DockContent clonedDocument = Globals.MainForm.CreateEditableDocument(currentDocument.SciControl.FileName, currentDocument.SciControl.Text, currentDocument.SciControl.CodePage);
            ITabbedDocument clonedoc = (TabbedDocument)clonedDocument;
            register(clonedoc);

            for (int i = 0; i < currentDocument.SciControl.LineCount; i++)
            {
                if (currentDocument.SciControl.MarkerGet(i) == 1) clonedoc.SciControl.MarkerAdd(i, 0);
                //currentDocument

                //currentDocument.SciControl.FoldParent(i + 1);

                Int32 foldParentLine = currentDocument.SciControl.FoldParent(i+1);
                if (foldParentLine == i)
                {
                    Boolean isExpanded = currentDocument.SciControl.FoldExpanded(foldParentLine);
                    if (!isExpanded)
                    {
                        clonedoc.SciControl.ToggleFold(i);
                    }
                }
            }

            clonedoc.IsModified = currentDocument.IsModified;
            clonedoc.SciControl.SelectionMode = currentDocument.SciControl.SelectionMode;
            clonedoc.SciControl.SelectionStart = currentDocument.SciControl.SelectionStart;
            clonedoc.SciControl.SelectionEnd = currentDocument.SciControl.SelectionEnd;

            clonedDocument.DockTo(((DockContent) currentDocument).Pane, settingObject.DockStyle, 0);

            DockPanel_ActiveDocumentChanged();

            syncMaster.Activate();
        }

        void register(ITabbedDocument doc)
        {
            documentList.Add(doc);
            doc.SciControl.Disposed += new EventHandler(delegate { documentList.Remove(doc); });
        }

        void DockPanel_ActiveDocumentChanged()
        {
            if (documentList == null) return;
            
            if (syncMaster != null && syncMaster.SciControl != null && !syncMaster.SciControl.Disposing)
            {
                syncMaster.SciControl.Modified -= new ModifiedHandler(SciControl_Modified);
            }
            syncMaster = null;

            ITabbedDocument currentDocument = Globals.CurrentDocument;

            if (!documentList.Contains(currentDocument)) return;

            syncMaster = currentDocument;

            syncChildren = documentList.FindAll(sameContent);

            if (syncChildren.Count < 1)
            {
                documentList.Remove(syncMaster);
                return;
            }
            else
            {
                syncMaster.SciControl.Modified += new ModifiedHandler(SciControl_Modified);
            }
        }

        public static Int32 GetMarkerMask(Int32 marker)
        {
            return 1 << marker;
        }

        void Manager_OnMarkerChanged(ScintillaControl sender, int line)
        {
            if (sender == syncMaster.SciControl)
            {
                int isMark = syncMaster.SciControl.MarkerGet(line);

                foreach (ITabbedDocument syncChild in syncChildren)
                {
                    if (isMark == 1) syncChild.SciControl.MarkerAdd(line, 0);
                    else if (isMark == 0) syncChild.SciControl.MarkerDelete(line, 0);
                }
                 
            }
        }

        private bool hasTwoByteChar(string str)
        {
            byte[] byte_data = System.Text.Encoding.GetEncoding(932).GetBytes(str);
            if (byte_data.Length == str.Length) return false;
            else return true;
        }

        void SciControl_Modified(ScintillaControl sender, int position, int modificationType, string text, int length, int linesAdded, int line, int foldLevelNow, int foldLevelPrev)
        {
            if (settingObject.DisableSync) return;

            if (hasTwoByteChar(syncMaster.SciControl.Text.Substring(0, position)))
            {
                switch (modificationType & 0xF)
                {
                    case 1: //SC_MOD_INSERTTEXT
                    case 2: //SC_MOD_DELETETEXT
                        foreach (ITabbedDocument syncChild in syncChildren)
                        {
                            int oldSelStart = syncChild.SciControl.SelectionStart;
                            //int caretLine = syncChild.SciControl.LineFromPosition(oldSelStart);

                            // スクロール位置を保持
                            Int32 scrollTop = syncChild.SciControl.FirstVisibleLine;

                            syncChild.SciControl.SetText(syncMaster.SciControl.Text);

                            syncChild.SciControl.SelectionStart = oldSelStart;
                            //syncChild.SciControl.LineScroll(0, caretLine);

                            // スクロール位置を元に戻す
                            if (scrollTop != syncChild.SciControl.FirstVisibleLine)
                            {
                                syncChild.SciControl.LineScroll(0, scrollTop - syncChild.SciControl.FirstVisibleLine);
                            }
                        }
                        break;
                }

            }
            else
            {
                switch (modificationType & 0xF)
                {
                    case 1: //SC_MOD_INSERTTEXT
                        foreach (ITabbedDocument syncChild in syncChildren)
                        {
                            syncChild.SciControl.InsertText(position, syncMaster.SciControl.Text.Substring(position, length));
                        }
                        break;
                    case 2: //SC_MOD_DELETETEXT
                        foreach (ITabbedDocument syncChild in syncChildren)
                        {
                            syncChild.SciControl.TargetStart = position;
                            syncChild.SciControl.TargetEnd = position + length;
                            syncChild.SciControl.ReplaceTarget(-1, null);
                        }
                        break;
                }
            }

            syncMaster.Activate();

            /*
            TraceManager.Add("---------------------------------------");
            TraceManager.Add("sender           :" + sender.ToString());
            TraceManager.Add("position         :" + position.ToString());
            TraceManager.Add("modificationType :" + modificationType.ToString());
            TraceManager.Add("text             :" + text.ToString());
            TraceManager.Add("length           :" + length.ToString());
            TraceManager.Add("linesAdded       :" + linesAdded.ToString());
            TraceManager.Add("line             :" + line.ToString());
            TraceManager.Add("foldLevelNow     :" + foldLevelNow.ToString());
            TraceManager.Add("foldLevelPrev    :" + foldLevelPrev.ToString());
            */
        }

        void FileSavedHandler()
        {
            if(syncMaster == null) return;
            if (syncMaster.IsModified) return;

            foreach (ITabbedDocument syncChild in syncChildren)
            {
                syncChild.Reload(false);
                syncChild.IsModified = false;
            }
        }

        private bool sameContent(ITabbedDocument doc)
        {
            if (   syncMaster == null
                || syncMaster.SciControl == null
                || syncMaster.SciControl.Disposing) return false;

            if (   doc == null
                || doc.SciControl == null
                || doc.SciControl.Disposing) return false;

            if (doc.SciControl.FileName.Equals(syncMaster.SciControl.FileName) && doc != syncMaster) return true;
            else return false;
        }

		#endregion

	}
	
}
