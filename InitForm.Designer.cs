namespace Database_Control
{
    partial class InitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitForm));
            label1 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            groupBox1 = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            ConnectBtn = new Button();
            QuitBtn = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            splitContainer1 = new SplitContainer();
            label2 = new Label();
            FilePath = new TextBox();
            tableLayoutPanel4 = new TableLayoutPanel();
            ServerOptions = new ComboBox();
            RESET = new Button();
            tableLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(48, 0);
            label1.Name = "label1";
            label1.Size = new Size(357, 56);
            label1.TabIndex = 0;
            label1.Text = "Enter Maestro Server Name";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.Controls.Add(label1, 1, 0);
            tableLayoutPanel1.Controls.Add(groupBox1, 1, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.23572F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 36.45681F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 30.30747F));
            tableLayoutPanel1.Size = new Size(454, 170);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tableLayoutPanel2);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(48, 119);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(357, 49);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(ConnectBtn, 0, 0);
            tableLayoutPanel2.Controls.Add(QuitBtn, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel2.Location = new Point(3, 18);
            tableLayoutPanel2.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(351, 29);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // ConnectBtn
            // 
            ConnectBtn.ForeColor = SystemColors.ActiveCaptionText;
            ConnectBtn.Location = new Point(3, 2);
            ConnectBtn.Margin = new Padding(3, 2, 3, 2);
            ConnectBtn.Name = "ConnectBtn";
            ConnectBtn.Size = new Size(82, 22);
            ConnectBtn.TabIndex = 0;
            ConnectBtn.Text = "Connect";
            ConnectBtn.UseVisualStyleBackColor = true;
            ConnectBtn.Click += ConnectBtn_Click;
            // 
            // QuitBtn
            // 
            QuitBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            QuitBtn.ForeColor = SystemColors.ActiveCaptionText;
            QuitBtn.Location = new Point(266, 2);
            QuitBtn.Margin = new Padding(3, 2, 3, 2);
            QuitBtn.Name = "QuitBtn";
            QuitBtn.Size = new Size(82, 22);
            QuitBtn.TabIndex = 1;
            QuitBtn.Text = "Quit";
            QuitBtn.UseVisualStyleBackColor = true;
            QuitBtn.Click += QuitBtn_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 18F));
            tableLayoutPanel3.Controls.Add(splitContainer1, 0, 1);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel3.Location = new Point(48, 58);
            tableLayoutPanel3.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(357, 57);
            tableLayoutPanel3.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(3, 30);
            splitContainer1.Margin = new Padding(3, 2, 3, 2);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(label2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(FilePath);
            splitContainer1.Size = new Size(351, 25);
            splitContainer1.SplitterDistance = 178;
            splitContainer1.TabIndex = 1;
            splitContainer1.SplitterMoved += splitContainer1_SplitterMoved;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(135, 15);
            label2.TabIndex = 0;
            label2.Text = "File Containing Options:";
            // 
            // FilePath
            // 
            FilePath.Dock = DockStyle.Fill;
            FilePath.Location = new Point(0, 0);
            FilePath.Margin = new Padding(3, 2, 3, 2);
            FilePath.Name = "FilePath";
            FilePath.ReadOnly = true;
            FilePath.Size = new Size(169, 23);
            FilePath.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 78.41191F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.58809F));
            tableLayoutPanel4.Controls.Add(ServerOptions, 0, 0);
            tableLayoutPanel4.Controls.Add(RESET, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel4.Location = new Point(3, 2);
            tableLayoutPanel4.Margin = new Padding(3, 2, 3, 2);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 15F));
            tableLayoutPanel4.Size = new Size(351, 24);
            tableLayoutPanel4.TabIndex = 2;
            // 
            // ServerOptions
            // 
            ServerOptions.Dock = DockStyle.Fill;
            ServerOptions.FormattingEnabled = true;
            ServerOptions.Location = new Point(3, 2);
            ServerOptions.Margin = new Padding(3, 2, 3, 2);
            ServerOptions.Name = "ServerOptions";
            ServerOptions.Size = new Size(269, 23);
            ServerOptions.TabIndex = 0;
            // 
            // RESET
            // 
            RESET.ForeColor = SystemColors.ActiveCaptionText;
            RESET.Location = new Point(278, 2);
            RESET.Margin = new Padding(3, 2, 3, 2);
            RESET.Name = "RESET";
            RESET.Size = new Size(70, 20);
            RESET.TabIndex = 1;
            RESET.Text = "RESET";
            RESET.UseVisualStyleBackColor = true;
            RESET.Click += RESET_Click;
            // 
            // InitForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(454, 170);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            ForeColor = SystemColors.ControlLightLight;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(470, 209);
            Name = "InitForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Maestro";
            Load += InitForm_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            groupBox1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox1;
        private TableLayoutPanel tableLayoutPanel2;
        private Button ConnectBtn;
        private Button QuitBtn;
        private TableLayoutPanel tableLayoutPanel3;
        private ComboBox ServerOptions;
        private SplitContainer splitContainer1;
        private Label label2;
        private TextBox FilePath;
        private TableLayoutPanel tableLayoutPanel4;
        private Button RESET;
    }
}