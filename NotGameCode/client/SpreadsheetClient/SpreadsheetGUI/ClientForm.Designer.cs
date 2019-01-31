namespace Client
{
    partial class ClientForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.UserNameTB = new System.Windows.Forms.TextBox();
            this.IPAddressTB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.InputEnter = new System.Windows.Forms.Button();
            this.WhichSpreadLabel = new System.Windows.Forms.Label();
            this.OpenButton = new System.Windows.Forms.Button();
            this.FileChoice = new System.Windows.Forms.ComboBox();
            this.NewFileTB = new System.Windows.Forms.TextBox();
            this.NewFileLabel = new System.Windows.Forms.Label();
            this.CancelB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(104, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User name";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // UserNameTB
            // 
            this.UserNameTB.Location = new System.Drawing.Point(54, 25);
            this.UserNameTB.Name = "UserNameTB";
            this.UserNameTB.Size = new System.Drawing.Size(176, 20);
            this.UserNameTB.TabIndex = 1;
            // 
            // IPAddressTB
            // 
            this.IPAddressTB.Location = new System.Drawing.Point(54, 64);
            this.IPAddressTB.Name = "IPAddressTB";
            this.IPAddressTB.Size = new System.Drawing.Size(176, 20);
            this.IPAddressTB.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "IP Address";
            // 
            // InputEnter
            // 
            this.InputEnter.Location = new System.Drawing.Point(98, 207);
            this.InputEnter.Name = "InputEnter";
            this.InputEnter.Size = new System.Drawing.Size(75, 23);
            this.InputEnter.TabIndex = 4;
            this.InputEnter.Text = "Enter";
            this.InputEnter.UseVisualStyleBackColor = true;
            this.InputEnter.Click += new System.EventHandler(this.InputEnter_Click);
            // 
            // WhichSpreadLabel
            // 
            this.WhichSpreadLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.WhichSpreadLabel.AutoSize = true;
            this.WhichSpreadLabel.Location = new System.Drawing.Point(35, 96);
            this.WhichSpreadLabel.Name = "WhichSpreadLabel";
            this.WhichSpreadLabel.Size = new System.Drawing.Size(214, 13);
            this.WhichSpreadLabel.TabIndex = 5;
            this.WhichSpreadLabel.Text = "Which spreadsheet would you like to open?";
            this.WhichSpreadLabel.Visible = false;
            // 
            // OpenButton
            // 
            this.OpenButton.Enabled = false;
            this.OpenButton.Location = new System.Drawing.Point(98, 178);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(75, 23);
            this.OpenButton.TabIndex = 7;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Visible = false;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // FileChoice
            // 
            this.FileChoice.Enabled = false;
            this.FileChoice.FormattingEnabled = true;
            this.FileChoice.Location = new System.Drawing.Point(53, 112);
            this.FileChoice.Name = "FileChoice";
            this.FileChoice.Size = new System.Drawing.Size(176, 21);
            this.FileChoice.TabIndex = 8;
            this.FileChoice.Visible = false;
            // 
            // NewFileTB
            // 
            this.NewFileTB.Enabled = false;
            this.NewFileTB.Location = new System.Drawing.Point(54, 152);
            this.NewFileTB.Name = "NewFileTB";
            this.NewFileTB.Size = new System.Drawing.Size(176, 20);
            this.NewFileTB.TabIndex = 10;
            this.NewFileTB.Visible = false;
            // 
            // NewFileLabel
            // 
            this.NewFileLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.NewFileLabel.AutoSize = true;
            this.NewFileLabel.Enabled = false;
            this.NewFileLabel.Location = new System.Drawing.Point(51, 136);
            this.NewFileLabel.Name = "NewFileLabel";
            this.NewFileLabel.Size = new System.Drawing.Size(184, 13);
            this.NewFileLabel.TabIndex = 9;
            this.NewFileLabel.Text = "What would you like to name the file?";
            this.NewFileLabel.Visible = false;
            // 
            // CancelB
            // 
            this.CancelB.Enabled = false;
            this.CancelB.Location = new System.Drawing.Point(98, 226);
            this.CancelB.Name = "CancelB";
            this.CancelB.Size = new System.Drawing.Size(75, 23);
            this.CancelB.TabIndex = 11;
            this.CancelB.Text = "Cancel";
            this.CancelB.UseVisualStyleBackColor = true;
            this.CancelB.Visible = false;
            this.CancelB.Click += new System.EventHandler(this.CancelB_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.CancelB);
            this.Controls.Add(this.NewFileTB);
            this.Controls.Add(this.NewFileLabel);
            this.Controls.Add(this.FileChoice);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.WhichSpreadLabel);
            this.Controls.Add(this.InputEnter);
            this.Controls.Add(this.IPAddressTB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UserNameTB);
            this.Controls.Add(this.label1);
            this.Name = "ClientForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox UserNameTB;
        private System.Windows.Forms.TextBox IPAddressTB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button InputEnter;
        private System.Windows.Forms.Label WhichSpreadLabel;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.ComboBox FileChoice;
        private System.Windows.Forms.TextBox NewFileTB;
        private System.Windows.Forms.Label NewFileLabel;
        private System.Windows.Forms.Button CancelB;
    }
}

