using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace FDjpPlugin
{
    [Serializable]
    class Settings
    {
        private Keys nextWordKey = Keys.Control | Keys.OemPeriod;
        private Keys prevWordKey = Keys.Control | Keys.Oemcomma;
        private bool wordSelect = false;
        private Keys alwaysCompileKey = Keys.Alt | Keys.A;
        private bool alwaysCompileAfterCompile = false;


        [Category("ワード移動")]
        [Description("次のワードに移動するショートカットの設定。"), DefaultValue(Keys.Control | Keys.OemPeriod)]
        public Keys NextWordKey
        {
            get { return this.nextWordKey; }
            set { this.nextWordKey = value; }
        }

        [Category("ワード移動")]
        [Description("前のワードに移動するショートカットの設定。"), DefaultValue(Keys.Control | Keys.Oemcomma)]
        public Keys PrevWordKey
        {
            get { return this.prevWordKey; }
            set { this.prevWordKey = value; }
        }

        [Category("ワード移動")]
        [Description("移動した際にワードをセレクトする設定"), DefaultValue(false)]
        public bool WordSelect
        {
            get { return this.wordSelect; }
            set { this.wordSelect = value; }
        }


        [Category("Always Compile")]
        [Description("Always Compile を現在のドキュメントに設定するショートカット"), DefaultValue(Keys.Alt | Keys.A)]
        public Keys AlwaysCompileKey
        {
            get { return this.alwaysCompileKey; }
            set { this.alwaysCompileKey = value; }
        }

        [Category("Always Compile")]
        [Description("切り替えた後コンパイルする。"), DefaultValue(false)]
        public bool AlwaysCompileAfterCompile
        {
            get { return this.alwaysCompileAfterCompile; }
            set { this.alwaysCompileAfterCompile = value; }
        }
    }
}
