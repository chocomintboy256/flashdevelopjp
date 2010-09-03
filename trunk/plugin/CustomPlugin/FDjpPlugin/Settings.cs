using System;
using System.Windows.Forms;
using System.ComponentModel;
using FDjpPlugin.Resources;

namespace FDjpPlugin
{
    [Serializable]
    class Settings
    {
        public const Keys NEXT_WORD_KEY = Keys.Control | Keys.OemPeriod;
        public const Keys PREV_WORD_KEY = Keys.Control | Keys.Oemcomma;
        public const Keys NEXT_LINE_KEY = Keys.Alt | Keys.Down;
        public const Keys PREV_LINE_KEY = Keys.Alt | Keys.Up;
        public const bool WORD_SELECT = true;
        public const Keys ALWAYS_COMP_KEY = Keys.Alt | Keys.A;
        public const bool ALWAYS_COMP_AFTER_COMP = false;
        public const Keys FOLD_ALL_COMMENTS_KEY = Keys.Control | Keys.Alt | Keys.F;
        public const Keys EXPAND_ALL_COMMENTS_KEY = Keys.Control | Keys.Alt | Keys.E;
        public const bool FOLD_COMMENTS_TOGGLE = false;
        public const Keys ALIGN_ASSIGNMENTS_KEY = Keys.Control | Keys.Alt | Keys.Oem6;
        public const Keys CALC_SELECTION_KEY = Keys.Alt | Keys.C;
        public const bool HIDE_MENU = false;
        // public const Keys SEARCH_NEXT_KEY = 
        // public const Keys SEARCH_PREV_KEY = 

        private Keys nextWordKey = NEXT_WORD_KEY;
        private Keys prevWordKey = PREV_WORD_KEY;
        private Keys nextLineKey = NEXT_LINE_KEY;
        private Keys prevLineKey = PREV_LINE_KEY;
        private bool wordSelect = WORD_SELECT;
        private Keys alwaysCompileKey = ALWAYS_COMP_KEY;
        private bool alwaysCompileAfterCompile = ALWAYS_COMP_AFTER_COMP;
        private Keys foldAllCommentsKey = FOLD_ALL_COMMENTS_KEY;
        private Keys expandAllCommentsKey = EXPAND_ALL_COMMENTS_KEY;
        private bool foldCommentsToggle = FOLD_COMMENTS_TOGGLE;
        private Keys alignAssignmentsKey = ALIGN_ASSIGNMENTS_KEY;
        private Keys calcSelectionKey = CALC_SELECTION_KEY;
        private bool hideMenu = HIDE_MENU;
        private Keys searchNextKey;// = SEARCH_NEXT_KEY;
        private Keys searchPrevKey;// = SEARCH_PREV_KEY;
        private string version = "";

        [DisplayName("Next Word Shortcut")]
        [LocalizedCategory("CATEGORY_MOVE_WORD")]
        [LocalizedDescription("DESCRIPTION_NEXT_WORD"), DefaultValue(NEXT_WORD_KEY)]
        public Keys NextWordKey
        {
            get { return this.nextWordKey; }
            set { this.nextWordKey = value; }
        }

        [DisplayName("Prev Word Shortcut")]
        [LocalizedCategory("CATEGORY_MOVE_WORD")]
        [LocalizedDescription("DESCRIPTION_PREV_WORD"), DefaultValue(PREV_WORD_KEY)]
        public Keys PrevWordKey
        {
            get { return this.prevWordKey; }
            set { this.prevWordKey = value; }
        }

        [DisplayName("Enable Select Word")]
        [LocalizedCategory("CATEGORY_MOVE_WORD")]
        [LocalizedDescription("DESCRIPTION_WORD_SELECT"), DefaultValue(WORD_SELECT)]
        public bool WordSelect
        {
            get { return this.wordSelect; }
            set { this.wordSelect = value; }
        }

        [DisplayName("Move Line Down Shortcut")]
        [LocalizedCategory("CATEGORY_MOVE_LINE")]
        [LocalizedDescription("DESCRIPTION_MOVE_LINE_DOWN"), DefaultValue(NEXT_LINE_KEY)]
        public Keys NextLineKey
        {
            get { return this.nextLineKey; }
            set { this.nextLineKey = value; }
        }

        [DisplayName("Move Line Up Shortcut")]
        [LocalizedCategory("CATEGORY_MOVE_LINE")]
        [LocalizedDescription("DESCRIPTION_MOVE_LINE_UP"), DefaultValue(PREV_LINE_KEY)]
        public Keys PrevLineKey
        {
            get { return this.prevLineKey; }
            set { this.prevLineKey = value; }
        }

        [DisplayName("Always Compile Shortcut")]
        [LocalizedCategory("CATEGORY_ALWAYS_COMPILE")]
        [LocalizedDescription("DESCRIPTION_ALWAYS_COMPILE"), DefaultValue(ALWAYS_COMP_KEY)]
        public Keys AlwaysCompileKey
        {
            get { return this.alwaysCompileKey; }
            set { this.alwaysCompileKey = value; }
        }

        [DisplayName("Enables Compile On Set Always Compile")]
        [LocalizedCategory("CATEGORY_ALWAYS_COMPILE")]
        [LocalizedDescription("DESCRIPTION_ALWAYS_COMP_AFTER_COMP"), DefaultValue(ALWAYS_COMP_AFTER_COMP)]
        public bool AlwaysCompileAfterCompile
        {
            get { return this.alwaysCompileAfterCompile; }
            set { this.alwaysCompileAfterCompile = value; }
        }

        [DisplayName("Fold All Commnets Shortcut")]
        [LocalizedCategory("CATEGORY_FOLDING")]
        [LocalizedDescription("DESCRIPTION_FOLD_ALL_COMMENTS"), DefaultValue(FOLD_ALL_COMMENTS_KEY)]
        public Keys FoldAllCommentsKey
        {
            get { return this.foldAllCommentsKey; }
            set { this.foldAllCommentsKey = value; }
        }

        [DisplayName("Expand All Comments Shortcut")]
        [LocalizedCategory("CATEGORY_FOLDING")]
        [LocalizedDescription("DESCRIPTION_EXPAND_ALL_COMMENTS"), DefaultValue(EXPAND_ALL_COMMENTS_KEY)]
        public Keys ExpandAllCommentsKey
        {
            get { return this.expandAllCommentsKey; }
            set { this.expandAllCommentsKey = value; }
        }

        [DisplayName("Enables Toggle Fold and Expand")]
        [LocalizedCategory("CATEGORY_FOLDING")]
        [LocalizedDescription("DESCRIPTION_TOGGLE_FOLD_EXPAND"), DefaultValue(FOLD_COMMENTS_TOGGLE)]
        public bool FoldCommentsToggle
        {
            get { return this.foldCommentsToggle; }
            set { this.foldCommentsToggle = value; }
        }

        [DisplayName("Align Assignments Shortcut")]
        [LocalizedCategory("CATEGORY_ALIGN_ASSIGMENTS")]
        [LocalizedDescription("DESCRIPTION_ALIGN_ASSIGNMENTS"), DefaultValue(ALIGN_ASSIGNMENTS_KEY)]
        public Keys AlignAssignmentsKey
        {
            get { return this.alignAssignmentsKey; }
            set { this.alignAssignmentsKey = value; }
        }

        [DisplayName("Calc Selection Shortcut")]
        [LocalizedCategory("CATEGORY_CALCULATE")]
        [LocalizedDescription("DESCRIPTION_CALC_SELECTION"), DefaultValue(CALC_SELECTION_KEY)]
        public Keys CalcSelectionKey
        {
            get { return this.calcSelectionKey; }
            set { this.calcSelectionKey = value; }
        }

        [DisplayName("Hide Menu")]
        [LocalizedDescription("DESCRIPTION_HIDE_MENU"), DefaultValue(false)]
        public bool HideMenu
        {
            get { return this.hideMenu; }
            set { this.hideMenu = value; }
        }

        [DisplayName("Search Next Shortcut")]
        [LocalizedCategory("CATEGORY_SEARCH")]
        [LocalizedDescription("DESCRIPTION_SEARCH_NEXT")]
        public Keys SearchNextKey
        {
            get { return this.searchNextKey; }
            set { this.searchNextKey = value; }
        }

        [DisplayName("Search Prev Shortcut")]
        [LocalizedCategory("CATEGORY_SEARCH")]
        [LocalizedDescription("DESCRIPTION_SEARCH_PREV")]
        public Keys SearchPrevKey
        {
            get { return this.searchPrevKey; }
            set { this.searchPrevKey = value; }
        }

        [Browsable(false)]
        public string Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        public void checkVersion(string v)
        {
            if (version != v)
            {
                if (nextLineKey == Keys.None) nextLineKey = NEXT_LINE_KEY;
                if (nextWordKey == Keys.None) nextWordKey = NEXT_WORD_KEY;
                if (prevLineKey == Keys.None) prevLineKey = PREV_LINE_KEY;
                if (prevWordKey == Keys.None) prevWordKey = PREV_WORD_KEY;
                if (alwaysCompileKey == Keys.None) alwaysCompileKey = ALWAYS_COMP_KEY;
                if (foldAllCommentsKey == Keys.None) foldAllCommentsKey = FOLD_ALL_COMMENTS_KEY;
                if (expandAllCommentsKey == Keys.None) expandAllCommentsKey = EXPAND_ALL_COMMENTS_KEY;
                if (alignAssignmentsKey == Keys.None) alignAssignmentsKey = ALIGN_ASSIGNMENTS_KEY;
                
                version = v;
            }
        }
    }
}
