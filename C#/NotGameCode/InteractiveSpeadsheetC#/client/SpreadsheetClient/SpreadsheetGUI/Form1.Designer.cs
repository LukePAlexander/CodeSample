using SS;

namespace SpreadsheetGUI
{
    partial class SpreadsheetForm
    {
        #region ---------------   GUI GLOBAL VARIABLES  -----------------
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel cellNameLabel;
        private System.Windows.Forms.ToolStripTextBox textBoxCellContent;
        private System.Windows.Forms.ToolStripLabel cellValueLabel;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        #endregion

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
        


        #region ----------------   Initialize   ------------------
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoCtrlZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoCtrlYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeColorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.changeColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.undoButton = new System.Windows.Forms.ToolStripLabel();
            this.redoButton = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cellNameLabel = new System.Windows.Forms.ToolStripLabel();
            this.textBoxCellContent = new System.Windows.Forms.ToolStripTextBox();
            this.cellValueLabel = new System.Windows.Forms.ToolStripLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.formulaErrorDisplay = new System.Windows.Forms.ToolStripStatusLabel();
            this.updateCellProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.showCollaboratorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collaboratorsPanel = new SS.Collaborators();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(692, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.newToolStripMenuItem.Text = "New              Ctrl+N";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.openToolStripMenuItem.Text = "Open            Ctrl+O";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveToolStripMenuItem.Text = "Save              Ctrl+S";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(173, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoCtrlZToolStripMenuItem,
            this.redoCtrlYToolStripMenuItem,
            this.changeColorToolStripMenuItem,
            this.changeColorToolStripMenuItem1,
            this.showCollaboratorsToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // undoCtrlZToolStripMenuItem
            // 
            this.undoCtrlZToolStripMenuItem.Name = "undoCtrlZToolStripMenuItem";
            this.undoCtrlZToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.undoCtrlZToolStripMenuItem.Text = "Undo     Ctrl+Z";
            this.undoCtrlZToolStripMenuItem.Click += new System.EventHandler(this.undo_Event);
            // 
            // redoCtrlYToolStripMenuItem
            // 
            this.redoCtrlYToolStripMenuItem.Name = "redoCtrlYToolStripMenuItem";
            this.redoCtrlYToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.redoCtrlYToolStripMenuItem.Text = "Redo      Ctrl+Y";
            this.redoCtrlYToolStripMenuItem.Click += new System.EventHandler(this.redo_Event);
            // 
            // changeColorToolStripMenuItem
            // 
            this.changeColorToolStripMenuItem.Name = "changeColorToolStripMenuItem";
            this.changeColorToolStripMenuItem.Size = new System.Drawing.Size(174, 6);
            // 
            // changeColorToolStripMenuItem1
            // 
            this.changeColorToolStripMenuItem1.Name = "changeColorToolStripMenuItem1";
            this.changeColorToolStripMenuItem1.Size = new System.Drawing.Size(177, 22);
            this.changeColorToolStripMenuItem1.Text = "Change Color";
            this.changeColorToolStripMenuItem1.Click += new System.EventHandler(this.ChooseColor_Event);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoButton,
            this.redoButton,
            this.toolStripSeparator2,
            this.cellNameLabel,
            this.textBoxCellContent,
            this.cellValueLabel});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(1, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStrip1.Size = new System.Drawing.Size(691, 28);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip2";
            // 
            // undoButton
            // 
            this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoButton.Enabled = false;
            this.undoButton.Image = global::SpreadsheetGUI.Properties.Resources.LeftArrow;
            this.undoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoButton.Margin = new System.Windows.Forms.Padding(0, 1, 5, 2);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(20, 25);
            this.undoButton.Text = "toolStripButton1";
            this.undoButton.ToolTipText = "Undo (Ctrl+Z)";
            this.undoButton.Click += new System.EventHandler(this.undo_Event);
            // 
            // redoButton
            // 
            this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoButton.Enabled = false;
            this.redoButton.Image = global::SpreadsheetGUI.Properties.Resources.RightArrow;
            this.redoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoButton.Name = "redoButton";
            this.redoButton.Size = new System.Drawing.Size(20, 25);
            this.redoButton.Text = "toolStripButton2";
            this.redoButton.ToolTipText = "Redo (Ctrl+Y)";
            this.redoButton.Click += new System.EventHandler(this.redo_Event);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // cellNameLabel
            // 
            this.cellNameLabel.AutoSize = false;
            this.cellNameLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.cellNameLabel.Name = "cellNameLabel";
            this.cellNameLabel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.cellNameLabel.Size = new System.Drawing.Size(32, 25);
            this.cellNameLabel.Text = "A1";
            // 
            // textBoxCellContent
            // 
            this.textBoxCellContent.AcceptsReturn = true;
            this.textBoxCellContent.AcceptsTab = true;
            this.textBoxCellContent.Name = "textBoxCellContent";
            this.textBoxCellContent.Size = new System.Drawing.Size(400, 28);
            this.textBoxCellContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCellContent_KeyDown);
            this.textBoxCellContent.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxCellContent_KeyUp);
            this.textBoxCellContent.TextChanged += new System.EventHandler(this.textBoxCellContent_TextChanged);
            // 
            // cellValueLabel
            // 
            this.cellValueLabel.Name = "cellValueLabel";
            this.cellValueLabel.Size = new System.Drawing.Size(41, 25);
            this.cellValueLabel.Text = "Value: ";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.formulaErrorDisplay,
            this.updateCellProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 471);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(692, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // formulaErrorDisplay
            // 
            this.formulaErrorDisplay.ForeColor = System.Drawing.Color.Red;
            this.formulaErrorDisplay.Name = "formulaErrorDisplay";
            this.formulaErrorDisplay.Size = new System.Drawing.Size(0, 17);
            // 
            // updateCellProgress
            // 
            this.updateCellProgress.Name = "updateCellProgress";
            this.updateCellProgress.Size = new System.Drawing.Size(300, 16);
            this.updateCellProgress.Visible = false;
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.spreadsheetPanel1.Location = new System.Drawing.Point(-1, 55);
            this.spreadsheetPanel1.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(693, 413);
            this.spreadsheetPanel1.TabIndex = 0;
            this.spreadsheetPanel1.SelectionChanged += new SS.SelectionChangedHandler(this.CellFocusChanged);
            this.spreadsheetPanel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.spreadsheetPanel1_MouseWheel);
            // 
            // showCollaboratorsToolStripMenuItem
            // 
            this.showCollaboratorsToolStripMenuItem.Name = "showCollaboratorsToolStripMenuItem";
            this.showCollaboratorsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showCollaboratorsToolStripMenuItem.Text = "Show Collaborators";
            this.showCollaboratorsToolStripMenuItem.Click += new System.EventHandler(this.ToggleCollab_Event);
            // 
            // collaboratorsPanel
            // 
            this.collaboratorsPanel.Location = new System.Drawing.Point(0, 55);
            this.collaboratorsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.collaboratorsPanel.Name = "collaboratorsPanel";
            this.collaboratorsPanel.Size = new System.Drawing.Size(692, 30);
            this.collaboratorsPanel.TabIndex = 5;
            this.collaboratorsPanel.Visible = false;
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 493);
            this.Controls.Add(this.collaboratorsPanel);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(624, 298);
            this.Name = "SpreadsheetForm";
            this.Text = "Spreadsheet";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel formulaErrorDisplay;
        private System.Windows.Forms.ToolStripProgressBar updateCellProgress;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoCtrlZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoCtrlYToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel undoButton;
        private System.Windows.Forms.ToolStripLabel redoButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator changeColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeColorToolStripMenuItem1;
        private Collaborators collaboratorsPanel;
        private System.Windows.Forms.ToolStripMenuItem showCollaboratorsToolStripMenuItem;
    }
}

