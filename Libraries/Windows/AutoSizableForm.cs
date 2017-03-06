using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Valutech.Windows.Forms
{
    public class AutoSizableForm : Form
    {
        protected Size minSize = new Size(100,100);

        public AutoSizableForm()
        {
            this.Resize += new System.EventHandler(this.onResize);
        }

        private void onLoad(object sender,EventArgs args) {

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

        public virtual void centerInScreen()
        {
            Location = new Point((Screen.PrimaryScreen.WorkingArea.Size.Width - Size.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Size.Height - Size.Height) / 2);
        }
    }
}
