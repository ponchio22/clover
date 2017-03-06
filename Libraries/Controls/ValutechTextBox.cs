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
    public class ValutechTextBox : TextBox
    {

        public ValutechTextBox()
            : base()
        {
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.BackColor = Color.White;
            this.Width = 120;            
        }
    }
}
