using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using PanelFlashViewer.Controls;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using PanelFlashViewer.Resources;

namespace PanelFlashViewer
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "PanelFlashViewer";
        private String pluginGuid = "9fe62cd3-027f-4cfe-bca5-b0d52118f083";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Displays flash movies in FlashDevelop.";
        private String pluginAuth = "matsumos";
        private List<Form> popups = new List<Form>();
        private String settingFilename;
        private Settings settingObject;
        private DockContent pluginPanel;
        private PluginUI pluginUI;
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
            this.InitLocalization();
            this.LoadSettings();
            this.AddEventHandlers();
            this.CreatePluginPanel();
            this.CreateMenuItem();
        }            

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public void InitLocalization()
        {
            LocaleVersion locale = PluginBase.MainForm.Settings.LocaleVersion;
            switch (locale)
            {
                case LocaleVersion.ja_JP:
                    LocaleHelper.Initialize(LocaleVersion.ja_JP);
                    break;
                default:
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }

            this.pluginDesc = LocaleHelper.GetString("Info.Description");
        }

		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{            
            switch (e.Type)
            {
                case EventType.Command :
                    this.HandleCommand(((DataEvent)e));
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
            String dataPath = Path.Combine(PathHelper.DataDir, "PanelFlashViewer");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            String resource = "PanelFlashViewer.Resources.Player.ico";
            Stream stream = assembly.GetManifestResourceStream(resource);
        //    this.pluginDesc = LocaleHelper.GetString("Info.Description");
            this.pluginImage = System.Drawing.Image.FromStream(stream);
            

        }


        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            //EventManager.AddEventHandler(this, EventType.FileOpening, HandlingPriority.High);
            EventManager.AddEventHandler(this, EventType.Command);
        }

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
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            viewMenu.DropDownItems.Add(new ToolStripMenuItem("PanelFlashViewer", this.pluginImage, new EventHandler(this.OpenPanel)));
            //PluginBase.MainForm.IgnoredKeys.Add(this.settingObject.SampleShortcut);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this, settingObject);
            this.pluginUI.Text = LocaleHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockRight);

            this.pluginPanel.DockStateChanged += new EventHandler(delegate{
                if (this.pluginPanel.DockState == DockState.Hidden)
                {
                    CloseHandler();
                }
            });
        }
        
        private void CloseHandler()
        {
            if (pluginUI.IsSWFPlaying)
            {
                pluginUI.unloadMovie();
            }
        }
        

        /// <summary>
        /// Handles the Command event and displays the movie
        /// </summary>
        public void HandleCommand(DataEvent evnt)
        {
            try
            {
                if (evnt.Action.StartsWith("PanelFlashViewer."))
                {
                    String action = evnt.Action;
                    String[] args = evnt.Data.ToString().Split(',');
                    switch (action)
                    {
                        case "PanelFlashViewer.Panel":
                            this.pluginPanel.Show();
                            this.pluginUI.OpenSWF(args[0]);
                            break;
                    }
                    evnt.Handled = true;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        private void NotifyDisposed(String file)
        {
            DataEvent de = new DataEvent(EventType.Command, "FlashViewer.Closed", file);
            EventManager.DispatchEvent(this, de);
        }

        public void OpenPanel(Object sender, System.EventArgs e)
        {
            this.pluginPanel.Show();
        }

		#endregion

	}
	
}
