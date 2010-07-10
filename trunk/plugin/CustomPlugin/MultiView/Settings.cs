using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace MultiView
{
    [Serializable]
    public class Settings
    {
        private DockStyle dockSryle = DockStyle.Left;
        private Keys cloneViewShortcut;// = Keys.Control | Keys.F1;
        private bool disableSync = false;
        
        [DisplayName("Cloned View Docking Position")]
        [Description("Cloned View Docking Position."), DefaultValue(DockStyle.Left)]
        public DockStyle DockStyle 
        {
            get { return this.dockSryle; }
            set { this.dockSryle = value; }
        }

        [DisplayName("Disable Auto Sync")]
        [Description("Disable Auto Sync."), DefaultValue(false)]
        public bool DisableSync
        {
            get { return this.disableSync; }
            set { this.disableSync = value; }
        }

        [DisplayName("Clone View Shortcut")]
        [Description("Shortcut of Clone View.")]
        public Keys CloneViewShortcut
        {
            get { return this.cloneViewShortcut; }
            set { this.cloneViewShortcut = value; }
        }

    }
}
