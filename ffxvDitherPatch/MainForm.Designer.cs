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
            this.originalLabel = new System.Windows.Forms.Label();
            this.originalPic = new System.Windows.Forms.PictureBox();
            this.originalDescLabel = new System.Windows.Forms.Label();
            this.widePic = new System.Windows.Forms.PictureBox();
            this.wideLabel = new System.Windows.Forms.Label();
            this.mediumPic = new System.Windows.Forms.PictureBox();
            this.mediumLabel = new System.Windows.Forms.Label();
            this.narrowPic = new System.Windows.Forms.PictureBox();
            this.narrowLabel = new System.Windows.Forms.Label();
            this.offDescLabel = new System.Windows.Forms.Label();
            this.offPic = new System.Windows.Forms.PictureBox();
            this.offLabel = new System.Windows.Forms.Label();
            this.wideRadio = new System.Windows.Forms.RadioButton();
            this.narrowRadio = new System.Windows.Forms.RadioButton();
            this.mediumRadio = new System.Windows.Forms.RadioButton();
            this.offRadio = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.originalPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediumPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.narrowPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.offPic)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.BackColor = System.Drawing.SystemColors.ControlLight;
            this.progressBar.Location = new System.Drawing.Point(364, 56);
            this.progressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(320, 40);
            this.progressBar.TabIndex = 0;
            // 
            // processButton
            // 
            this.processButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.processButton.Enabled = false;
            this.processButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.processButton.Location = new System.Drawing.Point(364, 8);
            this.processButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(320, 40);
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
            this.divider1.Location = new System.Drawing.Point(16, 112);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(672, 1);
            this.divider1.TabIndex = 8;
            // 
            // originalLabel
            // 
            this.originalLabel.AutoSize = true;
            this.originalLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.originalLabel.Location = new System.Drawing.Point(16, 128);
            this.originalLabel.Name = "originalLabel";
            this.originalLabel.Size = new System.Drawing.Size(82, 15);
            this.originalLabel.TabIndex = 10;
            this.originalLabel.Text = "Original (16x)";
            // 
            // originalPic
            // 
            this.originalPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.originalPic.Image = global::ffxvDitherPatch.Assets._16_crop;
            this.originalPic.Location = new System.Drawing.Point(520, 128);
            this.originalPic.Name = "originalPic";
            this.originalPic.Size = new System.Drawing.Size(150, 110);
            this.originalPic.TabIndex = 11;
            this.originalPic.TabStop = false;
            // 
            // originalDescLabel
            // 
            this.originalDescLabel.AutoSize = true;
            this.originalDescLabel.Location = new System.Drawing.Point(16, 152);
            this.originalDescLabel.Name = "originalDescLabel";
            this.originalDescLabel.Size = new System.Drawing.Size(76, 15);
            this.originalDescLabel.TabIndex = 12;
            this.originalDescLabel.Text = "For reference";
            // 
            // widePic
            // 
            this.widePic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.widePic.Image = global::ffxvDitherPatch.Assets._32_crop;
            this.widePic.Location = new System.Drawing.Point(176, 264);
            this.widePic.Name = "widePic";
            this.widePic.Size = new System.Drawing.Size(150, 110);
            this.widePic.TabIndex = 33;
            this.widePic.TabStop = false;
            // 
            // wideLabel
            // 
            this.wideLabel.AutoSize = true;
            this.wideLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wideLabel.Location = new System.Drawing.Point(16, 264);
            this.wideLabel.Name = "wideLabel";
            this.wideLabel.Size = new System.Drawing.Size(68, 15);
            this.wideLabel.TabIndex = 32;
            this.wideLabel.Text = "Wide (32x)";
            // 
            // mediumPic
            // 
            this.mediumPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mediumPic.Image = global::ffxvDitherPatch.Assets._40_crop;
            this.mediumPic.Location = new System.Drawing.Point(520, 264);
            this.mediumPic.Name = "mediumPic";
            this.mediumPic.Size = new System.Drawing.Size(150, 110);
            this.mediumPic.TabIndex = 37;
            this.mediumPic.TabStop = false;
            // 
            // mediumLabel
            // 
            this.mediumLabel.AutoSize = true;
            this.mediumLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mediumLabel.Location = new System.Drawing.Point(364, 264);
            this.mediumLabel.Name = "mediumLabel";
            this.mediumLabel.Size = new System.Drawing.Size(85, 15);
            this.mediumLabel.TabIndex = 36;
            this.mediumLabel.Text = "Medium (40x)";
            // 
            // narrowPic
            // 
            this.narrowPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.narrowPic.Image = global::ffxvDitherPatch.Assets._56_crop;
            this.narrowPic.Location = new System.Drawing.Point(176, 408);
            this.narrowPic.Name = "narrowPic";
            this.narrowPic.Size = new System.Drawing.Size(150, 110);
            this.narrowPic.TabIndex = 41;
            this.narrowPic.TabStop = false;
            // 
            // narrowLabel
            // 
            this.narrowLabel.AutoSize = true;
            this.narrowLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.narrowLabel.Location = new System.Drawing.Point(16, 408);
            this.narrowLabel.Name = "narrowLabel";
            this.narrowLabel.Size = new System.Drawing.Size(81, 15);
            this.narrowLabel.TabIndex = 40;
            this.narrowLabel.Text = "Narrow (56x)";
            // 
            // offDescLabel
            // 
            this.offDescLabel.AutoSize = true;
            this.offDescLabel.Location = new System.Drawing.Point(364, 432);
            this.offDescLabel.Name = "offDescLabel";
            this.offDescLabel.Size = new System.Drawing.Size(52, 15);
            this.offDescLabel.TabIndex = 46;
            this.offDescLabel.Text = "Bad idea";
            // 
            // offPic
            // 
            this.offPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.offPic.Image = global::ffxvDitherPatch.Assets.off_crop;
            this.offPic.Location = new System.Drawing.Point(520, 408);
            this.offPic.Name = "offPic";
            this.offPic.Size = new System.Drawing.Size(150, 110);
            this.offPic.TabIndex = 45;
            this.offPic.TabStop = false;
            // 
            // offLabel
            // 
            this.offLabel.AutoSize = true;
            this.offLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.offLabel.Location = new System.Drawing.Point(364, 408);
            this.offLabel.Name = "offLabel";
            this.offLabel.Size = new System.Drawing.Size(26, 15);
            this.offLabel.TabIndex = 44;
            this.offLabel.Text = "Off";
            // 
            // wideRadio
            // 
            this.wideRadio.AutoSize = true;
            this.wideRadio.Location = new System.Drawing.Point(16, 312);
            this.wideRadio.Name = "wideRadio";
            this.wideRadio.Size = new System.Drawing.Size(14, 13);
            this.wideRadio.TabIndex = 47;
            this.wideRadio.TabStop = true;
            this.wideRadio.UseVisualStyleBackColor = true;
            // 
            // narrowRadio
            // 
            this.narrowRadio.AutoSize = true;
            this.narrowRadio.Location = new System.Drawing.Point(16, 456);
            this.narrowRadio.Name = "narrowRadio";
            this.narrowRadio.Size = new System.Drawing.Size(14, 13);
            this.narrowRadio.TabIndex = 48;
            this.narrowRadio.TabStop = true;
            this.narrowRadio.UseVisualStyleBackColor = true;
            // 
            // mediumRadio
            // 
            this.mediumRadio.AutoSize = true;
            this.mediumRadio.Location = new System.Drawing.Point(364, 312);
            this.mediumRadio.Name = "mediumRadio";
            this.mediumRadio.Size = new System.Drawing.Size(14, 13);
            this.mediumRadio.TabIndex = 49;
            this.mediumRadio.TabStop = true;
            this.mediumRadio.UseVisualStyleBackColor = true;
            // 
            // offRadio
            // 
            this.offRadio.AutoSize = true;
            this.offRadio.Location = new System.Drawing.Point(364, 456);
            this.offRadio.Name = "offRadio";
            this.offRadio.Size = new System.Drawing.Size(14, 13);
            this.offRadio.TabIndex = 50;
            this.offRadio.TabStop = true;
            this.offRadio.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(704, 537);
            this.Controls.Add(this.offRadio);
            this.Controls.Add(this.mediumRadio);
            this.Controls.Add(this.narrowRadio);
            this.Controls.Add(this.wideRadio);
            this.Controls.Add(this.offDescLabel);
            this.Controls.Add(this.offPic);
            this.Controls.Add(this.offLabel);
            this.Controls.Add(this.narrowPic);
            this.Controls.Add(this.narrowLabel);
            this.Controls.Add(this.mediumPic);
            this.Controls.Add(this.mediumLabel);
            this.Controls.Add(this.widePic);
            this.Controls.Add(this.wideLabel);
            this.Controls.Add(this.originalDescLabel);
            this.Controls.Add(this.originalPic);
            this.Controls.Add(this.originalLabel);
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
            ((System.ComponentModel.ISupportInitialize)(this.originalPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widePic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mediumPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.narrowPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.offPic)).EndInit();
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
        private System.Windows.Forms.Label originalLabel;
        private System.Windows.Forms.PictureBox originalPic;
        private System.Windows.Forms.Label originalDescLabel;
        private System.Windows.Forms.PictureBox widePic;
        private System.Windows.Forms.Label wideLabel;
        private System.Windows.Forms.PictureBox mediumPic;
        private System.Windows.Forms.Label mediumLabel;
        private System.Windows.Forms.PictureBox narrowPic;
        private System.Windows.Forms.Label narrowLabel;
        private System.Windows.Forms.Label offDescLabel;
        private System.Windows.Forms.PictureBox offPic;
        private System.Windows.Forms.Label offLabel;
        private System.Windows.Forms.RadioButton wideRadio;
        private System.Windows.Forms.RadioButton narrowRadio;
        private System.Windows.Forms.RadioButton mediumRadio;
        private System.Windows.Forms.RadioButton offRadio;
    }
}

