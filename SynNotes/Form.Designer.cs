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
      this.fancyRenderer = new SynNotes.FancyRenderer();
      this.cSort = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.treeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.imageList1 = new System.Windows.Forms.ImageList(this.components);
      this.statusBar = new System.Windows.Forms.StatusStrip();
      this.btnAdd = new System.Windows.Forms.ToolStripDropDownButton();
      this.btnPin = new System.Windows.Forms.ToolStripDropDownButton();
      this.statusText = new System.Windows.Forms.ToolStripStatusLabel();
      this.btnSync = new System.Windows.Forms.ToolStripDropDownButton();
      this.tbSearch = new System.Windows.Forms.TextBox();
      this.panelFind = new System.Windows.Forms.Panel();
      this.tbFind = new System.Windows.Forms.TextBox();
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
      this.panelFind.SuspendLayout();
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
      this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
      this.splitContainer1.Panel1.Controls.Add(this.tree);
      this.splitContainer1.Panel1.Controls.Add(this.statusBar);
      this.splitContainer1.Panel1.Controls.Add(this.tbSearch);
      this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.panelFind);
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
      this.pictureBox1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
      this.pictureBox1.Size = new System.Drawing.Size(16, 19);
      this.pictureBox1.TabIndex = 5;
      this.pictureBox1.TabStop = false;
      // 
      // tree
      // 
      this.tree.AllColumns.Add(this.cName);
      this.tree.AllColumns.Add(this.cSort);
      this.tree.AlternateRowBackColor = System.Drawing.SystemColors.Window;
      this.tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tree.BackColor = System.Drawing.SystemColors.Control;
      this.tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.tree.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.SingleClick;
      this.tree.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cName});
      this.tree.ContextMenuStrip = this.treeMenu;
      this.tree.CopySelectionOnControlC = false;
      this.tree.EmptyListMsg = "";
      this.tree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.tree.FullRowSelect = true;
      this.tree.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.tree.HideSelection = false;
      this.tree.HighlightBackgroundColor = System.Drawing.SystemColors.Highlight;
      this.tree.HighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
      this.tree.IsSearchOnSortColumn = false;
      this.tree.Location = new System.Drawing.Point(0, 23);
      this.tree.Margin = new System.Windows.Forms.Padding(0);
      this.tree.Name = "tree";
      this.tree.OwnerDraw = true;
      this.tree.SelectAllOnControlA = false;
      this.tree.ShowGroups = false;
      this.tree.ShowHeaderInAllViews = false;
      this.tree.Size = new System.Drawing.Size(227, 602);
      this.tree.SmallImageList = this.imageList1;
      this.tree.TabIndex = 3;
      this.tree.TabStop = false;
      this.tree.UnfocusedHighlightBackgroundColor = System.Drawing.SystemColors.Highlight;
      this.tree.UnfocusedHighlightForegroundColor = System.Drawing.SystemColors.HighlightText;
      this.tree.UseAlternatingBackColors = true;
      this.tree.UseCompatibleStateImageBehavior = false;
      this.tree.UseNotifyPropertyChanged = true;
      this.tree.UseOverlays = false;
      this.tree.View = System.Windows.Forms.View.Details;
      this.tree.VirtualMode = true;
      this.tree.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.tree_CellEditFinishing);
      this.tree.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.tree_CellEditStarting);
      this.tree.CellEditValidating += new BrightIdeasSoftware.CellEditEventHandler(this.tree_CellEditValidating);
      this.tree.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.tree_CellClick);
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
      this.cName.Renderer = this.fancyRenderer;
      this.cName.Text = "Name";
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
      this.treeMenu.Size = new System.Drawing.Size(61, 4);
      // 
      // imageList1
      // 
      this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
      this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
      this.imageList1.Images.SetKeyName(0, "add");
      this.imageList1.Images.SetKeyName(1, "settings");
      this.imageList1.Images.SetKeyName(2, "open.png");
      this.imageList1.Images.SetKeyName(3, "close");
      this.imageList1.Images.SetKeyName(4, "all.png");
      this.imageList1.Images.SetKeyName(5, "trash.png");
      this.imageList1.Images.SetKeyName(6, "pinsmall.png");
      this.imageList1.Images.SetKeyName(7, "pinsmall_i.png");
      this.imageList1.Images.SetKeyName(8, "open_i.png");
      this.imageList1.Images.SetKeyName(9, "close_i.png");
      // 
      // statusBar
      // 
      this.statusBar.BackColor = System.Drawing.SystemColors.Control;
      this.statusBar.GripMargin = new System.Windows.Forms.Padding(0);
      this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnPin,
            this.statusText,
            this.btnSync});
      this.statusBar.Location = new System.Drawing.Point(0, 624);
      this.statusBar.Name = "statusBar";
      this.statusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
      this.statusBar.ShowItemToolTips = true;
      this.statusBar.Size = new System.Drawing.Size(227, 22);
      this.statusBar.SizingGrip = false;
      this.statusBar.TabIndex = 3;
      // 
      // btnAdd
      // 
      this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
      this.btnAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnAdd.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
      this.btnAdd.MergeIndex = -2;
      this.btnAdd.Name = "btnAdd";
      this.btnAdd.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
      this.btnAdd.ShowDropDownArrow = false;
      this.btnAdd.Size = new System.Drawing.Size(20, 21);
      this.btnAdd.Text = "Add Note (F7)";
      this.btnAdd.Click += new System.EventHandler(this.btnAdd_ButtonClick);
      // 
      // btnPin
      // 
      this.btnPin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnPin.Image = ((System.Drawing.Image)(resources.GetObject("btnPin.Image")));
      this.btnPin.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnPin.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnPin.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
      this.btnPin.Name = "btnPin";
      this.btnPin.ShowDropDownArrow = false;
      this.btnPin.Size = new System.Drawing.Size(20, 21);
      this.btnPin.Text = "Pin Note";
      this.btnPin.Click += new System.EventHandler(this.btnPin_Click);
      // 
      // statusText
      // 
      this.statusText.Name = "statusText";
      this.statusText.Size = new System.Drawing.Size(152, 17);
      this.statusText.Spring = true;
      // 
      // btnSync
      // 
      this.btnSync.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.btnSync.BackColor = System.Drawing.SystemColors.Control;
      this.btnSync.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSync.DoubleClickEnabled = true;
      this.btnSync.Image = ((System.Drawing.Image)(resources.GetObject("btnSync.Image")));
      this.btnSync.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnSync.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSync.Margin = new System.Windows.Forms.Padding(0, 1, 0, 0);
      this.btnSync.Name = "btnSync";
      this.btnSync.ShowDropDownArrow = false;
      this.btnSync.Size = new System.Drawing.Size(20, 21);
      this.btnSync.Text = "Sync (RightClick for Settings)";
      this.btnSync.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnSync_MouseDown);
      // 
      // tbSearch
      // 
      this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbSearch.BackColor = System.Drawing.SystemColors.Window;
      this.tbSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.tbSearch.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.tbSearch.ForeColor = System.Drawing.SystemColors.GrayText;
      this.tbSearch.Location = new System.Drawing.Point(16, 0);
      this.tbSearch.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
      this.tbSearch.MaxLength = 255;
      this.tbSearch.Name = "tbSearch";
      this.tbSearch.Size = new System.Drawing.Size(211, 19);
      this.tbSearch.TabIndex = 1;
      this.tbSearch.Tag = "";
      this.tbSearch.Text = "Search Notes";
      this.tbSearch.TextChanged += new System.EventHandler(this.cbSearch_TextChanged);
      this.tbSearch.Enter += new System.EventHandler(this.cbSearch_Enter);
      this.tbSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbSearch_KeyDown);
      this.tbSearch.Leave += new System.EventHandler(this.cbSearch_Leave);
      // 
      // panelFind
      // 
      this.panelFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.panelFind.AutoSize = true;
      this.panelFind.Controls.Add(this.tbFind);
      this.panelFind.Location = new System.Drawing.Point(567, 0);
      this.panelFind.Name = "panelFind";
      this.panelFind.Padding = new System.Windows.Forms.Padding(3, 0, 0, 3);
      this.panelFind.Size = new System.Drawing.Size(200, 27);
      this.panelFind.TabIndex = 3;
      this.panelFind.Visible = false;
      // 
      // tbFind
      // 
      this.tbFind.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.tbFind.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbFind.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.tbFind.Location = new System.Drawing.Point(3, 0);
      this.tbFind.Name = "tbFind";
      this.tbFind.Size = new System.Drawing.Size(197, 19);
      this.tbFind.TabIndex = 0;
      this.tbFind.TextChanged += new System.EventHandler(this.tbFind_TextChanged);
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
      this.scEdit.AutoCDropRestOfWord = true;
      this.scEdit.AutoCIgnoreCase = true;
      this.scEdit.AutoCMaxHeight = 10;
      this.scEdit.AutomaticFold = ((ScintillaNET.AutomaticFold)(((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click) 
            | ScintillaNET.AutomaticFold.Change)));
      this.scEdit.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.scEdit.CaretLineBackColor = System.Drawing.Color.LightYellow;
      this.scEdit.CaretLineVisible = true;
      this.scEdit.CaretWidth = 2;
      this.scEdit.IndentationGuides = ScintillaNET.IndentView.Real;
      this.scEdit.Location = new System.Drawing.Point(0, 0);
      this.scEdit.Margin = new System.Windows.Forms.Padding(0);
      this.scEdit.MouseSelectionRectangularSwitch = true;
      this.scEdit.Name = "scEdit";
      this.scEdit.Size = new System.Drawing.Size(783, 624);
      this.scEdit.TabIndex = 2;
      this.scEdit.TabWidth = 2;
      this.scEdit.UseTabs = false;
      this.scEdit.WrapIndentMode = ScintillaNET.WrapIndentMode.Indent;
      this.scEdit.WrapMode = ScintillaNET.WrapMode.Word;
      this.scEdit.WrapVisualFlags = ScintillaNET.WrapVisualFlags.End;
      this.scEdit.SavePointLeft += new System.EventHandler<System.EventArgs>(this.scEdit_SavePointLeft);
      this.scEdit.UpdateUI += new System.EventHandler<ScintillaNET.UpdateUIEventArgs>(this.scEdit_UpdateUI);
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
      this.Shown += new System.EventHandler(this.Form1_Shown);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
      this.Move += new System.EventHandler(this.Form1_Move);
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
      this.panelFind.ResumeLayout(false);
      this.panelFind.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private ScintillaNET.Scintilla scEdit;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusText;
        private BrightIdeasSoftware.TreeListView tree;
        private BrightIdeasSoftware.OLVColumn cName;
        private System.Windows.Forms.ToolStripDropDownButton btnAdd;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tagBox;
        private BrightIdeasSoftware.OLVColumn cSort;
        private System.Windows.Forms.ContextMenuStrip treeMenu;
        private System.Windows.Forms.Label btnLexer;
        private System.Windows.Forms.ContextMenuStrip lexerMenu;
        private System.Windows.Forms.ToolStripDropDownButton btnPin;
        private FancyRenderer fancyRenderer;
        private System.Windows.Forms.ToolStripDropDownButton btnSync;
        private System.Windows.Forms.Panel panelFind;
        private System.Windows.Forms.TextBox tbFind;
    }
}

