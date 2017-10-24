using EOSTools.Common;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace 納品データ作成ツール
{

    public partial class frmShops : Form
    {
        private string nr = Environment.NewLine;
        private LogWriter log;
        private CommonError err;

        public shops ss { set; private get; }
        private BindingList<shop> src;

        private const string ColCode = "ColCode";
        private const string ColName = "ColName";
        private const string ColOrder = "ColOrder";
        private const string ColDelete = "ColDelete";
        private const string ColUp = "ColUp";
        private const string ColDown = "ColDown";

        private Icon trs, aru, ard;

        private bool changed;
        public bool saved { private set; get;}

        #region "コントロールの制御　Win32API"
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// コントロールの再描画を停止させる
        /// </summary>
        /// <param name="control">対象のコントロール</param>
        public static void BeginControlUpdate(Control control)
        {
            SendMessage(new HandleRef(control, control.Handle),
                WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// コントロールの再描画を再開させる
        /// </summary>
        /// <param name="control">対象のコントロール</param>
        public static void EndControlUpdate(Control control)
        {
            SendMessage(new HandleRef(control, control.Handle),
                WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            control.Invalidate();
        }
        #endregion

        #region "初期化"
        public frmShops(shops argss = null):base()
        {

            log = new LogWriter("./log", "log.txt");
            err = new CommonError();
            log.write(LogWriter.TYPE_EV, "起動‥‥");

            // アイコンをDGVにセット
            trs = new Icon("./ゴミ箱.ico");
            aru = new Icon("./うえ矢印.ico");
            ard = new Icon("./した矢印.ico");

            InitializeComponent();

            dgvShops.RowValidating -= new DataGridViewCellCancelEventHandler(dgvShops_RowValidating);
            dgvShops.RowValidated -= new DataGridViewCellEventHandler(dgvShops_RowValidated);
            dgvShops.CellValidating -= new DataGridViewCellValidatingEventHandler(dgvShops_CellValidating);
            dgvShops.CellValueChanged -= new DataGridViewCellEventHandler(dgvShops_CellValueChanged);

            changed = false;
            saved = false;

            BeginControlUpdate(dgvShops);

            try
            {
                dgvShops.Columns.Add(CreateTextCol("Order", ColOrder, "No"));
                dgvShops.Columns.Add(CreateTextCol("Code", ColCode, "店舗コード"));
                dgvShops.Columns.Add(CreateTextCol("Name", ColName, "店舗名称"));
                dgvShops.Columns[ColOrder].ReadOnly = true; // 並び順は編集不可
                //dgvShops.Columns[ColOrder].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells; // ここで設定するとnullrefの例外
                dgvShops.Columns.Add(CreateImageCol(ColDelete, "削除"));
                dgvShops.Columns.Add(CreateImageCol(ColUp, "上へ" ));
                dgvShops.Columns.Add(CreateImageCol(ColDown, "下へ" ));

                if (argss == null)
                {
                    ss = new shops(Properties.Settings.Default.PathDefShops);
                }
                else
                {
                    ss = argss;
                }
                src = new BindingList<shop>(ss.List);
                src.ListChanged -= new ListChangedEventHandler(src_ListChanged);
                dgvShops.DataSource = src;
                src.RaiseListChangedEvents = true;

                src.ListChanged += new ListChangedEventHandler(src_ListChanged);
                dgvShops.Columns[ColOrder].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            }
            catch (Exception ex)
            {
                err.execute("データグリッドビューの初期化に失敗しました。"
                    + nr + ex.Message, LogWriter.TYPE_ER, true, log);
                return;
            }
        }

        private void frmShops_Load(object sender, EventArgs e)
        {
            // ウィンドウの位置・サイズを復元
            Bounds = Properties.Settings.Default.BoundsSetting;
            WindowState = Properties.Settings.Default.WindowStateSetting;

            dgvShops.RowValidating += new DataGridViewCellCancelEventHandler(dgvShops_RowValidating);
            dgvShops.RowValidated += new DataGridViewCellEventHandler(dgvShops_RowValidated);
            dgvShops.CellValidating += new DataGridViewCellValidatingEventHandler(dgvShops_CellValidating);
            dgvShops.CellValueChanged += new DataGridViewCellEventHandler(dgvShops_CellValueChanged);

            src.ListChanged += new ListChangedEventHandler(src_ListChanged);

            EndControlUpdate(dgvShops);
        }

        private void frmShops_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changed)
            {
                DialogResult a = MessageBox.Show("編集内容を保存しますか？", "", MessageBoxButtons.YesNoCancel);
                if(a == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if(a == DialogResult.Yes)
                {
                    ss.serialize();
                    this.changed = false;
                    this.saved = true;
                }
            }

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

        private DataGridViewTextBoxColumn CreateTextCol(string src, string name, string headertext)
        {
            var col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = src;
            col.Name = name;
            col.HeaderText = headertext;
            
            return col;
        }

        private DataGridViewImageColumn CreateImageCol(string name, string headertext, Icon i = null)
        {
            var col = new DataGridViewImageColumn();
            col.CellTemplate = new DataGridViewImageCellEx();
            col.Name = name;
            col.HeaderText = headertext;

            if (i != null) col.Image = i.ToBitmap();
            
            col.Width = 50;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            col.ImageLayout = DataGridViewImageCellLayout.Zoom;

            col.DefaultCellStyle.NullValue = null;

            return col;
        }

        private void dgvShop_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgvShop = (DataGridView)sender;
            string n = dgvShop.Columns[e.ColumnIndex].Name;
            // 上矢印は、2行目から(新規行を除く)
            if (e.RowIndex > 0 && e.RowIndex < dgvShop.Rows.Count - 1)
            {
                if (n == ColUp)
                {
                    e.Value = aru.ToBitmap();
                    e.FormattingApplied = true;
                }
            }

            // 下矢印から1行目から最終行の一つ手前まで（新規行は除く）
            if (e.RowIndex < dgvShop.Rows.Count - 2)
            {
                if (n == ColDown)
                {
                    e.Value = ard.ToBitmap();
                    e.FormattingApplied = true;
                }
            }

            // ゴミ箱は、1行目から最終行のまで（新規行は除く）
            if (e.RowIndex < dgvShop.Rows.Count - 1)
            {
                if (n == ColDelete)
                {
                    e.Value = trs.ToBitmap();
                    e.FormattingApplied = true;
                }
            }
        }
        #endregion

        #region "セルの移動、IMEの設定"
        private void dgvShops_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            switch (dgvShops.Columns[e.ColumnIndex].Name)
            {
                case ColOrder:
                    SendKeys.Send("{Tab}"); // Orderセルはスキップする
                    break;
                case ColCode:
                    dgvShops.ImeMode = ImeMode.Alpha;
                    break;
                case ColName:
                    dgvShops.ImeMode = ImeMode.Hiragana;
                    break;
            }
        }

        private void dgvShops_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (dgvShops.CurrentRow.IsNewRow) return;

            string n = dgvShops[dgvShops.CurrentCellAddress.X, dgvShops.CurrentCellAddress.Y].OwningColumn.Name;
            int icode = dgvShops.Columns[ColCode].Index;

            if (n != ColCode && e.KeyCode == Keys.Enter)
            {

                e.Handled = true;

                if (dgvShops.Rows[dgvShops.CurrentCellAddress.Y + 1].IsNewRow) return;

                dgvShops.CurrentCell = dgvShops[icode, dgvShops.CurrentCellAddress.Y + 1];
            }
        }
        #endregion

        #region "入力値のチェック"
        private void CheckCode(string v, bool isNew = false)
        {
            int r;
            if (v.Length < 3 || !int.TryParse(v, out r)) throw new Exception("コードは三桁の数値で入力してください");
            
            if(isNew && ss.List.Any(s => s.Code == r)) throw new Exception("入力されたコードは既に使用されています");
        }
        
        private void dgvShops_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!dgvShops.IsCurrentRowDirty) return;

            int icode = dgvShops.Columns[ColCode].Index;
            int iname = dgvShops.Columns[ColName].Index;
            string vcode, vname;

            StringBuilder m = new StringBuilder();

            try
            {
                vcode = dgvShops[icode, e.RowIndex].Value.ToString();
            }
            catch (Exception)
            {
                vcode = "";
            }
            try
            {
                CheckCode(vcode, dgvShops.CurrentRow.IsNewRow);
            }
            catch (Exception ex)
            {
                m.AppendLine(ex.Message);
            }


            try
            {
                vname = dgvShops[iname, e.RowIndex].Value.ToString();
            }
            catch (Exception)
            {
                vname = "";
            }
            if (vname == "") m.AppendLine("名称は必ず入力してください");

            if (m.Length > 0)
            {
                MessageBox.Show(m.ToString());
                e.Cancel = true;
            }
        }

        private void dgvShops_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            if (dgvShops.CurrentRow.IsNewRow) return;

            int icode = dgvShops.Columns[ColCode].Index;
            int iname = dgvShops.Columns[ColName].Index;
            string vcode, vname;
            try
            {
                vcode = e.FormattedValue.ToString();
            }
            catch (Exception)
            {
                vcode = "";
            }

            if (e.ColumnIndex == icode)
            {
                try
                {
                    CheckCode(vcode, dgvShops.CurrentRow.IsNewRow);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    e.Cancel = true;
                }
            }

            try
            {
                vname = e.FormattedValue.ToString(); 
            }
            catch (Exception)
            {
                vname = "";
            }

            if (e.ColumnIndex == iname && vname == "")
            {
                MessageBox.Show("名称は必ず入力してください");
                e.Cancel = true;
            }
        }

        private void dgvShops_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvShops.CurrentRow.IsNewRow) return;

            try
            {
                ss.List[e.RowIndex].Order = e.RowIndex + 1; // Orderをセット
            }
            catch (Exception)
            {
                // 最終行を削除するとインデックスエラーが発生するが、無視してよい
            }
        }

        private void dgvShops_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            changed = true;
        }
        #endregion

        #region "データの削除、移動"
        private void dgvShops_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvShops.CurrentRow.IsNewRow) return;

            int ci = e.ColumnIndex;
            int ri = e.RowIndex;
            if (ci < 0 || ri < 0) return;

            shop s = ss.List[ri];

            switch (dgvShops.Columns[ci].Name)
            {
                case ColDelete:
                    if (MessageBox.Show(s.Name + "(" + s.Code.ToString("000") + ")を削除してよろしいですか？"
                        , ""
                        , MessageBoxButtons.YesNo)
                          == DialogResult.Yes)
                    {
                        ss.RemoveAt(ri);
                        src.ResetBindings();
                    }
                    break;
                case ColUp:
                    if (ri > 1)
                    {
                        ss.Move(ri, ri - 1);
                        src.ResetBindings();
                    }
                    //dgvShops.Rows[ri - 1].Selected = true;
                    break;
                case ColDown:
                    if (ri < ss.List.Count - 1)
                    {
                        ss.Move(ri, ri + 1);
                        src.ResetBindings();
                    }
                    break;
                case ColCode:
                    SendKeys.Send("{F2}");
                    break;
                case ColName:
                    SendKeys.Send("{F2}");
                    break;
                default:
                    return;

            }
            
        }

        #endregion

        #region "保存、キャンセル"
        private void src_ListChanged(object sender, ListChangedEventArgs e)
        {
            Console.WriteLine(e.ToString());
            changed = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

    }
}
