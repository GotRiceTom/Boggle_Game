namespace SpreadsheetGUI
{
    partial class Spreadsheet_V2
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hellpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CenterPanel = new System.Windows.Forms.Panel();
            this.spreadsheetPanel1 = new SSGui.SpreadsheetPanel();
            this.TopPanelOfMain = new System.Windows.Forms.Panel();
            this.CellValueText = new System.Windows.Forms.TextBox();
            this.CellContentText = new System.Windows.Forms.TextBox();
            this.CellNameText = new System.Windows.Forms.TextBox();
            this.CellValueLabel = new System.Windows.Forms.Label();
            this.CellContentLabel = new System.Windows.Forms.Label();
            this.CellNameLabel = new System.Windows.Forms.Label();
            this.ChangeButton = new System.Windows.Forms.Button();
            this.ContentEditTextBox = new System.Windows.Forms.TextBox();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.CenterPanel.SuspendLayout();
            this.TopPanelOfMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.hellpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1139, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWindow,
            this.openToolStripMenuItem,
            this.closeWindow,
            this.SaveToItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newWindow
            // 
            this.newWindow.Name = "newWindow";
            this.newWindow.Size = new System.Drawing.Size(152, 22);
            this.newWindow.Text = "New";
            this.newWindow.Click += new System.EventHandler(this.NewSpreadsheet_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeWindow
            // 
            this.closeWindow.Name = "closeWindow";
            this.closeWindow.Size = new System.Drawing.Size(152, 22);
            this.closeWindow.Text = "Close";
            this.closeWindow.Click += new System.EventHandler(this.CloseSpreadsheet_Click);
            // 
            // SaveToItem
            // 
            this.SaveToItem.Name = "SaveToItem";
            this.SaveToItem.Size = new System.Drawing.Size(152, 22);
            this.SaveToItem.Text = "Save To...";
            this.SaveToItem.Click += new System.EventHandler(this.SaveToItem_Click);
            // 
            // hellpToolStripMenuItem
            // 
            this.hellpToolStripMenuItem.Name = "hellpToolStripMenuItem";
            this.hellpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.hellpToolStripMenuItem.Text = "Help";
            this.hellpToolStripMenuItem.Click += new System.EventHandler(this.helpEvent_Click);
            // 
            // CenterPanel
            // 
            this.CenterPanel.Controls.Add(this.spreadsheetPanel1);
            this.CenterPanel.Controls.Add(this.TopPanelOfMain);
            this.CenterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CenterPanel.Location = new System.Drawing.Point(0, 0);
            this.CenterPanel.Name = "CenterPanel";
            this.CenterPanel.Size = new System.Drawing.Size(1139, 615);
            this.CenterPanel.TabIndex = 1;
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spreadsheetPanel1.Location = new System.Drawing.Point(0, 111);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(1139, 504);
            this.spreadsheetPanel1.TabIndex = 3;
            this.spreadsheetPanel1.SelectionChanged += new SSGui.SelectionChangedHandler(this.Display_selectionchange);
            // 
            // TopPanelOfMain
            // 
            this.TopPanelOfMain.Controls.Add(this.CellValueText);
            this.TopPanelOfMain.Controls.Add(this.CellContentText);
            this.TopPanelOfMain.Controls.Add(this.CellNameText);
            this.TopPanelOfMain.Controls.Add(this.CellValueLabel);
            this.TopPanelOfMain.Controls.Add(this.CellContentLabel);
            this.TopPanelOfMain.Controls.Add(this.CellNameLabel);
            this.TopPanelOfMain.Controls.Add(this.ChangeButton);
            this.TopPanelOfMain.Controls.Add(this.ContentEditTextBox);
            this.TopPanelOfMain.Controls.Add(this.menuStrip1);
            this.TopPanelOfMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanelOfMain.Location = new System.Drawing.Point(0, 0);
            this.TopPanelOfMain.Name = "TopPanelOfMain";
            this.TopPanelOfMain.Size = new System.Drawing.Size(1139, 111);
            this.TopPanelOfMain.TabIndex = 2;
            // 
            // CellValueText
            // 
            this.CellValueText.Location = new System.Drawing.Point(644, 75);
            this.CellValueText.Name = "CellValueText";
            this.CellValueText.Size = new System.Drawing.Size(150, 20);
            this.CellValueText.TabIndex = 4;
            // 
            // CellContentText
            // 
            this.CellContentText.Location = new System.Drawing.Point(644, 45);
            this.CellContentText.Name = "CellContentText";
            this.CellContentText.Size = new System.Drawing.Size(150, 20);
            this.CellContentText.TabIndex = 3;
            // 
            // CellNameText
            // 
            this.CellNameText.Location = new System.Drawing.Point(644, 13);
            this.CellNameText.Name = "CellNameText";
            this.CellNameText.Size = new System.Drawing.Size(150, 20);
            this.CellNameText.TabIndex = 2;
            // 
            // CellValueLabel
            // 
            this.CellValueLabel.AutoSize = true;
            this.CellValueLabel.Location = new System.Drawing.Point(574, 78);
            this.CellValueLabel.Name = "CellValueLabel";
            this.CellValueLabel.Size = new System.Drawing.Size(54, 13);
            this.CellValueLabel.TabIndex = 7;
            this.CellValueLabel.Text = "Cell Value";
            // 
            // CellContentLabel
            // 
            this.CellContentLabel.AutoSize = true;
            this.CellContentLabel.Location = new System.Drawing.Point(574, 48);
            this.CellContentLabel.Name = "CellContentLabel";
            this.CellContentLabel.Size = new System.Drawing.Size(64, 13);
            this.CellContentLabel.TabIndex = 6;
            this.CellContentLabel.Text = "Cell Content";
            // 
            // CellNameLabel
            // 
            this.CellNameLabel.AutoSize = true;
            this.CellNameLabel.Location = new System.Drawing.Point(574, 16);
            this.CellNameLabel.Name = "CellNameLabel";
            this.CellNameLabel.Size = new System.Drawing.Size(55, 13);
            this.CellNameLabel.TabIndex = 5;
            this.CellNameLabel.Text = "Cell Name";
            // 
            // ChangeButton
            // 
            this.ChangeButton.Location = new System.Drawing.Point(4, 37);
            this.ChangeButton.Name = "ChangeButton";
            this.ChangeButton.Size = new System.Drawing.Size(78, 41);
            this.ChangeButton.TabIndex = 1;
            this.ChangeButton.Text = "Change Content";
            this.ChangeButton.UseVisualStyleBackColor = true;
            this.ChangeButton.Click += new System.EventHandler(this.ChangeButton_Click);
            // 
            // ContentEditTextBox
            // 
            this.ContentEditTextBox.Location = new System.Drawing.Point(6, 78);
            this.ContentEditTextBox.Name = "ContentEditTextBox";
            this.ContentEditTextBox.Size = new System.Drawing.Size(470, 20);
            this.ContentEditTextBox.TabIndex = 0;
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.Save_Click);
            // 
            // Spreadsheet_V2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1139, 615);
            this.Controls.Add(this.CenterPanel);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Spreadsheet_V2";
            this.Text = "Spreadsheet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Spreadsheet_V2_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Spreadsheet_V2_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.CenterPanel.ResumeLayout(false);
            this.TopPanelOfMain.ResumeLayout(false);
            this.TopPanelOfMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newWindow;
        private System.Windows.Forms.ToolStripMenuItem closeWindow;
        private System.Windows.Forms.ToolStripMenuItem hellpToolStripMenuItem;
        private System.Windows.Forms.Panel CenterPanel;
        private SSGui.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.Panel TopPanelOfMain;
        private System.Windows.Forms.Label CellValueLabel;
        private System.Windows.Forms.Label CellContentLabel;
        private System.Windows.Forms.Label CellNameLabel;
        private System.Windows.Forms.TextBox CellValueText;
        private System.Windows.Forms.TextBox CellContentText;
        private System.Windows.Forms.TextBox CellNameText;
        private System.Windows.Forms.Button ChangeButton;
        private System.Windows.Forms.TextBox ContentEditTextBox;
        private System.Windows.Forms.ToolStripMenuItem SaveToItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    }
}

