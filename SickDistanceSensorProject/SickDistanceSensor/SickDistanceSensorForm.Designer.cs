namespace SickDistanceSensor
{
    partial class SickDistanceSensorForm
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
            this.measurementLabel = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.unlockButton = new System.Windows.Forms.Button();
            this.lockButton = new System.Windows.Forms.Button();
            this.displayButton = new System.Windows.Forms.Button();
            this.setZeroButton = new System.Windows.Forms.Button();
            this.informationField = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // measurementLabel
            // 
            this.measurementLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.measurementLabel.Location = new System.Drawing.Point(3, 0);
            this.measurementLabel.Name = "measurementLabel";
            this.measurementLabel.Size = new System.Drawing.Size(366, 146);
            this.measurementLabel.TabIndex = 0;
            this.measurementLabel.Text = "0.000";
            this.measurementLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // connectButton
            // 
            this.connectButton.Enabled = false;
            this.connectButton.Location = new System.Drawing.Point(12, 227);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(374, 40);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.unlockButton);
            this.panel1.Controls.Add(this.lockButton);
            this.panel1.Controls.Add(this.displayButton);
            this.panel1.Controls.Add(this.setZeroButton);
            this.panel1.Controls.Add(this.measurementLabel);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(374, 177);
            this.panel1.TabIndex = 2;
            // 
            // unlockButton
            // 
            this.unlockButton.Enabled = false;
            this.unlockButton.Location = new System.Drawing.Point(132, 149);
            this.unlockButton.Name = "unlockButton";
            this.unlockButton.Size = new System.Drawing.Size(75, 23);
            this.unlockButton.TabIndex = 7;
            this.unlockButton.Text = "Unlock";
            this.unlockButton.UseVisualStyleBackColor = true;
            this.unlockButton.Click += new System.EventHandler(this.unlockButton_Click);
            // 
            // lockButton
            // 
            this.lockButton.Enabled = false;
            this.lockButton.Location = new System.Drawing.Point(51, 149);
            this.lockButton.Name = "lockButton";
            this.lockButton.Size = new System.Drawing.Size(75, 23);
            this.lockButton.TabIndex = 6;
            this.lockButton.Text = "Lock";
            this.lockButton.UseVisualStyleBackColor = true;
            this.lockButton.Click += new System.EventHandler(this.lockButton_Click);
            // 
            // displayButton
            // 
            this.displayButton.Enabled = false;
            this.displayButton.Location = new System.Drawing.Point(213, 149);
            this.displayButton.Name = "displayButton";
            this.displayButton.Size = new System.Drawing.Size(75, 23);
            this.displayButton.TabIndex = 5;
            this.displayButton.Text = "Display";
            this.displayButton.UseVisualStyleBackColor = true;
            this.displayButton.Click += new System.EventHandler(this.displayButton_Click);
            // 
            // setZeroButton
            // 
            this.setZeroButton.Enabled = false;
            this.setZeroButton.Location = new System.Drawing.Point(294, 149);
            this.setZeroButton.Name = "setZeroButton";
            this.setZeroButton.Size = new System.Drawing.Size(75, 23);
            this.setZeroButton.TabIndex = 4;
            this.setZeroButton.Text = "Set Zero";
            this.setZeroButton.UseVisualStyleBackColor = true;
            this.setZeroButton.Click += new System.EventHandler(this.setZeroButton_Click);
            // 
            // informationField
            // 
            this.informationField.AutoSize = true;
            this.informationField.Location = new System.Drawing.Point(12, 201);
            this.informationField.Name = "informationField";
            this.informationField.Size = new System.Drawing.Size(174, 13);
            this.informationField.TabIndex = 3;
            this.informationField.Text = "Looking for available comm ports ...";
            // 
            // SickDistanceSensorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 281);
            this.Controls.Add(this.informationField);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.connectButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SickDistanceSensorForm";
            this.Text = "Sick Distance Sensor Model: 0D1-B015C05A14";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label measurementLabel;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label informationField;
        private System.Windows.Forms.Button setZeroButton;
        private System.Windows.Forms.Button displayButton;
        private System.Windows.Forms.Button lockButton;
        private System.Windows.Forms.Button unlockButton;
    }
}

