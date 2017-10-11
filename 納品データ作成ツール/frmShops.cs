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

        private ShopController C;
        private string nr = Environment.NewLine;
        private LogWriter log;
        private CommonError err;

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
                C = new ShopController(ss);
            }
            catch (Exception ex)
            {
                err.execute(ex.Message, LogWriter.TYPE_ER, true, log);
                return;
            }

            try
            {
                dgvShops.Columns.Add(CreateTextCol("Code", "ShopCode", "店舗コード"));
                dgvShops.Columns.Add(CreateTextCol("Name", "ShopName", "店舗名称"));
                dgvShops.Columns.Add(CreateTextCol("Order", "ShopOrder", "並び"));
                dgvShops.Columns["ShopOrder"].Visible = false; // 並び順は非表示
                dgvShops.Columns.Add(CreateImageCol("Delete", "削除", trs));
                dgvShops.Columns.Add(CreateImageCol("Up", "上へ", aru));
                dgvShops.Columns.Add(CreateImageCol("Down", "下へ", ard));

                BindingList<shop> ds = new BindingList<shop>(C.ss.List);
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
    }
}
