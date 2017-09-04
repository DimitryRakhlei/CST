using System.ComponentModel;
using System.Windows.Forms;

namespace Compression {
    partial class MediaCompression {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.menuStrip = new System.Windows.Forms.StatusStrip();
            this.toolsFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.openCompressedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPEGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSecondFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressBothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompressToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.decompressToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SizeToolStrip = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.FOpen = new System.Windows.Forms.OpenFileDialog();
            this.leftPicture = new Cyotek.Windows.Forms.ImageBox();
            this.rightPicture = new Cyotek.Windows.Forms.ImageBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.splitContainer);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 0);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 31);
            this.contentPanel.Size = new System.Drawing.Size(922, 483);
            this.contentPanel.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.leftPicture);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.rightPicture);
            this.splitContainer.Size = new System.Drawing.Size(922, 452);
            this.splitContainer.SplitterDistance = 459;
            this.splitContainer.TabIndex = 0;
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsFile,
            this.SizeToolStrip,
            this.progressBar});
            this.menuStrip.Location = new System.Drawing.Point(0, 452);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(922, 31);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "statusStrip1";
            // 
            // toolsFile
            // 
            this.toolsFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCompressedToolStripMenuItem,
            this.compressToolStripMenuItem,
            this.mPEGToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.testToolStripMenuItem});
            this.toolsFile.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolsFile.Name = "toolsFile";
            this.toolsFile.Size = new System.Drawing.Size(38, 29);
            this.toolsFile.Text = "File";
            // 
            // openCompressedToolStripMenuItem
            // 
            this.openCompressedToolStripMenuItem.Name = "openCompressedToolStripMenuItem";
            this.openCompressedToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.openCompressedToolStripMenuItem.Text = "Decompress Image";
            this.openCompressedToolStripMenuItem.Click += new System.EventHandler(this.openCompressedToolStripMenuItem_Click);
            // 
            // compressToolStripMenuItem
            // 
            this.compressToolStripMenuItem.Name = "compressToolStripMenuItem";
            this.compressToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.compressToolStripMenuItem.Text = "Compress Image";
            this.compressToolStripMenuItem.Click += new System.EventHandler(this.compressToolStripMenuItem_Click);
            // 
            // mPEGToolStripMenuItem
            // 
            this.mPEGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadSecondFileToolStripMenuItem,
            this.compressBothToolStripMenuItem,
            this.decompressToolStripMenuItem,
            this.decompressToolStripMenuItem1,
            this.decompressToolStripMenuItem2});
            this.mPEGToolStripMenuItem.Name = "mPEGToolStripMenuItem";
            this.mPEGToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.mPEGToolStripMenuItem.Text = "MPEG";
            // 
            // loadSecondFileToolStripMenuItem
            // 
            this.loadSecondFileToolStripMenuItem.Name = "loadSecondFileToolStripMenuItem";
            this.loadSecondFileToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.loadSecondFileToolStripMenuItem.Text = "Load First File";
            this.loadSecondFileToolStripMenuItem.Click += new System.EventHandler(this.loadFirstFileToolStripMenuItem_Click);
            // 
            // compressBothToolStripMenuItem
            // 
            this.compressBothToolStripMenuItem.Name = "compressBothToolStripMenuItem";
            this.compressBothToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.compressBothToolStripMenuItem.Text = "Load Second File";
            this.compressBothToolStripMenuItem.Click += new System.EventHandler(this.loadSecondFileToolStripMenuItem_Click);
            // 
            // decompressToolStripMenuItem
            // 
            this.decompressToolStripMenuItem.Name = "decompressToolStripMenuItem";
            this.decompressToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.decompressToolStripMenuItem.Text = "MV";
            this.decompressToolStripMenuItem.Click += new System.EventHandler(this.MVToolStripMenuItem_Click);
            // 
            // decompressToolStripMenuItem1
            // 
            this.decompressToolStripMenuItem1.Name = "decompressToolStripMenuItem1";
            this.decompressToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.decompressToolStripMenuItem1.Text = "Compress";
            this.decompressToolStripMenuItem1.Click += new System.EventHandler(this.compressToolStripMenuItem1_Click);
            // 
            // decompressToolStripMenuItem2
            // 
            this.decompressToolStripMenuItem2.Name = "decompressToolStripMenuItem2";
            this.decompressToolStripMenuItem2.Size = new System.Drawing.Size(163, 22);
            this.decompressToolStripMenuItem2.Text = "Decompress";
            this.decompressToolStripMenuItem2.Click += new System.EventHandler(this.decompressToolStripMenuItem2_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.testToolStripMenuItem.Text = "Test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // SizeToolStrip
            // 
            this.SizeToolStrip.AutoSize = false;
            this.SizeToolStrip.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SizeToolStrip.Name = "SizeToolStrip";
            this.SizeToolStrip.Size = new System.Drawing.Size(100, 26);
            this.SizeToolStrip.Text = "Size:";
            this.SizeToolStrip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 25);
            // 
            // leftPicture
            // 
            this.leftPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPicture.Location = new System.Drawing.Point(0, 0);
            this.leftPicture.Name = "leftPicture";
            this.leftPicture.Size = new System.Drawing.Size(459, 452);
            this.leftPicture.TabIndex = 1;
            this.leftPicture.Click += new System.EventHandler(this.leftPicture_Click);
            // 
            // rightPicture
            // 
            this.rightPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPicture.Location = new System.Drawing.Point(0, 0);
            this.rightPicture.Name = "rightPicture";
            this.rightPicture.Size = new System.Drawing.Size(459, 452);
            this.rightPicture.TabIndex = 0;
            this.rightPicture.Click += new System.EventHandler(this.rightPicture_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            // 
            // MediaCompression
            // 
            this.AccessibleName = "                                                                                 " +
    "                                                                                " +
    "                                     ";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(922, 483);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.contentPanel);
            this.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MediaCompression";
            this.Opacity = 0.99D;
            this.ShowIcon = false;
            this.Text = "Media Compression";
            this.contentPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel contentPanel;
        private StatusStrip menuStrip;
        private SplitContainer splitContainer;
        private ToolStripStatusLabel SizeToolStrip;
        private ToolStripDropDownButton toolsFile;
        private OpenFileDialog FOpen;
        private ToolStripMenuItem openCompressedToolStripMenuItem;
        private ToolStripMenuItem compressToolStripMenuItem;
        private ToolStripMenuItem mPEGToolStripMenuItem;
        private ToolStripMenuItem loadSecondFileToolStripMenuItem;
        private ToolStripMenuItem compressBothToolStripMenuItem;
        private ToolStripMenuItem decompressToolStripMenuItem;
        private ToolStripMenuItem decompressToolStripMenuItem1;
        private ToolStripMenuItem decompressToolStripMenuItem2;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripProgressBar progressBar;
        private Cyotek.Windows.Forms.ImageBox leftPicture;
        private Cyotek.Windows.Forms.ImageBox rightPicture;
        private ContextMenuStrip contextMenuStrip1;
        private ContextMenuStrip contextMenuStrip2;
    }
}

