using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FDjpPlugin.Properties;
using FlashDevelop.Managers;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager;
using ProjectManager.Controls.TreeView;
using ProjectManager.Projects;
using ScintillaNet;

namespace FDjpPlugin
{
    public class PluginMain : IPlugin
    {
        private string pluginAuth = "bkzen";
        private string pluginDesc = "FlashDevelop.jpプラグイン";
        private string pluginGuid = "4308cb28-d1d1-4ac5-aaee-ebd7dc6fa4da";
        private string pluginVer = "1.0.0.7";
        private string pluginHelp = "http://code.google.com/p/flashdevelopjp/";
        private string pluginName = "FDjpPlugin";
        private Settings settingObj = null;
        private string settingFilename = "";
        private ScintillaNet.ScintillaControl sci = null;
        private Regex alwaysCompileReg = new Regex("^.*\\\\(?<filename>.*)$");
        private ToolStripMenuItem fdjpMenu = null;
        private ToolStripMenuItem nextItem = null;
        private ToolStripMenuItem prevItem = null;
        private ToolStripMenuItem alwaysItem = null;
        private ToolStripMenuItem nextLineItem = null;
        private ToolStripMenuItem prevLineItem = null;
        private ToolStripMenuItem foldAllCommentsItem = null;
        private ToolStripMenuItem expandAllCommentsItem = null;


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
                this.settingObj.checkVersion(pluginVer);
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

            this.nextLineItem = new ToolStripMenuItem();
            this.nextLineItem.Text = Resources.LABEL_NEXT_LINE;
            this.nextLineItem.Click += new EventHandler(this.nextLine);
            this.prevLineItem = new ToolStripMenuItem();
            this.prevLineItem.Text = Resources.LABEL_PREV_LINE;
            this.prevLineItem.Click += new EventHandler(this.prevLine);
            this.foldAllCommentsItem = new ToolStripMenuItem();
            this.foldAllCommentsItem.Text = Resources.LABEL_FOLD_ALL_COMMENTS;
            this.foldAllCommentsItem.Click += new EventHandler(this.foldAllComments);
            this.expandAllCommentsItem = new ToolStripMenuItem();
            this.expandAllCommentsItem.Text = Resources.LABEL_EXPAND_ALL_COMMENTS;
            this.expandAllCommentsItem.Click += new EventHandler(this.expandAllComments);

            this.fdjpMenu.DropDownItems.Add(this.nextItem);
            this.fdjpMenu.DropDownItems.Add(this.prevItem);
            this.fdjpMenu.DropDownItems.Add(this.alwaysItem);
            this.fdjpMenu.DropDownItems.Add(this.nextLineItem);
            this.fdjpMenu.DropDownItems.Add(this.prevLineItem);
            this.fdjpMenu.DropDownItems.Add(this.foldAllCommentsItem);
            this.fdjpMenu.DropDownItems.Add(this.expandAllCommentsItem);
            mainMenu.Items.Add(this.fdjpMenu);
        }

        private void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.ApplySettings);
            AddKeyEventHandler();
        }

        private void AddKeyEventHandler()
        {
            Keys alwaysKey            = settingObj.AlwaysCompileKey;
            Keys nextKey              = settingObj.NextWordKey;
            Keys prevKey              = settingObj.PrevWordKey;
            Keys nextLineKey          = settingObj.NextLineKey;
            Keys prevLineKey          = settingObj.PrevLineKey;
            Keys foldAllCommentsKey   = settingObj.FoldAllCommentsKey;
            Keys expandAllCommentsKey = settingObj.ExpandAllCommentsKey;

            if (!PluginBase.MainForm.IgnoredKeys.Contains(alwaysKey))            { PluginBase.MainForm.IgnoredKeys.Add(alwaysKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(nextKey))              { PluginBase.MainForm.IgnoredKeys.Add(nextKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(prevKey))              { PluginBase.MainForm.IgnoredKeys.Add(prevKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(nextLineKey))          { PluginBase.MainForm.IgnoredKeys.Add(nextLineKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(prevLineKey))          { PluginBase.MainForm.IgnoredKeys.Add(prevLineKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(foldAllCommentsKey))   { PluginBase.MainForm.IgnoredKeys.Add(foldAllCommentsKey); }
            if (!PluginBase.MainForm.IgnoredKeys.Contains(expandAllCommentsKey)) { PluginBase.MainForm.IgnoredKeys.Add(expandAllCommentsKey); }

            this.alwaysItem.ShortcutKeys            = alwaysKey;
            this.nextItem.ShortcutKeys              = nextKey;
            this.prevItem.ShortcutKeys              = prevKey;
            this.nextLineItem.ShortcutKeys          = nextLineKey;
            this.prevLineItem.ShortcutKeys          = prevLineKey;
            this.foldAllCommentsItem.ShortcutKeys   = foldAllCommentsKey;
            this.expandAllCommentsItem.ShortcutKeys = expandAllCommentsKey;
        }

        private void nextWord(Object sender, EventArgs e)
        {
            string cword = "";
            string nword = "";
            if (sci.CurrentPos >= sci.Length)
            {
                sci.CurrentPos = sci.Length;
                return;
            }
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
                int epos = sci.CurrentPos;
                sci.WordLeft();
                sci.SelectionEnd = epos;
            }
        }

        private void prevWord(Object sender, EventArgs e)
        {
            string cword = "";
            string nword = sci.GetWordFromPosition(sci.SelectionStart);
            if ((nword != null) && sci.CurrentPos - nword.Length < 1)
            {
                sci.CurrentPos = 0;
                return;
            }
            do
            {
                cword = sci.GetWordFromPosition(sci.CurrentPos);
                sci.WordLeftEnd();
                nword = sci.GetWordFromPosition(sci.CurrentPos);
            }
            while ((nword == null || cword == nword) && sci.CurrentPos > 0);
            if (settingObj.WordSelect && sci.GetWordFromPosition(sci.SelectionStart) != null)
            {
                int epos = sci.CurrentPos;
                sci.WordLeft();
                sci.SelectionEnd = epos;
            }
            return ;
        }

        private void nextLine(Object sender, EventArgs e)
        {
            int start = sci.SelectionStart < sci.SelectionEnd ? sci.SelectionStart : sci.SelectionEnd;
            int end = sci.SelectionStart > sci.SelectionEnd ? sci.SelectionStart : sci.SelectionEnd;
            int len = sci.LineFromPosition(end) - sci.LineFromPosition(start);
            sci.BeginUndoAction();
            sci.SelectionStart = sci.PositionFromLine(sci.LineFromPosition(start));
            sci.SelectionEnd = sci.PositionFromLine(sci.LineFromPosition(end) + 1);
            string selectStr = sci.SelText;
            sci.Clear();
            sci.LineDown();
            sci.InsertText(sci.PositionFromLine(sci.LineFromPosition(sci.CurrentPos)), selectStr);
            sci.SelectionStart = sci.PositionFromLine(start = sci.LineFromPosition(sci.CurrentPos));
            sci.SelectionEnd = sci.LineEndPosition(start + len);
            sci.EndUndoAction();
        }

        private void prevLine(Object sender, EventArgs e)
        {
            int start = sci.SelectionStart < sci.SelectionEnd ? sci.SelectionStart : sci.SelectionEnd;
            int end = sci.SelectionStart > sci.SelectionEnd ? sci.SelectionStart : sci.SelectionEnd;
            int len = sci.LineFromPosition(end) - sci.LineFromPosition(start);
            sci.BeginUndoAction();
            sci.SelectionStart = sci.PositionFromLine(sci.LineFromPosition(start));
            sci.SelectionEnd = sci.PositionFromLine(sci.LineFromPosition(end) + 1);
            string selectStr = sci.SelText;
            sci.Clear();
            sci.LineUp();
            sci.InsertText(sci.PositionFromLine(sci.LineFromPosition(sci.CurrentPos)), selectStr);
            sci.SelectionStart = sci.PositionFromLine(start = sci.LineFromPosition(sci.CurrentPos));
            sci.SelectionEnd = sci.LineEndPosition(start + len);
            sci.EndUndoAction();
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


        private void foldAllComments(Object sender, EventArgs e)
        {
            foldAllComments(true);
        }

        private void expandAllComments(Object sender, EventArgs e)
        {
            foldAllComments(false);
        }

        private void foldAllComments(Boolean fold)
        {
            // スクロール位置を保持
            Int32 scrollTop = sci.FirstVisibleLine;

            // コメントの開始を検索。検索だと重いので他の方法がいいんですけどわからなくて・・・
            //String commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage); //これ使いたいんですけどプロテクトがかかってる・・・
            String commentStart = "/*"; 
            List<SearchMatch> matches = this.searchString(sci, commentStart);

            if (matches != null && matches.Count != 0)
            {
                Int32 lexer = sci.Lexer;
                Int32 expNum = 0;
                foreach (SearchMatch match in matches)
                {
                    Int32 pos = sci.MBSafePosition(match.Index);
                    Int32 line = sci.LineFromPosition(pos);
                    Int32 foldParentLine = sci.FoldParent(line + 1);
                    if (foldParentLine == line)
                    {
                        Boolean isExpanded = sci.FoldExpanded(foldParentLine);
                        if (fold)
                        {
                            if (isExpanded)
                            {
                                sci.ToggleFold(line);
                            }
                            else
                            {
                                expNum++;
                            }
                        }
                        else
                        {
                            if (!isExpanded)
                            {
                                sci.ToggleFold(line);
                            }
                            else
                            {
                                expNum++;
                            }
                        }
                    }
                }
				
				// 折りたたみ・展開するものがなかったら逆の動きをする
                if (expNum == matches.Count && settingObj.FoldCommentsToggle)
                {
                    foldAllComments(!fold);
                    return;
                }
            }

            // スクロール位置を元に戻す
            if (scrollTop != sci.FirstVisibleLine)
            {
                sci.LineScroll(0, scrollTop - sci.FirstVisibleLine);
            }
        }

        // 文字列検索
        private List<SearchMatch> searchString(ScintillaControl sci, String text)
        {
            FRSearch search = new FRSearch(text);
            return search.Matches(sci.Text);
        }
    }
}
