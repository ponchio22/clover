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
    public class ValutechButton : Button
    {
        public ValutechButton()
            : base()
        {
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackColor = Color.FromArgb(100, 152, 0);
            this.ForeColor = Color.White;
            this.Height = 25;
            this.Width = 120;
            this.Text = "Button";
        }
    }
}
