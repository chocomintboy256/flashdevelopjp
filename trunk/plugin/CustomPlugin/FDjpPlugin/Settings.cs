using System;
using System.Windows.Forms;
using System.ComponentModel;

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

        private Keys nextWordKey = NEXT_WORD_KEY;
        private Keys prevWordKey = PREV_WORD_KEY;
        private Keys nextLineKey = NEXT_LINE_KEY;
        private Keys prevLineKey = PREV_LINE_KEY;
        private bool wordSelect = WORD_SELECT;
        private Keys alwaysCompileKey = ALWAYS_COMP_KEY;
        private bool alwaysCompileAfterCompile = ALWAYS_COMP_AFTER_COMP;
        private Keys foldAllCommentsKey = FOLD_ALL_COMMENTS_KEY;
        private Keys expandAllCommentsKey = EXPAND_ALL_COMMENTS_KEY;
        private string version = "";


        [Category("ワード移動")]
        [Description("次のワードに移動するショートカットの設定。"), DefaultValue(NEXT_WORD_KEY)]
        public Keys NextWordKey
        {
            get { return this.nextWordKey; }
            set { this.nextWordKey = value; }
        }

        [Category("ワード移動")]
        [Description("前のワードに移動するショートカットの設定。"), DefaultValue(PREV_WORD_KEY)]
        public Keys PrevWordKey
        {
            get { return this.prevWordKey; }
            set { this.prevWordKey = value; }
        }

        [Category("ワード移動")]
        [Description("移動した際にワードをセレクトする設定。"), DefaultValue(WORD_SELECT)]
        public bool WordSelect
        {
            get { return this.wordSelect; }
            set { this.wordSelect = value; }
        }

        [Category("行移動")]
        [Description("下に行を移動する。"), DefaultValue(NEXT_LINE_KEY)]
        public Keys NextLineKey
        {
            get { return this.nextLineKey; }
            set { this.nextLineKey = value; }
        }

        [Category("行移動")]
        [Description("下に行を移動する。"), DefaultValue(PREV_LINE_KEY)]
        public Keys PrevLineKey
        {
            get { return this.prevLineKey; }
            set { this.prevLineKey = value; }
        }

        [Category("Always Compile")]
        [Description("Always Compile を現在のドキュメントに設定するショートカット"), DefaultValue(ALWAYS_COMP_KEY)]
        public Keys AlwaysCompileKey
        {
            get { return this.alwaysCompileKey; }
            set { this.alwaysCompileKey = value; }
        }

        [Category("Always Compile")]
        [Description("切り替えた後コンパイルする。"), DefaultValue(ALWAYS_COMP_AFTER_COMP)]
        public bool AlwaysCompileAfterCompile
        {
            get { return this.alwaysCompileAfterCompile; }
            set { this.alwaysCompileAfterCompile = value; }
        }

        [Category("折りたたみ")]
        [Description("すべての複数コメント行を折りたたむ。"), DefaultValue(FOLD_ALL_COMMENTS_KEY)]
        public Keys FoldAllCommentsKey
        {
            get { return this.foldAllCommentsKey; }
            set { this.foldAllCommentsKey = value; }
        }

        [Category("折りたたみ")]
        [Description("すべての複数コメント行を展開する。"), DefaultValue(EXPAND_ALL_COMMENTS_KEY)]
        public Keys ExpandAllCommentsKey
        {
            get { return this.expandAllCommentsKey; }
            set { this.expandAllCommentsKey = value; }
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
                version = v;
            }
        }
    }
}
