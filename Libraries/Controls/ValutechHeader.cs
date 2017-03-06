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
    public class ValutechHeader : PictureBox
    {
        public ValutechHeader()
            : base()
        {
            this.BackColor = Color.White;
            this.Dock = DockStyle.Top;
            this.Height = 52;
            this.Padding = new Padding(8,8,8,8);
        }
    }
}
