namespace View
{
    partial class Form1
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
            this.serverLabel = new System.Windows.Forms.Label();
            this.serverAddressTextBox = new System.Windows.Forms.TextBox();
            this.startGameButton = new System.Windows.Forms.Button();
            this.playerNameLabel = new System.Windows.Forms.Label();
            this.playerNameTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.scorePanel = new ScorePanel.ScorePanel();
            this.startPanel = new StartPanel.StartPanel();
            this.snakeWorldPanel1 = new SnakeWorldPanel.SnakeWorldPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Font = new System.Drawing.Font("OCR A Std", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverLabel.Location = new System.Drawing.Point(54, 420);
            this.serverLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(106, 22);
            this.serverLabel.TabIndex = 0;
            this.serverLabel.Text = "Server";
            // 
            // serverAddressTextBox
            // 
            this.serverAddressTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverAddressTextBox.Location = new System.Drawing.Point(59, 451);
            this.serverAddressTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.serverAddressTextBox.Name = "serverAddressTextBox";
            this.serverAddressTextBox.Size = new System.Drawing.Size(153, 26);
            this.serverAddressTextBox.TabIndex = 1;
            this.serverAddressTextBox.Text = "localhost";
            // 
            // startGameButton
            // 
            this.startGameButton.BackColor = System.Drawing.Color.Red;
            this.startGameButton.Font = new System.Drawing.Font("OCR A Std", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startGameButton.Location = new System.Drawing.Point(181, 486);
            this.startGameButton.Margin = new System.Windows.Forms.Padding(2);
            this.startGameButton.Name = "startGameButton";
            this.startGameButton.Size = new System.Drawing.Size(196, 44);
            this.startGameButton.TabIndex = 3;
            this.startGameButton.Text = "Start";
            this.startGameButton.UseVisualStyleBackColor = false;
            this.startGameButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // playerNameLabel
            // 
            this.playerNameLabel.AutoSize = true;
            this.playerNameLabel.Font = new System.Drawing.Font("OCR A Std", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerNameLabel.Location = new System.Drawing.Point(346, 420);
            this.playerNameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.playerNameLabel.Name = "playerNameLabel";
            this.playerNameLabel.Size = new System.Drawing.Size(186, 22);
            this.playerNameLabel.TabIndex = 3;
            this.playerNameLabel.Text = "Player Name";
            // 
            // playerNameTextBox
            // 
            this.playerNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerNameTextBox.Location = new System.Drawing.Point(351, 451);
            this.playerNameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.playerNameTextBox.Name = "playerNameTextBox";
            this.playerNameTextBox.Size = new System.Drawing.Size(152, 26);
            this.playerNameTextBox.TabIndex = 2;
            this.playerNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerNameTextBox_KeyDown);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.scorePanel);
            this.panel1.Location = new System.Drawing.Point(622, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 545);
            this.panel1.TabIndex = 7;
            // 
            // scorePanel
            // 
            this.scorePanel.Location = new System.Drawing.Point(0, 0);
            this.scorePanel.Name = "scorePanel";
            this.scorePanel.Size = new System.Drawing.Size(295, 545);
            this.scorePanel.TabIndex = 0;
            this.scorePanel.Visible = false;
            // 
            // startPanel
            // 
            this.startPanel.BackColor = System.Drawing.Color.DarkGreen;
            this.startPanel.Location = new System.Drawing.Point(0, 0);
            this.startPanel.Margin = new System.Windows.Forms.Padding(2);
            this.startPanel.Name = "startPanel";
            this.startPanel.Size = new System.Drawing.Size(600, 400);
            this.startPanel.TabIndex = 0;
            // 
            // snakeWorldPanel1
            // 
            this.snakeWorldPanel1.Location = new System.Drawing.Point(0, 0);
            this.snakeWorldPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.snakeWorldPanel1.Name = "snakeWorldPanel1";
            this.snakeWorldPanel1.Size = new System.Drawing.Size(400, 400);
            this.snakeWorldPanel1.TabIndex = 6;
            this.snakeWorldPanel1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.ClientSize = new System.Drawing.Size(600, 541);
            this.Controls.Add(this.startPanel);
            this.Controls.Add(this.playerNameTextBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.snakeWorldPanel1);
            this.Controls.Add(this.playerNameLabel);
            this.Controls.Add(this.serverLabel);
            this.Controls.Add(this.serverAddressTextBox);
            this.Controls.Add(this.startGameButton);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(616, 580);
            this.MinimumSize = new System.Drawing.Size(616, 580);
            this.Name = "Form1";
            this.Text = "Snake";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.TextBox serverAddressTextBox;
        private System.Windows.Forms.Button startGameButton;
        private System.Windows.Forms.Label playerNameLabel;
        private System.Windows.Forms.TextBox playerNameTextBox;
        private SnakeWorldPanel.SnakeWorldPanel snakeWorldPanel1;
        private StartPanel.StartPanel startPanel;
        private System.Windows.Forms.Panel panel1;
        private ScorePanel.ScorePanel scorePanel;
    }
}

