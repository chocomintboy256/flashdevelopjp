using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using AutoBuildPlugin.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using ProjectManager.Projects;
using ProjectManager;
using ASCompletion.Model;
using ASCompletion.Context;
using System.Collections.Generic;
using ProjectManager.Actions;
using FlashDevelop;
using FlashDevelop.Utilities;
using System.Collections;
using ProjectManager.Projects.AS3;
using ProjectManager.Projects.AS2;
using ProjectManager.Helpers;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace AutoBuildPlugin
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "AutoBuildPlugin";
        private String pluginGuid = "686ba6b0-e888-4adc-82bf-f4ed60263b3e";
        private String pluginHelp = "";
        private String pluginDesc = "AutoBuildPlugin for the FlashDevelop 3.";
        private String pluginAuth = "matsumos";
        private String settingFilename;
        private Settings settingObject;
        private Image pluginImage;
        private BuildAction buildAction;

	    #region Required Properties

        public String Name { get { return this.pluginName; } }
        public String Guid { get { return this.pluginGuid; } }
        public String Author { get { return this.pluginAuth; } }
        public String Description { get { return this.pluginDesc; } }
        public String Help { get { return this.pluginHelp; } }

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
        }

		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            this.stopWatcher();
            this.SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
        {
            TextEvent te = e as TextEvent;

            switch (e.Type)
            {
                
                case EventType.UIStarted:
                    UIStartedHandler();
                    break;
                 
                // Catches Project change event and display the active project path
                case EventType.Command:
                    string cmd = (e as DataEvent).Action;
                    if (cmd == "ProjectManager.Project")
                    {
                        IProject project = PluginBase.CurrentProject;
                        if (project == null)
                            projectWasClosed();
                        else
                            projectIsOpen();
                    }
                    break;
                case EventType.ProcessStart:
                    buildAction.NotifyBuildStarted();
                    break;
                case EventType.ProcessEnd:
                    string result = te.Value;
                    buildAction.NotifyBuildEnded(result);
                    break;
                case EventType.Keys:
                    KeyEvent ke = (KeyEvent)e;
                    if (ke.Value == this.settingObject.ToggleAutoBuildShortcut)
                    {
                        ke.Handled = true;
                        ToggleBuild();
                    }
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
            String dataPath = Path.Combine(PathHelper.DataDir, "AutoBuildPlugin");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("100");
            buildAction = new BuildAction();
            buildAction.BuildComplete += new BuildCompleteHandler(delegate { OpenSwf(); });
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted, HandlingPriority.Low);
            EventType eventMask = EventType.Command | EventType.Keys | EventType.ProcessStart | EventType.ProcessEnd;
            EventManager.AddEventHandler(this, eventMask);
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public void InitLocalization()
        {
            LocaleVersion locale = PluginBase.MainForm.Settings.LocaleVersion;
            switch (locale)
            {
                case LocaleVersion.ja_JP : 
                    LocaleHelper.Initialize(LocaleVersion.ja_JP);
                    break;
                default : 
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
            this.pluginDesc = LocaleHelper.GetString("Info.Description");
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

        private ToolStripButton ToggleBuildButton;
        private ToolStripButton ReloadProjectButton;
        private ToolStripComboBox ProjectsComboBox;

        private void AddToolStrip()
        {
            if (ToggleBuildButton != null) return;

            ToggleBuildButton = new ToolStripButton(Get("127|11|5|5").Img);
            ToggleBuildButton.ToolTipText = LocaleHelper.GetString("Label");
            ToggleBuildButton.Click += new EventHandler(ToggleBuildButtonClick);

            ProjectsComboBox = new ToolStripComboBox();
            ProjectsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ProjectsComboBox.Enabled = false;
            ProjectsComboBox.FlatStyle = PluginBase.MainForm.Settings.ComboBoxFlatStyle;
            ProjectsComboBox.Font = PluginBase.Settings.DefaultFont;
            ProjectsComboBox.SelectedIndexChanged += delegate
            {
                ChangeProject(ProjectsComboBox.SelectedIndex);
            };

            ReloadProjectButton = new ToolStripButton(Get("66").Img);
            ReloadProjectButton.ToolTipText = LocaleHelper.GetString("Label.ReloadProjects");
            ReloadProjectButton.Click += new EventHandler(ReloadProjectButtonClick);

            PluginBase.MainForm.ToolStrip.Items.Add(new ToolStripSeparator());
            PluginBase.MainForm.ToolStrip.Items.Add(ToggleBuildButton);
            PluginBase.MainForm.ToolStrip.Items.Add(ProjectsComboBox);
            PluginBase.MainForm.ToolStrip.Items.Add(ReloadProjectButton);
        }

        
		#endregion

        
        private void UIStartedHandler()
        {
            AddToolStrip();
        }


        private Project project;
        private ClasspathWatcher classpathWatcher;
        private SingleFileWatcher singleFileWatcher;

        public void startWatcher()
        {
            if (isRunning)
            {
                ErrorManager.ShowInfo(LocaleHelper.GetString("Info.WatcherWasRunning"));
                return;
            }

            changeState(UIState.Running);
            classpathWatcher = new ClasspathWatcher(Globals.MainForm.ToolStrip, project, delegate() { buildAction.Build(project, true); });
            //singleFileWatcher = new SingleFileWatcher(Globals.MainForm.ToolStrip, project.OutputPathAbsolute, delegate() { OpenSwf(); });

            buildAction.Build(project, true);

            ToggleBuildButton.ToolTipText = LocaleHelper.GetString("Label.Stop");

            isRunning = true;
        }

        public void stopWatcher()
        {
            if (classpathWatcher != null)
            {
                classpathWatcher.Dispose();
                classpathWatcher = null;
            }

            if (singleFileWatcher != null)
            {
                singleFileWatcher.Dispose();
                singleFileWatcher = null;
            }

            if (project != null)
            {
                changeState(UIState.Standby);
            }
            else
            {
                changeState(UIState.Disabled);
            }

            ToggleBuildButton.ToolTipText = LocaleHelper.GetString("Label.Start");

            isRunning = false;
        }

        public bool isRunning = false;

        private void projectIsOpen()
        {
            if (isRunning) return;

            ReloadProject();

            changeState(UIState.Standby);

        }

        private List<Project> projectList;

        public void ReloadProject()
        {
            projectList = new List<Project>();
            ProjectsComboBox.Items.Clear();
                        
            project = (Project)PluginBase.CurrentProject;

            if (project != null)
            {
                ProjectsComboBox.Enabled = true;

                ProjectsComboBox.Items.Add(project.Name);
                projectList.Add(project);

                // 開いたプロジェクトと同じ階層にあるプロジェクトを検索
                string[] fs = System.IO.Directory.GetFiles(project.Directory, "*.as3proj");

                // リストに登録
                foreach (string projectfile in fs)
                {
                    if (projectfile == project.ProjectPath) continue;
                    Project proj = ProjectLoader.Load(projectfile);
                    ProjectsComboBox.Items.Add(proj.Name);
                    projectList.Add(proj);
                }
                ProjectsComboBox.SelectedIndex = 0;
            }
            else
            {
                ProjectsComboBox.Enabled = false;
            }
        }

        private void ChangeProject(int Index)
        {
            project = projectList[Index];
        }
        
        private void projectWasClosed()
        {
            changeState(UIState.Disabled);
        }

        private void OpenSwf()
        {
            DataEvent de;

            string path = project.OutputPathAbsolute;

            int w = project.MovieOptions.Width;
            int h = project.MovieOptions.Height;

            try
            {
                switch(this.settingObject.DisplayStyle){
                        
                    case ViewStyle.Popup:
                        //Console.WriteLine("FlashViewer.Popup");
                        de = new DataEvent(EventType.Command, "FlashViewer.Popup", path + "," + w.ToString() + "," + h.ToString());
                        EventManager.DispatchEvent(this, de);
                    break;
                    case ViewStyle.Document:
                        //Console.WriteLine("FlashViewer.Document");
                        de = new DataEvent(EventType.Command, "FlashViewer.Document", path);
                        EventManager.DispatchEvent(this, de);
                    break;
                    case ViewStyle.External:
                        //Console.WriteLine("FlashViewer.External");
                        de = new DataEvent(EventType.Command, "FlashViewer.External", path);
                        EventManager.DispatchEvent(this, de);
                    break;
                    case ViewStyle.Panel:
                        //Console.WriteLine("PanelFlashViewer.Panel");
                        de = new DataEvent(EventType.Command, "PanelFlashViewer.Panel", path);
                        EventManager.DispatchEvent(this, de);
                    break;
                    default:
                    break;
                }
             
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
            
        }

        /*
        public string projectName
        {
            set { if (ProjectNameDropDown != null) ProjectNameDropDownProjectNameLabel.Text = value; }
        }
        */

        public UIState state;

        public void changeState(UIState state)
        {
            switch (state)
            {
                case UIState.Standby:
                    this.ToggleBuildButton.Image = Get("127|11|5|5").Img;
                    this.ToggleBuildButton.Enabled = true;
                    ReloadProjectButton.Enabled = true;
                    break;
                case UIState.Running:
                    this.ToggleBuildButton.Image = Get("127|23|5|5").Img;
                    this.ToggleBuildButton.Enabled = true;
                    ReloadProjectButton.Enabled = false;
                    break;
                case UIState.Disabled:
                    this.ToggleBuildButton.Image = Get("127|11|5|5").Img;
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
            ReloadProject();
        }

        private void ToggleBuildButtonClick(object sender, System.EventArgs e)
        {
            ToggleBuild();
        }

        private void ToggleBuild()
        {
            switch (state)
            {
                case UIState.Standby:
                    startWatcher();
                    break;
                case UIState.Running:
                    stopWatcher();
                    break;
                case UIState.Disabled:
                    break;
                default:
                    break;
            }
        }

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

    class ProjectLoader
    {
        public static Project Load(string file)
        {
            string ext = Path.GetExtension(file).ToLower();

            if (FileInspector.IsProject(file))
            {
                Type projectType =
                    ProjectCreator.GetProjectType(ProjectCreator.KeyForProjectPath(file));
                if (projectType != null)
                {
                    object[] para = new object[1];
                    para[0] = file;
                    return (Project)projectType.GetMethod("Load").Invoke(null, para);
                }
                else
                {
                    throw new Exception("Invalid project type: " + Path.GetFileName(file));
                }
            }
            else
            {
                throw new Exception("Unknown project extension: " + Path.GetFileName(file));
            }
        }
    }
}
