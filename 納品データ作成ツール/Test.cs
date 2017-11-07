using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 納品データ作成ツール
{

    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
            dgv.RowValidating -= new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgv_RowValidating);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // テストの準備（Form1 には DataGridView が一つ必要）
            DataTable table = new DataTable();
            table.Columns.Add("Col1", typeof(string));
            table.Columns.Add("Col2", typeof(string));
            for (int i = 1; i <= 5; i++)
                table.Rows.Add(i.ToString("000"), "名称" + i.ToString());
            dgv.DataSource = table;
            foreach (DataGridViewColumn col in dgv.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv.AllowDrop = true;
            dgv.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgv_RowValidating);

        }

        private void dgv_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            string v,v2;
            try
            {
                v = (string)dgv[0, e.RowIndex].Value;
            }
            catch (Exception)
            {
                v = "";
            }

            try
            {
                v2 = (string)dgv[1, e.RowIndex].Value;
            }
            catch (Exception)
            {
                v2 = "";
            }

            int r;
            StringBuilder m = new StringBuilder();
            if (v.Length < 3 || !int.TryParse(v,out r))
            {
                m.AppendLine("コードは三桁の数値で入力してください");
                e.Cancel = true;
            }
            if (v2 == "")
            {
                m.AppendLine("名称は必ず入力してください");
                e.Cancel = true;
            }
            if (m.Length > 0) MessageBox.Show(m.ToString());
        }
    }
}
