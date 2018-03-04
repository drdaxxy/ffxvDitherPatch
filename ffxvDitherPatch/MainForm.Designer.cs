namespace ffxvDitherPatch
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.processButton = new System.Windows.Forms.Button();
            this.statusDescLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.headingLabel = new System.Windows.Forms.Label();
            this.versionDescLabel = new System.Windows.Forms.Label();
            this.projectLink = new System.Windows.Forms.LinkLabel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.divider1 = new System.Windows.Forms.Label();
            this.divider2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.SystemColors.ControlLight;
            this.progressBar.Location = new System.Drawing.Point(16, 104);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(312, 31);
            this.progressBar.TabIndex = 0;
            // 
            // processButton
            // 
            this.processButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.processButton.Enabled = false;
            this.processButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processButton.Location = new System.Drawing.Point(16, 520);
            this.processButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(312, 31);
            this.processButton.TabIndex = 1;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = false;
            this.processButton.Click += new System.EventHandler(this.processButton_Click);
            // 
            // statusDescLabel
            // 
            this.statusDescLabel.AutoSize = true;
            this.statusDescLabel.Location = new System.Drawing.Point(16, 80);
            this.statusDescLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusDescLabel.Name = "statusDescLabel";
            this.statusDescLabel.Size = new System.Drawing.Size(45, 15);
            this.statusDescLabel.TabIndex = 2;
            this.statusDescLabel.Text = "Status: ";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(72, 80);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(50, 15);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Loading";
            // 
            // headingLabel
            // 
            this.headingLabel.AutoSize = true;
            this.headingLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headingLabel.Location = new System.Drawing.Point(16, 8);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Size = new System.Drawing.Size(98, 15);
            this.headingLabel.TabIndex = 4;
            this.headingLabel.Text = "ffxvDitherPatch";
            // 
            // versionDescLabel
            // 
            this.versionDescLabel.AutoSize = true;
            this.versionDescLabel.Location = new System.Drawing.Point(16, 56);
            this.versionDescLabel.Name = "versionDescLabel";
            this.versionDescLabel.Size = new System.Drawing.Size(48, 15);
            this.versionDescLabel.TabIndex = 5;
            this.versionDescLabel.Text = "Version:";
            // 
            // projectLink
            // 
            this.projectLink.ActiveLinkColor = System.Drawing.SystemColors.ControlText;
            this.projectLink.AutoSize = true;
            this.projectLink.LinkColor = System.Drawing.SystemColors.ControlText;
            this.projectLink.Location = new System.Drawing.Point(16, 32);
            this.projectLink.Name = "projectLink";
            this.projectLink.Size = new System.Drawing.Size(240, 15);
            this.projectLink.TabIndex = 6;
            this.projectLink.TabStop = true;
            this.projectLink.Text = "https://github.com/drdaxxy/ffxvDitherPatch";
            this.projectLink.VisitedLinkColor = System.Drawing.SystemColors.ControlText;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Location = new System.Drawing.Point(72, 56);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(22, 15);
            this.versionLabel.TabIndex = 7;
            this.versionLabel.Text = "1.0";
            // 
            // divider1
            // 
            this.divider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.divider1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divider1.Location = new System.Drawing.Point(16, 152);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(312, 1);
            this.divider1.TabIndex = 8;
            // 
            // divider2
            // 
            this.divider2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.divider2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divider2.Location = new System.Drawing.Point(16, 504);
            this.divider2.Name = "divider2";
            this.divider2.Size = new System.Drawing.Size(312, 1);
            this.divider2.TabIndex = 9;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(344, 561);
            this.Controls.Add(this.divider2);
            this.Controls.Add(this.divider1);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.projectLink);
            this.Controls.Add(this.versionDescLabel);
            this.Controls.Add(this.headingLabel);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.statusDescLabel);
            this.Controls.Add(this.processButton);
            this.Controls.Add(this.progressBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "ffxvDitherPatch";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.Label statusDescLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label headingLabel;
        private System.Windows.Forms.Label versionDescLabel;
        private System.Windows.Forms.LinkLabel projectLink;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label divider1;
        private System.Windows.Forms.Label divider2;
    }
}

