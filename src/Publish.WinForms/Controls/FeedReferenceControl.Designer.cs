﻿namespace ZeroInstall.Publish.WinForms.Controls
{
    partial class FeedReferenceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblExtFeedURL = new System.Windows.Forms.Label();
            this.textBoxExtFeedURL = new Common.Controls.HintTextBox();
            this.targetBaseControl = new ZeroInstall.Publish.WinForms.Controls.TargetBaseControl();
            this.SuspendLayout();
            // 
            // lblExtFeedURL
            // 
            this.lblExtFeedURL.AutoSize = true;
            this.lblExtFeedURL.Location = new System.Drawing.Point(-3, 0);
            this.lblExtFeedURL.Name = "lblExtFeedURL";
            this.lblExtFeedURL.Size = new System.Drawing.Size(56, 13);
            this.lblExtFeedURL.TabIndex = 0;
            this.lblExtFeedURL.Text = "Feed URL";
            // 
            // textBoxExtFeedURL
            // 
            this.textBoxExtFeedURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExtFeedURL.HintText = "URL to an external feed";
            this.textBoxExtFeedURL.Location = new System.Drawing.Point(0, 16);
            this.textBoxExtFeedURL.Name = "textBoxExtFeedURL";
            this.textBoxExtFeedURL.ShowClearButton = true;
            this.textBoxExtFeedURL.Size = new System.Drawing.Size(502, 20);
            this.textBoxExtFeedURL.TabIndex = 1;
            this.textBoxExtFeedURL.TextChanged += new System.EventHandler(this.textBoxExtFeedURL_TextChanged);
            // 
            // targetBaseControl
            // 
            this.targetBaseControl.Location = new System.Drawing.Point(0, 40);
            this.targetBaseControl.Name = "targetBaseControl";
            this.targetBaseControl.Size = new System.Drawing.Size(502, 128);
            this.targetBaseControl.TabIndex = 2;
            // 
            // FeedReferenceControl
            // 
            this.Controls.Add(this.lblExtFeedURL);
            this.Controls.Add(this.textBoxExtFeedURL);
            this.Controls.Add(this.targetBaseControl);
            this.Name = "FeedReferenceControl";
            this.Size = new System.Drawing.Size(502, 168);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TargetBaseControl targetBaseControl;
        private System.Windows.Forms.Label lblExtFeedURL;
        private Common.Controls.HintTextBox textBoxExtFeedURL;
    }
}
