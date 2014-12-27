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
      System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Node1");
      System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Node2");
      System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Node3");
      System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Node4");
      System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Node0", new System.Windows.Forms.TreeNode[] {
            treeNode28,
            treeNode29,
            treeNode30,
            treeNode31});
      System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Node6");
      System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Node7");
      System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Node8");
      System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Node5", new System.Windows.Forms.TreeNode[] {
            treeNode33,
            treeNode34,
            treeNode35});
      this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
      this.scEdit = new ScintillaNET.Scintilla();
      this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
      this.toolStripContainer1.ContentPanel.SuspendLayout();
      this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
      this.toolStripContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.scEdit)).BeginInit();
      this.SuspendLayout();
      // 
      // toolStripContainer1
      // 
      // 
      // toolStripContainer1.BottomToolStripPanel
      // 
      this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
      // 
      // toolStripContainer1.ContentPanel
      // 
      this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
      this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1014, 600);
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
      // statusStrip1
      // 
      this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
      this.statusStrip1.Location = new System.Drawing.Point(0, 0);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(1014, 22);
      this.statusStrip1.TabIndex = 2;
      this.statusStrip1.Text = "statusStrip1";
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
      this.splitContainer1.Size = new System.Drawing.Size(1014, 600);
      this.splitContainer1.SplitterDistance = 219;
      this.splitContainer1.TabIndex = 0;
      // 
      // treeView1
      // 
      this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.treeView1.Indent = 19;
      this.treeView1.Location = new System.Drawing.Point(0, 21);
      this.treeView1.Name = "treeView1";
      treeNode28.Name = "Node1";
      treeNode28.Text = "Node1";
      treeNode29.Name = "Node2";
      treeNode29.Text = "Node2";
      treeNode30.Name = "Node3";
      treeNode30.Text = "Node3";
      treeNode31.Name = "Node4";
      treeNode31.Text = "Node4";
      treeNode32.Name = "Node0";
      treeNode32.Text = "Node0";
      treeNode33.Name = "Node6";
      treeNode33.Text = "Node6";
      treeNode34.Name = "Node7";
      treeNode34.Text = "Node7";
      treeNode35.Name = "Node8";
      treeNode35.Text = "Node8";
      treeNode36.Name = "Node5";
      treeNode36.Text = "Node5";
      this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode32,
            treeNode36});
      this.treeView1.ShowLines = false;
      this.treeView1.Size = new System.Drawing.Size(219, 579);
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
      this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
      this.openToolStripMenuItem.Text = "Open";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      // 
      // notifyIcon1
      // 
      this.notifyIcon1.Text = "notifyIcon1";
      this.notifyIcon1.Visible = true;
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
      this.scEdit.Size = new System.Drawing.Size(791, 600);
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
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1014, 646);
      this.Controls.Add(this.toolStripContainer1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Form1";
      this.Text = "SynNotes";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
      this.toolStripContainer1.ContentPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
      this.toolStripContainer1.TopToolStripPanel.PerformLayout();
      this.toolStripContainer1.ResumeLayout(false);
      this.toolStripContainer1.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.scEdit)).EndInit();
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
        private System.Windows.Forms.StatusStrip statusStrip1;
        private ScintillaNET.Scintilla scEdit;
    }
}

