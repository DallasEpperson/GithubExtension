namespace GithubExtension
{
    partial class frmConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfig));
            this.txtGitLocation = new System.Windows.Forms.TextBox();
            this.btnGitLocation = new System.Windows.Forms.Button();
            this.lblGitLocation = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // txtGitLocation
            // 
            this.txtGitLocation.Location = new System.Drawing.Point(12, 28);
            this.txtGitLocation.Name = "txtGitLocation";
            this.txtGitLocation.ReadOnly = true;
            this.txtGitLocation.Size = new System.Drawing.Size(294, 20);
            this.txtGitLocation.TabIndex = 0;
            // 
            // btnGitLocation
            // 
            this.btnGitLocation.Location = new System.Drawing.Point(312, 28);
            this.btnGitLocation.Name = "btnGitLocation";
            this.btnGitLocation.Size = new System.Drawing.Size(75, 20);
            this.btnGitLocation.TabIndex = 1;
            this.btnGitLocation.Text = "Browse...";
            this.btnGitLocation.UseVisualStyleBackColor = true;
            this.btnGitLocation.Click += new System.EventHandler(this.btnGitLocation_Click);
            // 
            // lblGitLocation
            // 
            this.lblGitLocation.AutoSize = true;
            this.lblGitLocation.Location = new System.Drawing.Point(12, 9);
            this.lblGitLocation.Name = "lblGitLocation";
            this.lblGitLocation.Size = new System.Drawing.Size(123, 13);
            this.lblGitLocation.TabIndex = 2;
            this.lblGitLocation.Text = "Git Executable Location:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(312, 68);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 101);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblGitLocation);
            this.Controls.Add(this.btnGitLocation);
            this.Controls.Add(this.txtGitLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmConfig";
            this.Text = "Github Extension Configuration";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGitLocation;
        private System.Windows.Forms.Button btnGitLocation;
        private System.Windows.Forms.Label lblGitLocation;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}