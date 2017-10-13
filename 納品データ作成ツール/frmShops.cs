using EOSTools.Common;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace 納品データ作成ツール
{

    public partial class frmShops : Form
    {
        public shops ss { set; private get; }

        private string nr = Environment.NewLine;
        private LogWriter log;
        private CommonError err;

        private string ColCode = "ColCode";
        private string ColName = "ColName";
        private string ColOrder = "ColOrder";
        private string ColDelete = "ColDelete";
        private string ColUp = "ColUp";
        private string ColDown = "ColDown";


        private void frmShops_Load(object sender, EventArgs e)
        {
            // ウィンドウの位置・サイズを復元
            Bounds = Properties.Settings.Default.BoundsSetting;
            WindowState = Properties.Settings.Default.WindowStateSetting;
        }

        private void frmShops_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ウィンドウの位置・サイズを保存
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.BoundsSetting = Bounds;
            }
            else
            {
                Properties.Settings.Default.BoundsSetting = RestoreBounds;
            }
            Properties.Settings.Default.WindowStateSetting = WindowState;
            Properties.Settings.Default.Save();
        }

        public frmShops(shops ss):base()
        {
            InitializeComponent();

            log = new LogWriter("./log", "log.txt");
            err = new CommonError();
            log.write(LogWriter.TYPE_EV, "起動‥‥");

            // アイコンをDGVにセット
            Icon trs = new Icon("./ゴミ箱.ico");
            Icon aru = new Icon("./うえ矢印.ico");
            Icon ard = new Icon("./した矢印.ico");

            try
            {
                dgvShops.Columns.Add(CreateTextCol("Code", ColCode, "店舗コード"));
                dgvShops.Columns.Add(CreateTextCol("Name", ColName, "店舗名称"));
                dgvShops.Columns.Add(CreateTextCol("Order", ColOrder, "並び"));
                dgvShops.Columns[ColOrder].Visible = false; // 並び順は非表示
                dgvShops.Columns.Add(CreateImageCol(ColDelete, "削除", trs));
                dgvShops.Columns.Add(CreateImageCol(ColUp, "上へ", aru));
                dgvShops.Columns.Add(CreateImageCol(ColDown, "下へ", ard));

                BindingList<shop> ds = new BindingList<shop>(ss.List);
                dgvShops.DataSource = ds;
            }
            catch (Exception ex)
            {
                err.execute("データグリッドビューの初期化に失敗しました。"
                    + nr + ex.Message, LogWriter.TYPE_ER, true, log);
                return;
            }
        }

        private DataGridViewTextBoxColumn CreateTextCol(string src, string name, string headertext)
        {
            var col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = src;
            col.Name = name;
            col.HeaderText = headertext;
            return col;
        }

        private DataGridViewImageColumn CreateImageCol(string name, string headertext, Icon i)
        {
            var col = new DataGridViewImageColumn();
            col.Name = name;
            col.HeaderText = headertext;
            col.Image = i.ToBitmap();
            col.Width = 50;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            col.ImageLayout = DataGridViewImageCellLayout.Zoom;

            col.DefaultCellStyle.NullValue = null;

            return col;
        }

        private void dgvShops_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string col = dgvShops.Columns[e.ColumnIndex].Name;
            if (col == ColCode || col == ColName) dgvShops.BeginEdit(true); // 編集モードに
        }

        private void dgvShops_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string col = dgvShops.Columns[e.ColumnIndex].Name;
            if (col == ColCode)
            {
                if((string)dgvShops[e.ColumnIndex + 1 , e.RowIndex].Value == "" 
                    || dgvShops[e.ColumnIndex + 1, e.RowIndex].Value == null) // 名称が未入力の場合
                {
                    dgvShops[e.ColumnIndex + 1, e.RowIndex].Selected = true;
                    dgvShops.BeginEdit(true);
                }
            }


        }
    }
}
