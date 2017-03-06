using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Valutech.Windows.Forms
{
    public class AutoSizableControl : UserControl
    {
        protected Size minSize = new Size(100,25);

        public AutoSizableControl()
        {
            this.Resize += new System.EventHandler(this.onResize);
        }

        private void onResize(object sender, EventArgs args)
        {
            setSize(this.Size);
        }

        public virtual void setSize(Size size)
        {
            if (size.Width < minSize.Width) size.Width = minSize.Width;
            if (size.Height < minSize.Height) size.Height = minSize.Height;
            Size = size;            
        }
    }
}
