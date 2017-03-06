using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Valutech.Controls
{
    public class ValutechTextBoxContainer : Panel
    {
        private TextBox textBoxControl = null;

        public ValutechTextBoxContainer()
            : base()
        {
            this.BackColor = Color.White;
            this.Width = 120;
            this.Height = 25;
            this.ControlAdded += new ControlEventHandler(ValutechPanel_ControlAdded);
            this.SizeChanged += new EventHandler(ValutechPanel_SizeChanged);
            this.Padding = new Padding(6,2,6,2);
            SetSize();
        }

        void ValutechPanel_SizeChanged(object sender, EventArgs e)
        {
            SetSize();
        }

        void SetSize()
        {
            if (textBoxControl != null)
            {
                this.textBoxControl.Width = this.Width - this.Padding.Horizontal;
                this.textBoxControl.Location = new Point(this.Padding.Left, (this.Height / 2) - (textBoxControl.Height / 2));
            }
        }

        void ValutechPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control.GetType() == typeof(TextBox) || e.Control.GetType() == typeof(ValutechTextBox))
            {
                textBoxControl = (TextBox)e.Control;
                SetSize();           
            }
        }
    }
}
