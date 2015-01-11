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
              hook.Dispose();
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
      System.Windows.Forms.ContextMenuStrip trayMenu;
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.tree = new BrightIdeasSoftware.TreeListView();
      this.cName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.cDate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.cSort = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.treeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.statusBar = new System.Windows.Forms.StatusStrip();
      this.btnAdd = new System.Windows.Forms.ToolStripSplitButton();
      this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
      this.cbSearch = new System.Windows.Forms.ComboBox();
      this.btnLexer = new System.Windows.Forms.Label();
      this.tagBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.scEdit = new ScintillaNET.Scintilla();
      this.lexerMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
      trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      trayMenu.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tree)).BeginInit();
      this.statusBar.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.scEdit)).BeginInit();
      this.SuspendLayout();
      // 
      // trayMenu
      // 
      trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem1});
      trayMenu.Name = "contextMenuTray";
      trayMenu.Size = new System.Drawing.Size(104, 54);
      // 
      // openToolStripMenuItem1
      // 
      this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
      this.openToolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
      this.openToolStripMenuItem1.Text = "Show";
      this.openToolStripMenuItem1.Click += new System.EventHandler(this.showToolStripMenuItem1_Click);
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
      // splitContainer1
      // 
      this.splitContainer1.BackColor = System.Drawing.Color.Gainsboro;
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
      this.splitContainer1.Panel1.Controls.Add(this.tree);
      this.splitContainer1.Panel1.Controls.Add(this.statusBar);
      this.splitContainer1.Panel1.Controls.Add(this.cbSearch);
      this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.btnLexer);
      this.splitContainer1.Panel2.Controls.Add(this.tagBox);
      this.splitContainer1.Panel2.Controls.Add(this.label1);
      this.splitContainer1.Panel2.Controls.Add(this.scEdit);
      this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.splitContainer1.Size = new System.Drawing.Size(1014, 646);
      this.splitContainer1.SplitterDistance = 227;
      this.splitContainer1.TabIndex = 0;
      this.splitContainer1.TabStop = false;
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
      this.pictureBox1.ErrorImage = null;
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.InitialImage = null;
      this.pictureBox1.Location = new System.Drawing.Point(0, 0);
      this.pictureBox1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
      this.pictureBox1.Size = new System.Drawing.Size(16, 26);
      this.pictureBox1.TabIndex = 5;
      this.pictureBox1.TabStop = false;
      // 
      // tree
      // 
      this.tree.AllColumns.Add(this.cName);
      this.tree.AllColumns.Add(this.cDate);
      this.tree.AllColumns.Add(this.cSort);
      this.tree.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
      this.tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tree.BackColor = System.Drawing.SystemColors.Control;
      this.tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.tree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
      this.tree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cName});
      this.tree.ContextMenuStrip = this.treeMenu;
      this.tree.EmptyListMsg = "";
      this.tree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.tree.FullRowSelect = true;
      this.tree.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.tree.HideSelection = false;
      this.tree.IsSearchOnSortColumn = false;
      this.tree.Location = new System.Drawing.Point(0, 30);
      this.tree.Margin = new System.Windows.Forms.Padding(0);
      this.tree.Name = "tree";
      this.tree.OwnerDraw = true;
      this.tree.ShowGroups = false;
      this.tree.ShowHeaderInAllViews = false;
      this.tree.Size = new System.Drawing.Size(227, 594);
      this.tree.SmallImageList = this.imageList1;
      this.tree.TabIndex = 2;
      this.tree.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.Highlight;
      this.tree.UnfocusedHighlightForegroundColor = System.Drawing.Color.White;
      this.tree.UseAlternatingBackColors = true;
      this.tree.UseCompatibleStateImageBehavior = false;
      this.tree.UseNotifyPropertyChanged = true;
      this.tree.UseOverlays = false;
      this.tree.View = System.Windows.Forms.View.Details;
      this.tree.VirtualMode = true;
      this.tree.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.tree_CellEditFinishing);
      this.tree.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.tree_CellEditStarting);
      this.tree.CellEditValidating += new BrightIdeasSoftware.CellEditEventHandler(this.tree_CellEditValidating);
      this.tree.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.tree_CellRightClick);
      this.tree.ModelCanDrop += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.tree_ModelCanDrop);
      this.tree.ModelDropped += new System.EventHandler<BrightIdeasSoftware.ModelDropEventArgs>(this.tree_ModelDropped);
      this.tree.SelectionChanged += new System.EventHandler(this.tree_SelectionChanged);
      this.tree.ItemActivate += new System.EventHandler(this.tree_ItemActivate);
      this.tree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tree_ItemDrag);
      this.tree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tree_MouseClick);
      // 
      // cName
      // 
      this.cName.AspectName = "Name";
      this.cName.AutoCompleteEditor = false;
      this.cName.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
      this.cName.FillsFreeSpace = true;
      this.cName.Text = "Name";
      // 
      // cDate
      // 
      this.cDate.AspectName = "";
      this.cDate.AutoCompleteEditor = false;
      this.cDate.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
      this.cDate.DisplayIndex = 1;
      this.cDate.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.cDate.IsEditable = false;
      this.cDate.IsVisible = false;
      this.cDate.Text = "Date";
      this.cDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.cDate.UseFiltering = false;
      this.cDate.Width = 30;
      // 
      // cSort
      // 
      this.cSort.AspectName = "";
      this.cSort.AutoCompleteEditor = false;
      this.cSort.AutoCompleteEditorMode = System.Windows.Forms.AutoCompleteMode.None;
      this.cSort.DisplayIndex = 2;
      this.cSort.IsEditable = false;
      this.cSort.IsVisible = false;
      this.cSort.Searchable = false;
      this.cSort.Text = "Sort";
      this.cSort.UseFiltering = false;
      this.cSort.Width = 30;
      // 
      // treeMenu
      // 
      this.treeMenu.Name = "treeMenu";
      this.treeMenu.ShowImageMargin = false;
      this.treeMenu.Size = new System.Drawing.Size(36, 4);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "add.png");
      this.imageList1.Images.SetKeyName(1, "search.png");
      this.imageList1.Images.SetKeyName(2, "settings.png");
      // 
      // statusBar
      // 
      this.statusBar.BackColor = System.Drawing.SystemColors.Control;
      this.statusBar.GripMargin = new System.Windows.Forms.Padding(0);
      this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.statusText});
      this.statusBar.Location = new System.Drawing.Point(0, 624);
      this.statusBar.Name = "statusBar";
      this.statusBar.ShowItemToolTips = true;
      this.statusBar.Size = new System.Drawing.Size(227, 22);
      this.statusBar.SizingGrip = false;
      this.statusBar.TabIndex = 3;
      // 
      // btnAdd
      // 
      this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnAdd.DropDownButtonWidth = 0;
      this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
      this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnAdd.Margin = new System.Windows.Forms.Padding(0);
      this.btnAdd.MergeIndex = -2;
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Size = new System.Drawing.Size(21, 22);
      this.btnAdd.Text = "Add Note";
      this.btnAdd.ButtonClick += new System.EventHandler(this.btnAdd_ButtonClick);
      // 
      // statusText
      // 
      this.statusText.Name = "statusText";
      this.statusText.Size = new System.Drawing.Size(191, 17);
      this.statusText.Spring = true;
      this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // cbSearch
      // 
      this.cbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cbSearch.BackColor = System.Drawing.SystemColors.Window;
      this.cbSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.cbSearch.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.cbSearch.ForeColor = System.Drawing.SystemColors.GrayText;
      this.cbSearch.IntegralHeight = false;
      this.cbSearch.ItemHeight = 18;
      this.cbSearch.Location = new System.Drawing.Point(16, 0);
      this.cbSearch.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
      this.cbSearch.Name = "cbSearch";
      this.cbSearch.Size = new System.Drawing.Size(211, 26);
      this.cbSearch.TabIndex = 1;
      this.cbSearch.Tag = "hint";
      this.cbSearch.Text = "Search Notes";
      this.cbSearch.TextChanged += new System.EventHandler(this.cbSearch_TextChanged);
      this.cbSearch.Enter += new System.EventHandler(this.cbSearch_Enter);
      this.cbSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbSearch_KeyDown);
      this.cbSearch.Leave += new System.EventHandler(this.cbSearch_Leave);
      // 
      // btnLexer
      // 
      this.btnLexer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnLexer.BackColor = System.Drawing.SystemColors.Window;
      this.btnLexer.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btnLexer.Location = new System.Drawing.Point(683, 623);
      this.btnLexer.Margin = new System.Windows.Forms.Padding(0);
      this.btnLexer.Name = "btnLexer";
      this.btnLexer.Size = new System.Drawing.Size(100, 24);
      this.btnLexer.TabIndex = 0;
      this.btnLexer.Text = "Lexer";
      this.btnLexer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.btnLexer.UseMnemonic = false;
      this.btnLexer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.btnLexer_Click);
      // 
      // tagBox
      // 
      this.tagBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tagBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
      this.tagBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
      this.tagBox.BackColor = System.Drawing.SystemColors.Window;
      this.tagBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.tagBox.Location = new System.Drawing.Point(37, 630);
      this.tagBox.Margin = new System.Windows.Forms.Padding(0);
      this.tagBox.Name = "tagBox";
      this.tagBox.Size = new System.Drawing.Size(646, 13);
      this.tagBox.TabIndex = 1;
      this.tagBox.TabStop = false;
      this.tagBox.TextChanged += new System.EventHandler(this.tagBox_TextChanged);
      this.tagBox.Enter += new System.EventHandler(this.tagBox_Enter);
      this.tagBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tagBox_KeyDown);
      // 
      // label1
      // 
      this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label1.BackColor = System.Drawing.SystemColors.Window;
      this.label1.Location = new System.Drawing.Point(0, 624);
      this.label1.Margin = new System.Windows.Forms.Padding(0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(683, 22);
      this.label1.TabIndex = 2;
      this.label1.Text = "Tags:";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // scEdit
      // 
      this.scEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scEdit.AutoComplete.IsCaseSensitive = false;
      this.scEdit.AutoComplete.ListString = "";
      this.scEdit.AutoComplete.MaxHeight = 10;
      this.scEdit.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.scEdit.Caret.CurrentLineBackgroundAlpha = 50;
      this.scEdit.Caret.CurrentLineBackgroundColor = System.Drawing.Color.GreenYellow;
      this.scEdit.Caret.HighlightCurrentLine = true;
      this.scEdit.Caret.Width = 2;
      this.scEdit.ConfigurationManager.IsBuiltInEnabled = false;
      this.scEdit.ConfigurationManager.IsUserEnabled = false;
      this.scEdit.ConfigurationManager.Language = "html";
      this.scEdit.ConfigurationManager.UseXmlReader = false;
      this.scEdit.Folding.MarkerScheme = ScintillaNET.FoldMarkerScheme.Arrow;
      this.scEdit.Indentation.ShowGuides = true;
      this.scEdit.Indentation.SmartIndentType = ScintillaNET.SmartIndent.CPP;
      this.scEdit.Indentation.TabWidth = 2;
      this.scEdit.Indentation.UseTabs = false;
      this.scEdit.IsBraceMatching = true;
      this.scEdit.LineWrapping.IndentMode = ScintillaNET.LineWrappingIndentMode.Indent;
      this.scEdit.LineWrapping.IndentSize = 1;
      this.scEdit.LineWrapping.Mode = ScintillaNET.LineWrappingMode.Word;
      this.scEdit.LineWrapping.VisualFlags = ScintillaNET.LineWrappingVisualFlags.End;
      this.scEdit.Location = new System.Drawing.Point(0, 0);
      this.scEdit.Margin = new System.Windows.Forms.Padding(0);
      this.scEdit.Margins.Margin1.Width = 0;
      this.scEdit.Margins.Margin2.Width = 16;
      this.scEdit.Name = "scEdit";
      this.scEdit.Size = new System.Drawing.Size(783, 624);
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
      this.scEdit.TabIndex = 3;
      this.scEdit.SelectionChanged += new System.EventHandler(this.scEdit_SelectionChanged);
      // 
      // lexerMenu
      // 
      this.lexerMenu.Name = "lexerMenu";
      this.lexerMenu.ShowCheckMargin = true;
      this.lexerMenu.ShowImageMargin = false;
      this.lexerMenu.Size = new System.Drawing.Size(61, 4);
      this.lexerMenu.Opening += new System.ComponentModel.CancelEventHandler(this.lexerMenu_Opening);
      // 
      // notifyIcon1
      // 
      this.notifyIcon1.ContextMenuStrip = trayMenu;
      this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
      this.notifyIcon1.Text = "SynNotes";
      this.notifyIcon1.Visible = true;
      this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1014, 646);
      this.Controls.Add(this.splitContainer1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Name = "Form1";
      this.Text = "SynNotes";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
      this.Resize += new System.EventHandler(this.Form1_Resize);
      trayMenu.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tree)).EndInit();
      this.statusBar.ResumeLayout(false);
      this.statusBar.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.scEdit)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private ScintillaNET.Scintilla scEdit;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ComboBox cbSearch;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private BrightIdeasSoftware.TreeListView tree;
        private BrightIdeasSoftware.OLVColumn cName;
        private BrightIdeasSoftware.OLVColumn cDate;
        private System.Windows.Forms.ToolStripSplitButton btnAdd;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tagBox;
        private BrightIdeasSoftware.OLVColumn cSort;
        private System.Windows.Forms.ContextMenuStrip treeMenu;
        private System.Windows.Forms.Label btnLexer;
        private System.Windows.Forms.ContextMenuStrip lexerMenu;
    }
}

