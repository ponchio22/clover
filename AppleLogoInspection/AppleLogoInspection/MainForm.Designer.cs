namespace AppleLogoInspection
{
    partial class MainForm
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
            this.videoPlayer = new AForge.Controls.VideoSourcePlayer();
            this.startButton = new System.Windows.Forms.Button();
            this.camerasCombo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // videoPlayer
            // 
            this.videoPlayer.Location = new System.Drawing.Point(12, 39);
            this.videoPlayer.Name = "videoPlayer";
            this.videoPlayer.Size = new System.Drawing.Size(430, 334);
            this.videoPlayer.TabIndex = 0;
            this.videoPlayer.VideoSource = null;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(340, 10);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(102, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Iniciar";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // camerasCombo
            // 
            this.camerasCombo.FormattingEnabled = true;
            this.camerasCombo.Location = new System.Drawing.Point(12, 12);
            this.camerasCombo.Name = "camerasCombo";
            this.camerasCombo.Size = new System.Drawing.Size(322, 21);
            this.camerasCombo.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 385);
            this.Controls.Add(this.camerasCombo);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.videoPlayer);
            this.Name = "MainForm";
            this.Text = "Apple Logo Inspection";
            this.ResumeLayout(false);

        }

        #endregion

        private AForge.Controls.VideoSourcePlayer videoPlayer;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ComboBox camerasCombo;

    }
}

