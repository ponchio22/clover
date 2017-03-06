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
    public partial class IconControl : UserControl
    {
        private System.Windows.Forms.PictureBox ApplicationIcon;
        private System.Windows.Forms.Label AppName;
        private System.Windows.Forms.Label AppVersion;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public IconControl()
        {
            InitializeComponent();
            ApplicationIcon.Image = Icon.ExtractAssociatedIcon(Application.ExecutablePath).ToBitmap();
            ApplicationIcon.Size = new Size(32, 32);
            AppName.Text = Application.ProductName;
            AppVersion.Text = Application.ProductVersion;
            base.Width = AppName.Width + ApplicationIcon.Width + 10;
            base.Height = 42;
            AppName.Left = this.Width - AppName.Width - 5;
            AppVersion.Left = AppName.Left;
            ApplicationIcon.Left = AppName.Left - ApplicationIcon.Width - 5;
            this.ParentChanged += new EventHandler(IconControl_ParentChanged);
        }

        void IconControl_ParentChanged(object sender, EventArgs e)
        {
            this.Parent.Resize += new EventHandler(Parent_Resize);
            Parent_Resize(this, EventArgs.Empty);
        }

        void Parent_Resize(object sender, EventArgs e)
        {
            this.Location = new Point(this.Parent.Width - this.Width - 10,5);
        }

        public new int Width
        {
            set { }
            get
            {
                return base.Width;
            }
        }

        public new Size Size
        {
            set { }
            get
            {
                return base.Size;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ApplicationIcon = new System.Windows.Forms.PictureBox();
            this.AppName = new System.Windows.Forms.Label();
            this.AppVersion = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ApplicationIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // ApplicationIcon
            // 
            this.ApplicationIcon.Location = new System.Drawing.Point(3, 3);
            this.ApplicationIcon.Name = "ApplicationIcon";
            this.ApplicationIcon.Size = new System.Drawing.Size(32, 32);
            this.ApplicationIcon.TabIndex = 7;
            this.ApplicationIcon.TabStop = false;
            // 
            // AppName
            // 
            this.AppName.AutoSize = true;
            this.AppName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AppName.Location = new System.Drawing.Point(39, 6);
            this.AppName.Name = "AppName";
            this.AppName.Size = new System.Drawing.Size(62, 13);
            this.AppName.TabIndex = 8;
            this.AppName.Text = "WTMTool";
            // 
            // AppVersion
            // 
            this.AppVersion.AutoSize = true;
            this.AppVersion.Location = new System.Drawing.Point(39, 21);
            this.AppVersion.Name = "AppVersion";
            this.AppVersion.Size = new System.Drawing.Size(40, 13);
            this.AppVersion.TabIndex = 9;
            this.AppVersion.Text = "2.0.0.0";
            // 
            // IconControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.AppVersion);
            this.Controls.Add(this.AppName);
            this.Controls.Add(this.ApplicationIcon);
            this.Name = "IconControl";
            this.Size = new System.Drawing.Size(221, 41);
            ((System.ComponentModel.ISupportInitialize)(this.ApplicationIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
