using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace PanelFlashViewer
{
    [Serializable]
    public class Settings
    {

        private Int32 swfWidth = 500;
        private Int32 swfHegiht = 400;
        private DisplayState displayState = DisplayState.NO_SCALE;

        //private String sampleText = "This is a sample plugin.";
        //private Keys sampleShortcut = Keys.Control | Keys.F1;
        
        /// <summary> 
        /// Get and sets the sampleText
        /// </summary>
        [Description("swf width"), DefaultValue(500)]
        public Int32 SWFWidth
        {
            get { return this.swfWidth; }
            set { this.swfWidth = value; }
        }

        [Description("swf height"), DefaultValue(400)]
        public Int32 SWFHeight
        {
            get { return this.swfHegiht; }
            set { this.swfHegiht = value; }
        }

        //[Description("resize"), DefaultValue(DisplayState.NO_SCALE)]
        public DisplayState DisplayState
        {
            get { return this.displayState; }
            set { this.displayState = value; }
        }

        /*
        /// <summary> 
        /// Get and sets the sampleNumber
        /// </summary>
        [Description("A sample integer setting."), DefaultValue(69)]
        public Int32 SampleNumber
        {
            get { return this.sampleNumber; }
            set { this.sampleNumber = value; }
        }

        /// <summary> 
        /// Get and sets the sampleShortcut
        /// </summary>
        [Description("A sample shortcut setting."), DefaultValue(Keys.Control | Keys.F1)]
        public Keys SampleShortcut
        {
            get { return this.sampleShortcut; }
            set { this.sampleShortcut = value; }
        }
        */
    }

    public enum DisplayState {   
        NO_SCALE,
        EXACT_FIT,
        FREE
    }

}
