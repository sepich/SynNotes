namespace SynNotes {
  partial class FormSync {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.linkCreate = new System.Windows.Forms.LinkLabel();
      this.tbEmail = new System.Windows.Forms.TextBox();
      this.tbPass = new System.Windows.Forms.TextBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.cbFreq = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // linkCreate
      // 
      this.linkCreate.AutoSize = true;
      this.linkCreate.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
      this.linkCreate.Location = new System.Drawing.Point(67, 95);
      this.linkCreate.Name = "linkCreate";
      this.linkCreate.Size = new System.Drawing.Size(103, 13);
      this.linkCreate.TabIndex = 5;
      this.linkCreate.Text = "Create new account";
      this.linkCreate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkCreate_LinkClicked);
      // 
      // tbEmail
      // 
      this.tbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbEmail.Location = new System.Drawing.Point(70, 15);
      this.tbEmail.Name = "tbEmail";
      this.tbEmail.Size = new System.Drawing.Size(162, 20);
      this.tbEmail.TabIndex = 1;
      this.tbEmail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbEmail_KeyDown);
      // 
      // tbPass
      // 
      this.tbPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbPass.Location = new System.Drawing.Point(70, 41);
      this.tbPass.Name = "tbPass";
      this.tbPass.Size = new System.Drawing.Size(162, 20);
      this.tbPass.TabIndex = 2;
      this.tbPass.UseSystemPasswordChar = true;
      // 
      // btnSave
      // 
      this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnSave.Location = new System.Drawing.Point(70, 127);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(88, 23);
      this.btnSave.TabIndex = 4;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(8, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(32, 13);
      this.label1.TabIndex = 5;
      this.label1.Text = "Email";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(8, 44);
      this.label2.Margin = new System.Windows.Forms.Padding(3, 20, 3, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(53, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Password";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(8, 70);
      this.label3.Margin = new System.Windows.Forms.Padding(3, 20, 3, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(57, 13);
      this.label3.TabIndex = 7;
      this.label3.Text = "Frequency";
      // 
      // cbFreq
      // 
      this.cbFreq.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cbFreq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbFreq.FormattingEnabled = true;
      this.cbFreq.Location = new System.Drawing.Point(70, 67);
      this.cbFreq.Name = "cbFreq";
      this.cbFreq.Size = new System.Drawing.Size(162, 21);
      this.cbFreq.TabIndex = 3;
      // 
      // SyncSetup
      // 
      this.AcceptButton = this.btnSave;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(244, 162);
      this.Controls.Add(this.cbFreq);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.tbPass);
      this.Controls.Add(this.tbEmail);
      this.Controls.Add(this.linkCreate);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(260, 201);
      this.Name = "SyncSetup";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Simplenote Account";
      this.TopMost = true;
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.LinkLabel linkCreate;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox cbFreq;
    private System.Windows.Forms.TextBox tbPass;
    private System.Windows.Forms.TextBox tbEmail;
  }
}