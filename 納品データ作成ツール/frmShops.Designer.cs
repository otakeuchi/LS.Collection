namespace 納品データ作成ツール
{
    partial class frmShops
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShops));
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvShops = new 納品データ作成ツール.DataGridViewEx();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShops)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(392, 379);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 29);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dgvShops
            // 
            this.dgvShops.AllowUserToDeleteRows = false;
            this.dgvShops.AllowUserToResizeRows = false;
            this.dgvShops.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvShops.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvShops.Location = new System.Drawing.Point(12, 12);
            this.dgvShops.Name = "dgvShops";
            this.dgvShops.RowHeadersVisible = false;
            this.dgvShops.RowTemplate.Height = 21;
            this.dgvShops.Size = new System.Drawing.Size(490, 361);
            this.dgvShops.TabIndex = 0;
            this.dgvShops.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShops_CellClick);
            this.dgvShops.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShops_CellEnter);
            this.dgvShops.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvShop_CellFormatting);
            this.dgvShops.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvShops_CellValidating);
            this.dgvShops.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShops_CellValueChanged);
            this.dgvShops.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShops_RowValidated);
            this.dgvShops.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvShops_RowValidating);
            this.dgvShops.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvShops_KeyDown);
            // 
            // frmShops
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 415);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvShops);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmShops";
            this.Text = "店舗情報の編集";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmShops_FormClosing);
            this.Load += new System.EventHandler(this.frmShops_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvShops)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewEx dgvShops;
        private System.Windows.Forms.Button btnClose;
    }
}