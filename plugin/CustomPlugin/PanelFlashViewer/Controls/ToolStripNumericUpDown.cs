using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PanelFlashViewer
{
    class ToolStripNumericUpDown : ToolStripControlHost
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ToolStripNumericUpDown() : base(new MyNumericUpDown())
        {
            this.Control.ImeMode = ImeMode.Off;
        }

        /// <summary>
        /// ホストしているNumericUpDownコントロール
        /// </summary>
        public NumericUpDown NumericUpDown
        {
            get
            {
                return (MyNumericUpDown)Control;
            }
        }

        /// <summary>
        /// 値の設定と取得
        /// </summary>
        public decimal Value
        {
            get
            {
                return NumericUpDown.Value;
            }
            set
            {
                NumericUpDown.Value = value;
            }
        }

        public int Width
        {
            get
            {
                return NumericUpDown.Width;
            }
            set
            {
                NumericUpDown.Width = value;
            }
        }

        public HorizontalAlignment TextAlign
        {
            get
            {
                return NumericUpDown.TextAlign;
            }
            set
            {
                NumericUpDown.TextAlign = value;
            }
        }

        public decimal Maximum
        {
            get
            {
                return NumericUpDown.Maximum;
            }
            set
            {
                NumericUpDown.Maximum = value;
            }
        }

        //ホストしているNumericUpDownのイベントをサブスクライブする
        protected override void OnSubscribeControlEvents(Control control)
        {
            base.OnSubscribeControlEvents(control);
            NumericUpDown numControl = (NumericUpDown)control;
            numControl.ValueChanged +=
                new EventHandler(NumericUpDown_OnValueChanged);
        }

        //ホストしているNumericUpDownのイベントをアンサブスクライブする
        protected override void OnUnsubscribeControlEvents(Control control)
        {
            base.OnUnsubscribeControlEvents(control);
            MyNumericUpDown numControl = (MyNumericUpDown)control;
            numControl.ValueChanged -=
                new EventHandler(NumericUpDown_OnValueChanged);
        }

        /// <summary>
        /// 値が変化した
        /// </summary>
        public event EventHandler ValueChanged;

        //ValueChangedイベントを発生
        private void NumericUpDown_OnValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }
    }

    public class MyNumericUpDown : NumericUpDown
    {
        const int WM_PASTE = 0x302;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_PASTE)
            {
                IDataObject iData = Clipboard.GetDataObject();
                //文字列がクリップボードにあるか
                if (iData != null && iData.GetDataPresent(DataFormats.Text))
                {
                    string clipStr = (string)iData.GetData(DataFormats.Text);
                    //クリップボードの文字列が数字か調べる
                    if (!System.Text.RegularExpressions.Regex.IsMatch(
                        clipStr,
                        @"^[0-9]+$"))
                        return;
                }
            }

            base.WndProc(ref m);
        }
    }

}
