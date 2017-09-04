namespace FacialRecog {
    partial class FaceRecog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel1 = new System.Windows.Forms.Panel();
            this.closestFace = new System.Windows.Forms.PictureBox();
            this.faceImage = new System.Windows.Forms.PictureBox();
            this.PrevFrame = new System.Windows.Forms.PictureBox();
            this.display = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.formatsBox = new System.Windows.Forms.ComboBox();
            this.StopButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.streamDeviceChoiceBox = new System.Windows.Forms.ComboBox();
            this.SnapShotButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closestFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.faceImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PrevFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.display)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.closestFace);
            this.panel1.Controls.Add(this.faceImage);
            this.panel1.Controls.Add(this.PrevFrame);
            this.panel1.Controls.Add(this.display);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1135, 752);
            this.panel1.TabIndex = 2;
            // 
            // closestFace
            // 
            this.closestFace.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.closestFace.Location = new System.Drawing.Point(929, 0);
            this.closestFace.Name = "closestFace";
            this.closestFace.Size = new System.Drawing.Size(100, 100);
            this.closestFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.closestFace.TabIndex = 3;
            this.closestFace.TabStop = false;
            // 
            // faceImage
            // 
            this.faceImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.faceImage.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.faceImage.Location = new System.Drawing.Point(1035, 0);
            this.faceImage.Name = "faceImage";
            this.faceImage.Size = new System.Drawing.Size(100, 100);
            this.faceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.faceImage.TabIndex = 2;
            this.faceImage.TabStop = false;
            // 
            // PrevFrame
            // 
            this.PrevFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PrevFrame.Location = new System.Drawing.Point(876, 493);
            this.PrevFrame.Name = "PrevFrame";
            this.PrevFrame.Size = new System.Drawing.Size(256, 256);
            this.PrevFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PrevFrame.TabIndex = 1;
            this.PrevFrame.TabStop = false;
            // 
            // display
            // 
            this.display.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.display.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.display.Location = new System.Drawing.Point(0, 0);
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(1135, 752);
            this.display.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.display.TabIndex = 0;
            this.display.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.SnapShotButton);
            this.panel2.Controls.Add(this.formatsBox);
            this.panel2.Controls.Add(this.StopButton);
            this.panel2.Controls.Add(this.StartButton);
            this.panel2.Controls.Add(this.streamDeviceChoiceBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 758);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1135, 100);
            this.panel2.TabIndex = 3;
            // 
            // formatsBox
            // 
            this.formatsBox.FormattingEnabled = true;
            this.formatsBox.Location = new System.Drawing.Point(361, 60);
            this.formatsBox.Name = "formatsBox";
            this.formatsBox.Size = new System.Drawing.Size(362, 28);
            this.formatsBox.TabIndex = 3;
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(204, 4);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(150, 50);
            this.StopButton.TabIndex = 2;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(12, 4);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(150, 50);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // streamDeviceChoiceBox
            // 
            this.streamDeviceChoiceBox.FormattingEnabled = true;
            this.streamDeviceChoiceBox.Location = new System.Drawing.Point(12, 60);
            this.streamDeviceChoiceBox.Name = "streamDeviceChoiceBox";
            this.streamDeviceChoiceBox.Size = new System.Drawing.Size(342, 28);
            this.streamDeviceChoiceBox.TabIndex = 0;
            // 
            // SnapShotButton
            // 
            this.SnapShotButton.Location = new System.Drawing.Point(360, 4);
            this.SnapShotButton.Name = "SnapShotButton";
            this.SnapShotButton.Size = new System.Drawing.Size(150, 50);
            this.SnapShotButton.TabIndex = 4;
            this.SnapShotButton.Text = "Snapshot";
            this.SnapShotButton.UseVisualStyleBackColor = true;
            this.SnapShotButton.Click += new System.EventHandler(this.SnapShotButton_Click);
            // 
            // FaceRecog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1135, 858);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FaceRecog";
            this.Text = "Facial Recognition - Rakhlei";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FaceRecog_FormClosing);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.closestFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.faceImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PrevFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.display)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox display;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox streamDeviceChoiceBox;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.ComboBox formatsBox;
        private System.Windows.Forms.PictureBox PrevFrame;
        private System.Windows.Forms.PictureBox faceImage;
        private System.Windows.Forms.PictureBox closestFace;
        private System.Windows.Forms.Button SnapShotButton;
    }
}

