using System;
using System.ComponentModel;
using PluginCore;
using System.IO;
using PluginCore.Helpers;
using PluginCore.Utilities;
using PluginCore.Managers;
using System.Windows.Forms;
using System.Diagnostics;
using ProjectManager.Projects;
using ProjectManager.Controls.TreeView;
using System.Text.RegularExpressions;
using ProjectManager;
using FDjpPlugin.Properties;

namespace FDjpPlugin
{
    public class PluginMain : IPlugin
    {
        private string pluginAuth = "bkzen";
        private string pluginDesc = "FlashDevelop.jpプラグイン";
        private string pluginGuid = "4308cb28-d1d1-4ac5-aaee-ebd7dc6fa4da";
        private string pluginHelp = "";
        private string pluginName = "FDjpPlugin";
        private Settings settingObj = null;
        private string settingFilename = "";
        private ScintillaNet.ScintillaControl sci = null;
        private Regex alwaysCompileReg = new Regex("^.*\\\\(?<filename>.*)$");
        private ToolStripMenuItem fdjpMenu = null;
        private ToolStripMenuItem nextItem = null;
        private ToolStripMenuItem prevItem = null;
        private ToolStripMenuItem alwaysItem = null;


        #region IPlugin メンバ

        public string Author
        {
            get { return this.pluginAuth; }
        }

        public string Description
        {
            get { return this.pluginDesc; }
        }

        public string Guid
        {
            get { return this.pluginGuid; }
        }

        public string Help
        {
            get { return this.pluginHelp; }
        }

        public string Name
        {
            get { return this.pluginName; }
        }

        public object Settings
        {
            get { return this.settingObj; }
        }

        #endregion

        #region IEventHandler メンバ

        public void Dispose()
        {
            SaveSetting();
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            try
            {
                switch (e.Type)
                {
                    case EventType.FileSwitch:
                        sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    break;
                    case EventType.ApplySettings:
                        AddKeyEventHandler();
                    break;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            CreateMenu();
            AddEventHandlers();
        }

        #endregion

        private void InitBasics()
        {
            string dataPath = Path.Combine(PathHelper.DataDir, Name);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        private void LoadSettings()
        {
            this.settingObj = new Settings();
            if (File.Exists(this.settingFilename))
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObj);
                this.settingObj = (Settings)obj;
            }
            else
            {
                SaveSetting();
            }
        }

        private void SaveSetting()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObj);
        }

        /// <summary>
        /// メニューアイテムを作る。
        /// </summary>
        private void CreateMenu()
        {
            MenuStrip mainMenu = PluginBase.MainForm.MenuStrip;
            this.fdjpMenu = new ToolStripMenuItem();
            this.fdjpMenu.Text = Resources.LABEL_MENU;
            this.nextItem = new ToolStripMenuItem();
            this.nextItem.Text = Resources.LABEL_NEXT_WORD;
            this.nextItem.Click += new EventHandler(this.nextWord);
            this.prevItem = new ToolStripMenuItem();
            this.prevItem.Text = Resources.LABEL_PREV_WORD;
            this.prevItem.Click += new EventHandler(this.prevWord);
            this.alwaysItem = new ToolStripMenuItem();
            this.alwaysItem.Text = Resources.LABEL_ALWAYS_COMPILE;
            this.alwaysItem.Click += new EventHandler(this.alwaysCompile);

            this.fdjpMenu.DropDownItems.Add(this.nextItem);
            this.fdjpMenu.DropDownItems.Add(this.prevItem);
            this.fdjpMenu.DropDownItems.Add(this.alwaysItem);
            mainMenu.Items.Add(this.fdjpMenu);
        }

        private void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.ApplySettings);
            AddKeyEventHandler();
        }

        private void AddKeyEventHandler()
        {
            Keys alwaysKey = settingObj.AlwaysCompileKey;
            Keys nextKey   = settingObj.NextWordKey;
            Keys prevKey   = settingObj.PrevWordKey;

            if (!PluginBase.MainForm.IgnoredKeys.Contains(alwaysKey)) { PluginBase.MainForm.IgnoredKeys.Add(alwaysKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(nextKey)) { PluginBase.MainForm.IgnoredKeys.Add(nextKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(prevKey)) { PluginBase.MainForm.IgnoredKeys.Add(prevKey); }

            this.alwaysItem.ShortcutKeys = alwaysKey;
            this.nextItem.ShortcutKeys = nextKey;
            this.prevItem.ShortcutKeys = prevKey;
        }

        private void nextWord(Object sender, EventArgs e)
        {
            string cword = "";
            string nword = "";
            if (sci.CurrentPos == sci.Length) return;
            cword = sci.GetWordFromPosition(sci.CurrentPos);
            sci.WordRightEnd();
            nword = sci.GetWordFromPosition(sci.CurrentPos);
            if (cword != nword && nword != null)
            {
                cword = sci.GetWordFromPosition(sci.CurrentPos);
                sci.WordRightEnd();
                nword = sci.GetWordFromPosition(sci.CurrentPos);
                if (cword != nword) sci.WordLeftEnd();
            }
            else
            {
                while ((nword != cword && !(cword == null && nword != null)) || (cword == null && nword == null) && sci.Length > sci.CurrentPos)
                {
                    cword = sci.GetWordFromPosition(sci.CurrentPos);
                    sci.WordRightEnd();
                    nword = sci.GetWordFromPosition(sci.CurrentPos);
                }
            }
            if (settingObj.WordSelect && sci.GetWordFromPosition(sci.SelectionStart) != null)
            {
                nword = sci.GetWordFromPosition(sci.SelectionStart);
                sci.SelectText(nword, sci.SelectionStart - nword.Length);
            }
        }

        private void prevWord(Object sender, EventArgs e)
        {
            string cword = "";
            string nword = "";
            if (sci.CurrentPos == 0) return ;
            do
            {
                cword = sci.GetWordFromPosition(sci.CurrentPos);
                sci.WordLeftEnd();
                nword = sci.GetWordFromPosition(sci.CurrentPos);
            }
            while ((nword == null || cword == nword) && sci.CurrentPos > 0);
            if (settingObj.WordSelect && sci.GetWordFromPosition(sci.SelectionStart) != null)
            {
                nword = sci.GetWordFromPosition(sci.SelectionStart);
                sci.SelectText(nword, sci.SelectionStart - nword.Length);
            }
            return ;
        }

        private void alwaysCompile(Object sender, EventArgs e)
        {
            Project project = (Project)PluginBase.CurrentProject;
            ITabbedDocument nowDoc = PluginBase.MainForm.CurrentDocument;
            string path = nowDoc.FileName;
            if (path.StartsWith(project.Directory))
            {
                if (path.EndsWith(".as") || path.EndsWith(".mxml"))
                {
                    project.SetCompileTarget(path, true);
                    if (project.MaxTargetsCount > 0)
                    {
                        while (project.CompileTargets.Count > project.MaxTargetsCount)
                        {
                            string relPath = project.CompileTargets[0];
                            string path2 = project.GetAbsolutePath(relPath);
                            project.SetCompileTarget(path2, false);
                        }
                    }
                    project.Save();
                    ProjectTreeView.Instance.RefreshTree();
                    string str = path;
                    if (alwaysCompileReg.IsMatch(str))
                    {
                        Match match = alwaysCompileReg.Match(str);
                        str = match.Groups["filename"].Value;
                    }
                    if (settingObj.AlwaysCompileAfterCompile)
                    {
                        PluginBase.MainForm.CallCommand("PluginCommand", ProjectManagerCommands.TestMovie);
                    }
                    TraceManager.Add("Always Compile: " + str, -2);
                }
                else 
                {
                    ErrorManager.ShowWarning(path + "は AS, MXML ファイルではありません。", null);
                }
            }
            else
            {
                ErrorManager.ShowWarning(path + "は、現在の Project のファイルではありません。", null);
            }
        }
    }
}
