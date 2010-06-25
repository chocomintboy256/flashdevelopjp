using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace AutoBuildPlugin
{
    [Serializable]
    public class Settings
    {
        private ViewStyle displayStyle = ViewStyle.Popup;
        private Keys toggleAutoBuildShortcut;
        
        /// <summary> 
        /// Get and sets the sampleShortcut
        /// </summary>
        [DisplayName("Toggle Auto Build Shortcut")]
        //[LocalizedCategory("ResultsPanel.Category.Shortcuts")]
        //[LocalizedDescription("ResultsPanel.Description.PreviousError"), DefaultValue(DEFAULT_PREVIOUSERROR)]
        [Description("Shortcut of the Toggle Auto Build.")]
        public Keys ToggleAutoBuildShortcut
        {
            get { return this.toggleAutoBuildShortcut; }
            set { this.toggleAutoBuildShortcut = value; }
        }
        
        /// <summary> 
        /// Get and sets the displayStyle
        /// </summary>
        [DisplayName("Movie Display Style"), DefaultValue(ViewStyle.Popup)]
        //[LocalizedDescription("FlashViewer.Description.DisplayStyle"), DefaultValue(ViewStyle.External)]
        public ViewStyle DisplayStyle
        {
            get { return this.displayStyle; }
            set { this.displayStyle = value; }
        }
        
    }

    public enum ViewStyle
    {
        Popup,
        External,
        Document,
        Panel,
        None
    }

}
