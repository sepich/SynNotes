namespace SynNotes
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Node1");
      System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Node2");
      System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Node3");
      System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Node4");
      System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13});
      System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Node6");
      System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Node7");
      System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Node8");
      System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Node5", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16,
            treeNode17});
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
      this.statusBar = new System.Windows.Forms.StatusStrip();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.scEdit = new ScintillaNET.Scintilla();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
      this.contextMenuTray = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
      this.statusText2 = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
      this.toolStripContainer1.ContentPanel.SuspendLayout();
      this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
      this.toolStripContainer1.SuspendLayout();
      this.statusBar.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.scEdit)).BeginInit();
      this.menuStrip1.SuspendLayout();
      this.contextMenuTray.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStripContainer1
      // 
      // 
      // toolStripContainer1.BottomToolStripPanel
      // 
      this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusBar);
      // 
      // toolStripContainer1.ContentPanel
      // 
      this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
      this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1014, 598);
      this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
      this.toolStripContainer1.Name = "toolStripContainer1";
      this.toolStripContainer1.Size = new System.Drawing.Size(1014, 646);
      this.toolStripContainer1.TabIndex = 0;
      this.toolStripContainer1.Text = "toolStripContainer1";
      // 
      // toolStripContainer1.TopToolStripPanel
      // 
      this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
      // 
      // statusBar
      // 
      this.statusBar.Dock = System.Windows.Forms.DockStyle.None;
      this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusText,
            this.statusText2});
      this.statusBar.Location = new System.Drawing.Point(0, 0);
      this.statusBar.Name = "statusBar";
      this.statusBar.Size = new System.Drawing.Size(1014, 24);
      this.statusBar.TabIndex = 2;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.treeView1);
      this.splitContainer1.Panel1.Controls.Add(this.comboBox1);
      this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.scEdit);
      this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.splitContainer1.Size = new System.Drawing.Size(1014, 598);
      this.splitContainer1.SplitterDistance = 219;
      this.splitContainer1.TabIndex = 0;
      // 
      // treeView1
      // 
      this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView1.Indent = 19;
      this.treeView1.Location = new System.Drawing.Point(0, 21);
      this.treeView1.Name = "treeView1";
      treeNode10.Name = "Node1";
      treeNode10.Text = "Node1";
      treeNode11.Name = "Node2";
      treeNode11.Text = "Node2";
      treeNode12.Name = "Node3";
      treeNode12.Text = "Node3";
      treeNode13.Name = "Node4";
      treeNode13.Text = "Node4";
      treeNode14.Name = "Node0";
      treeNode14.Text = "Node0";
      treeNode15.Name = "Node6";
      treeNode15.Text = "Node6";
      treeNode16.Name = "Node7";
      treeNode16.Text = "Node7";
      treeNode17.Name = "Node8";
      treeNode17.Text = "Node8";
      treeNode18.Name = "Node5";
      treeNode18.Text = "Node5";
      this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode14,
            treeNode18});
      this.treeView1.ShowLines = false;
      this.treeView1.Size = new System.Drawing.Size(219, 577);
      this.treeView1.TabIndex = 1;
      // 
      // comboBox1
      // 
      this.comboBox1.Dock = System.Windows.Forms.DockStyle.Top;
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Location = new System.Drawing.Point(0, 0);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(219, 21);
      this.comboBox1.TabIndex = 2;
      // 
      // scEdit
      // 
      this.scEdit.AutoComplete.IsCaseSensitive = false;
      this.scEdit.AutoComplete.ListString = "";
      this.scEdit.AutoComplete.MaxHeight = 10;
      this.scEdit.Caret.CurrentLineBackgroundAlpha = 50;
      this.scEdit.Caret.CurrentLineBackgroundColor = System.Drawing.Color.GreenYellow;
      this.scEdit.Caret.HighlightCurrentLine = true;
      this.scEdit.Caret.Width = 2;
      this.scEdit.ConfigurationManager.Language = "html";
      this.scEdit.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scEdit.Folding.MarkerScheme = ScintillaNET.FoldMarkerScheme.Arrow;
      this.scEdit.Indentation.ShowGuides = true;
      this.scEdit.Indentation.TabWidth = 2;
      this.scEdit.Indentation.UseTabs = false;
      this.scEdit.LineWrapping.IndentMode = ScintillaNET.LineWrappingIndentMode.Indent;
      this.scEdit.LineWrapping.IndentSize = 1;
      this.scEdit.LineWrapping.Mode = ScintillaNET.LineWrappingMode.Word;
      this.scEdit.Location = new System.Drawing.Point(0, 0);
      this.scEdit.Name = "scEdit";
      this.scEdit.Size = new System.Drawing.Size(791, 598);
      this.scEdit.Styles.BraceBad.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.BraceLight.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.CallTip.FontName = "Segoe UI\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.ControlChar.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.Default.BackColor = System.Drawing.SystemColors.Window;
      this.scEdit.Styles.Default.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.IndentGuide.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.LastPredefined.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.LineNumber.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.Styles.Max.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
      this.scEdit.TabIndex = 0;
      // 
      // menuStrip1
      // 
      this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(1014, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // openToolStripMenuItem
      // 
      this.openToolStripMenuItem.Name = "openToolStripMenuItem";
      this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.openToolStripMenuItem.Text = "Open";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      // 
      // notifyIcon1
      // 
      this.notifyIcon1.ContextMenuStrip = this.contextMenuTray;
      this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
      this.notifyIcon1.Text = "SynNotes";
      this.notifyIcon1.Visible = true;
      this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
      // 
      // contextMenuTray
      // 
      this.contextMenuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem1});
      this.contextMenuTray.Name = "contextMenuTray";
      this.contextMenuTray.Size = new System.Drawing.Size(104, 54);
      // 
      // openToolStripMenuItem1
      // 
      this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
      this.openToolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
      this.openToolStripMenuItem1.Text = "Show";
      this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(100, 6);
      // 
      // exitToolStripMenuItem1
      // 
      this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
      this.exitToolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
      this.exitToolStripMenuItem1.Text = "Exit";
      this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
      // 
      // statusText
      // 
      this.statusText.Name = "statusText";
      this.statusText.Size = new System.Drawing.Size(868, 19);
      this.statusText.Spring = true;
      this.statusText.Text = "statusText";
      this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // statusText2
      // 
      this.statusText2.AutoSize = false;
      this.statusText2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
      this.statusText2.Name = "statusText2";
      this.statusText2.Size = new System.Drawing.Size(100, 19);
      this.statusText2.Text = "statusText2";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1014, 646);
      this.Controls.Add(this.toolStripContainer1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Form1";
      this.Text = "SynNotes";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
      this.Resize += new System.EventHandler(this.Form1_Resize);
      this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
      this.toolStripContainer1.ContentPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.PerformLayout();
      this.toolStripContainer1.ResumeLayout(false);
      this.toolStripContainer1.PerformLayout();
      this.statusBar.ResumeLayout(false);
      this.statusBar.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.scEdit)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.contextMenuTray.ResumeLayout(false);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.StatusStrip statusBar;
        private ScintillaNET.Scintilla scEdit;
        private System.Windows.Forms.ContextMenuStrip contextMenuTray;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private System.Windows.Forms.ToolStripStatusLabel statusText2;
    }
}

