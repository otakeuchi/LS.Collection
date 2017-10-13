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
            this.dgvShops = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvShops)).BeginInit();
            this.SuspendLayout();
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
            this.dgvShops.Size = new System.Drawing.Size(440, 309);
            this.dgvShops.TabIndex = 0;
            this.dgvShops.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShops_CellClick);
            this.dgvShops.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvShops_CellValueChanged);
            // 
            // frmShops
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 333);
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

        private System.Windows.Forms.DataGridView dgvShops;
    }
}