namespace Valutech.Electrox
{
    partial class LaserSelection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addButton = new System.Windows.Forms.Button();
            this.laserSelectionButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.plantComboBox = new System.Windows.Forms.ComboBox();
            this.areaComboBox = new System.Windows.Forms.ComboBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.laserNameLabel = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.hardwareInfoLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.programsTextBox = new System.Windows.Forms.TextBox();
            this.programsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.connectButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.propertiesDataGridView = new Valutech.Controls.ValutechDataGridView();
            this.laserEquipmentDataGridView = new Valutech.Controls.ValutechDataGridView();
            this.laserEquipmentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.propertiesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.laserEquipmentDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.laserEquipmentBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addButton);
            this.groupBox1.Controls.Add(this.laserSelectionButton);
            this.groupBox1.Controls.Add(this.laserEquipmentDataGridView);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 494);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(6, 465);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 5;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // laserSelectionButton
            // 
            this.laserSelectionButton.Location = new System.Drawing.Point(342, 465);
            this.laserSelectionButton.Name = "laserSelectionButton";
            this.laserSelectionButton.Size = new System.Drawing.Size(75, 23);
            this.laserSelectionButton.TabIndex = 4;
            this.laserSelectionButton.Text = "Open";
            this.laserSelectionButton.UseVisualStyleBackColor = true;
            this.laserSelectionButton.Click += new System.EventHandler(this.laserSelectionButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Select the laser equipment to use:";
            // 
            // plantComboBox
            // 
            this.plantComboBox.FormattingEnabled = true;
            this.plantComboBox.Location = new System.Drawing.Point(3, 6);
            this.plantComboBox.Name = "plantComboBox";
            this.plantComboBox.Size = new System.Drawing.Size(424, 21);
            this.plantComboBox.TabIndex = 4;
            // 
            // areaComboBox
            // 
            this.areaComboBox.FormattingEnabled = true;
            this.areaComboBox.Location = new System.Drawing.Point(3, 32);
            this.areaComboBox.Name = "areaComboBox";
            this.areaComboBox.Size = new System.Drawing.Size(424, 21);
            this.areaComboBox.TabIndex = 7;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(486, 37);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(37, 13);
            this.statusLabel.TabIndex = 11;
            this.statusLabel.Text = "Offline";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(439, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Status:";
            // 
            // laserNameLabel
            // 
            this.laserNameLabel.AutoSize = true;
            this.laserNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.laserNameLabel.Location = new System.Drawing.Point(436, 9);
            this.laserNameLabel.Name = "laserNameLabel";
            this.laserNameLabel.Size = new System.Drawing.Size(64, 18);
            this.laserNameLabel.TabIndex = 9;
            this.laserNameLabel.Text = "Laser 1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(433, 59);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(304, 494);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.deleteButton);
            this.tabPage1.Controls.Add(this.propertiesDataGridView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(296, 468);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Properties";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.hardwareInfoLabel);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.programsTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(296, 468);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Information";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // hardwareInfoLabel
            // 
            this.hardwareInfoLabel.AutoSize = true;
            this.hardwareInfoLabel.Location = new System.Drawing.Point(69, 7);
            this.hardwareInfoLabel.Name = "hardwareInfoLabel";
            this.hardwareInfoLabel.Size = new System.Drawing.Size(10, 13);
            this.hardwareInfoLabel.TabIndex = 14;
            this.hardwareInfoLabel.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Hardware:";
            // 
            // programsTextBox
            // 
            this.programsTextBox.BackColor = System.Drawing.Color.White;
            this.programsTextBox.Location = new System.Drawing.Point(3, 33);
            this.programsTextBox.Multiline = true;
            this.programsTextBox.Name = "programsTextBox";
            this.programsTextBox.ReadOnly = true;
            this.programsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.programsTextBox.Size = new System.Drawing.Size(290, 429);
            this.programsTextBox.TabIndex = 12;
            // 
            // programsBindingSource
            // 
            this.programsBindingSource.DataMember = "Programs";
            this.programsBindingSource.DataSource = this.laserEquipmentBindingSource;
            // 
            // connectButton
            // 
            this.connectButton.Enabled = false;
            this.connectButton.Location = new System.Drawing.Point(658, 6);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 12;
            this.connectButton.Text = "Retry";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Visible = false;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(6, 439);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 6;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // propertiesDataGridView
            // 
            this.propertiesDataGridView.AllowUserToAddRows = false;
            this.propertiesDataGridView.AllowUserToDeleteRows = false;
            this.propertiesDataGridView.AllowUserToResizeColumns = false;
            this.propertiesDataGridView.AllowUserToResizeRows = false;
            this.propertiesDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.propertiesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.propertiesDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.propertiesDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.propertiesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.propertiesDataGridView.ColumnHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.propertiesDataGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.propertiesDataGridView.Location = new System.Drawing.Point(2, 6);
            this.propertiesDataGridView.Name = "propertiesDataGridView";
            this.propertiesDataGridView.RowHeadersVisible = false;
            this.propertiesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.propertiesDataGridView.Size = new System.Drawing.Size(291, 431);
            this.propertiesDataGridView.TabIndex = 14;
            // 
            // laserEquipmentDataGridView
            // 
            this.laserEquipmentDataGridView.AllowUserToAddRows = false;
            this.laserEquipmentDataGridView.AllowUserToDeleteRows = false;
            this.laserEquipmentDataGridView.AllowUserToResizeRows = false;
            this.laserEquipmentDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.laserEquipmentDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.laserEquipmentDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.laserEquipmentDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.laserEquipmentDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.laserEquipmentDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.laserEquipmentDataGridView.Location = new System.Drawing.Point(6, 42);
            this.laserEquipmentDataGridView.Name = "laserEquipmentDataGridView";
            this.laserEquipmentDataGridView.ReadOnly = true;
            this.laserEquipmentDataGridView.RowHeadersVisible = false;
            this.laserEquipmentDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.laserEquipmentDataGridView.Size = new System.Drawing.Size(411, 417);
            this.laserEquipmentDataGridView.TabIndex = 4;
            // 
            // laserEquipmentBindingSource
            // 
            this.laserEquipmentBindingSource.DataSource = typeof(Valutech.Electrox.LaserEquipment);
            // 
            // LaserSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 557);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.laserNameLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.areaComboBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.plantComboBox);
            this.Controls.Add(this.groupBox1);
            this.Name = "LaserSelection";
            this.Text = "Laser Selection";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.propertiesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.laserEquipmentDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.laserEquipmentBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion        
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private Valutech.Controls.ValutechDataGridView laserEquipmentDataGridView;
        private System.Windows.Forms.Button laserSelectionButton;
        private System.Windows.Forms.ComboBox plantComboBox;
        private System.Windows.Forms.ComboBox areaComboBox;
        private System.Windows.Forms.Label laserNameLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.BindingSource laserEquipmentBindingSource;
        private System.Windows.Forms.BindingSource programsBindingSource;
        private Controls.ValutechDataGridView propertiesDataGridView;
        private System.Windows.Forms.TextBox programsTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label hardwareInfoLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
    }
}

