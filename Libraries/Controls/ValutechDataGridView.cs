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
    public class ValutechDataGridView : DataGridView
    {
        public ValutechDataGridView()
            : base()
        {
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.BackgroundColor = Color.FromArgb(210, 210, 210);
            this.ForeColor = Color.Black;
            this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            this.ColumnHeadersDefaultCellStyle = GetHeadersCellStyle();
            this.DefaultCellStyle = GetDefaultCellStyle();
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeColumns = true;
            this.ColumnAdded += new DataGridViewColumnEventHandler(ValutechDataGridView_ColumnAdded);
            this.RowHeadersVisible = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        void ValutechDataGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Index > 0)
            {                
                this.Columns[e.Column.Index - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                this.Columns[e.Column.Index - 1].Width = 120;
            }
            e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private DataGridViewCellStyle GetDefaultCellStyle()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.SelectionBackColor = Color.FromArgb(210, 210, 210);
            style.SelectionForeColor = Color.Black;
            style.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = System.Drawing.SystemColors.Window;
            style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            style.ForeColor = System.Drawing.Color.Black;
            style.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            return style;
        }

        private DataGridViewCellStyle GetHeadersCellStyle()
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = Color.FromArgb(200, 200, 200);
            style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            style.ForeColor = System.Drawing.Color.Black;
            style.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            return style;
        }
    }
}
