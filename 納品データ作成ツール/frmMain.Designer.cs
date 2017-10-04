namespace 納品データ作成ツール
{
    partial class frmMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.grpOutDir = new System.Windows.Forms.GroupBox();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.btnSelectDIr = new System.Windows.Forms.Button();
            this.grpFileName = new System.Windows.Forms.GroupBox();
            this.lblExt = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.grpBatchList = new System.Windows.Forms.GroupBox();
            this.dgvBatch = new System.Windows.Forms.DataGridView();
            this.BatchNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShopName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewImageColumn();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.btnExec = new System.Windows.Forms.Button();
            this.pbr = new System.Windows.Forms.ProgressBar();
            this.grpExecute = new System.Windows.Forms.GroupBox();
            this.grpOutDir.SuspendLayout();
            this.grpFileName.SuspendLayout();
            this.grpBatchList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatch)).BeginInit();
            this.grpExecute.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpOutDir
            // 
            this.grpOutDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOutDir.Controls.Add(this.txtDir);
            this.grpOutDir.Controls.Add(this.btnSelectDIr);
            this.grpOutDir.Location = new System.Drawing.Point(17, 10);
            this.grpOutDir.Name = "grpOutDir";
            this.grpOutDir.Size = new System.Drawing.Size(645, 69);
            this.grpOutDir.TabIndex = 0;
            this.grpOutDir.TabStop = false;
            this.grpOutDir.Text = "①データを出力するフォルダを選択して下さい";
            // 
            // txtDir
            // 
            this.txtDir.Enabled = false;
            this.txtDir.Location = new System.Drawing.Point(84, 18);
            this.txtDir.Multiline = true;
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(555, 41);
            this.txtDir.TabIndex = 1;
            // 
            // btnSelectDIr
            // 
            this.btnSelectDIr.Location = new System.Drawing.Point(6, 18);
            this.btnSelectDIr.Name = "btnSelectDIr";
            this.btnSelectDIr.Size = new System.Drawing.Size(66, 21);
            this.btnSelectDIr.TabIndex = 0;
            this.btnSelectDIr.Text = "選択";
            this.btnSelectDIr.UseVisualStyleBackColor = true;
            this.btnSelectDIr.Click += new System.EventHandler(this.btnSelectDIr_Click);
            // 
            // grpFileName
            // 
            this.grpFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFileName.Controls.Add(this.lblExt);
            this.grpFileName.Controls.Add(this.txtFile);
            this.grpFileName.Location = new System.Drawing.Point(18, 91);
            this.grpFileName.Name = "grpFileName";
            this.grpFileName.Size = new System.Drawing.Size(644, 47);
            this.grpFileName.TabIndex = 1;
            this.grpFileName.TabStop = false;
            this.grpFileName.Text = "②納品するExcelのファイル名を指定してください(拡張子不要)";
            // 
            // lblExt
            // 
            this.lblExt.AutoSize = true;
            this.lblExt.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblExt.Location = new System.Drawing.Point(158, 21);
            this.lblExt.Name = "lblExt";
            this.lblExt.Size = new System.Drawing.Size(42, 13);
            this.lblExt.TabIndex = 1;
            this.lblExt.Text = ".xslx";
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(5, 18);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(151, 19);
            this.txtFile.TabIndex = 0;
            this.txtFile.TextChanged += new System.EventHandler(this.txtFile_TextChanged);
            // 
            // grpBatchList
            // 
            this.grpBatchList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBatchList.Controls.Add(this.dgvBatch);
            this.grpBatchList.Location = new System.Drawing.Point(17, 151);
            this.grpBatchList.Name = "grpBatchList";
            this.grpBatchList.Size = new System.Drawing.Size(635, 259);
            this.grpBatchList.TabIndex = 2;
            this.grpBatchList.TabStop = false;
            this.grpBatchList.Text = "③↓目検済ファイル(CSV)をドラッグアンドドロップして下さい。その後、店舗名称をリストから選択して下さい。↓";
            // 
            // dgvBatch
            // 
            this.dgvBatch.AllowDrop = true;
            this.dgvBatch.AllowUserToAddRows = false;
            this.dgvBatch.AllowUserToDeleteRows = false;
            this.dgvBatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBatch.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.BatchNo,
            this.ShopName,
            this.Delete});
            this.dgvBatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBatch.Location = new System.Drawing.Point(3, 15);
            this.dgvBatch.MultiSelect = false;
            this.dgvBatch.Name = "dgvBatch";
            this.dgvBatch.RowHeadersVisible = false;
            this.dgvBatch.RowTemplate.Height = 21;
            this.dgvBatch.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBatch.Size = new System.Drawing.Size(629, 241);
            this.dgvBatch.TabIndex = 0;
            this.dgvBatch.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBatch_CellClick);
            this.dgvBatch.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBatch_CellEnter);
            this.dgvBatch.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBatch_CellValueChanged);
            this.dgvBatch.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvBatch_CurrentCellDirtyStateChanged);
            this.dgvBatch.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvBatch_DragDrop);
            this.dgvBatch.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvBatch_DragEnter);
            // 
            // BatchNo
            // 
            this.BatchNo.HeaderText = "バッチ番号";
            this.BatchNo.MinimumWidth = 150;
            this.BatchNo.Name = "BatchNo";
            this.BatchNo.ReadOnly = true;
            this.BatchNo.Width = 150;
            // 
            // ShopName
            // 
            this.ShopName.HeaderText = "店舗名称";
            this.ShopName.MinimumWidth = 200;
            this.ShopName.Name = "ShopName";
            this.ShopName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ShopName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ShopName.Width = 250;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "削除";
            this.Delete.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.Delete.Name = "Delete";
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // btnExec
            // 
            this.btnExec.Location = new System.Drawing.Point(6, 18);
            this.btnExec.Name = "btnExec";
            this.btnExec.Size = new System.Drawing.Size(144, 22);
            this.btnExec.TabIndex = 3;
            this.btnExec.Text = "実行";
            this.btnExec.UseVisualStyleBackColor = true;
            this.btnExec.Click += new System.EventHandler(this.btnExec_Click);
            // 
            // pbr
            // 
            this.pbr.Location = new System.Drawing.Point(166, 18);
            this.pbr.Name = "pbr";
            this.pbr.Size = new System.Drawing.Size(465, 22);
            this.pbr.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbr.TabIndex = 4;
            // 
            // grpExecute
            // 
            this.grpExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grpExecute.Controls.Add(this.pbr);
            this.grpExecute.Controls.Add(this.btnExec);
            this.grpExecute.Location = new System.Drawing.Point(18, 429);
            this.grpExecute.Name = "grpExecute";
            this.grpExecute.Size = new System.Drawing.Size(635, 50);
            this.grpExecute.TabIndex = 5;
            this.grpExecute.TabStop = false;
            this.grpExecute.Text = "④実行ボタンを押して下さい";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 491);
            this.Controls.Add(this.grpExecute);
            this.Controls.Add(this.grpBatchList);
            this.Controls.Add(this.grpFileName);
            this.Controls.Add(this.grpOutDir);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(680, 500);
            this.Name = "frmMain";
            this.Text = "レイジースーザン納品データ作成";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.grpOutDir.ResumeLayout(false);
            this.grpOutDir.PerformLayout();
            this.grpFileName.ResumeLayout(false);
            this.grpFileName.PerformLayout();
            this.grpBatchList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatch)).EndInit();
            this.grpExecute.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpOutDir;
        private System.Windows.Forms.GroupBox grpFileName;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Button btnSelectDIr;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.GroupBox grpBatchList;
        private System.Windows.Forms.DataGridView dgvBatch;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.Label lblExt;
        private System.Windows.Forms.Button btnExec;
        private System.Windows.Forms.ProgressBar pbr;
        private System.Windows.Forms.GroupBox grpExecute;
        private System.Windows.Forms.DataGridViewTextBoxColumn BatchNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn ShopName;
        private System.Windows.Forms.DataGridViewImageColumn Delete;
    }
}

