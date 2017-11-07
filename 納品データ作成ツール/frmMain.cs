using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EOSTools.Common;
using System.IO;
using System.Collections.Generic;

namespace 納品データ作成ツール
{
    public partial class frmMain : Form
    {
        private string nr = Environment.NewLine;
        private LogWriter log;
        private CommonError err;
        private Controller C;

        // DataGridViewの列の意味
        private int BatchNoCol = 0;
        private int ShopCol = 1;
        private int DeleteCol = 2;

        private string ResultText = @".\log\処理結果.txt";

        #region "初期化・破棄"
        private struct LoadErr
        {
            public bool IsErr;
            public string Message;
        }
        private LoadErr ler;

        public frmMain()
        {
            InitializeComponent();

            if (!FileAccessor.isExistDir("./log")) FileAccessor.createDir("./log");
            log = new LogWriter("./log", "log.txt");
            err = new CommonError();
            ler = new LoadErr();

            log.write(LogWriter.TYPE_EV, "起動‥‥");

            // バッチリストを初期化
            try
            {
                C = new Controller(Properties.Settings.Default.PathDefShops
                                   , Properties.Settings.Default.ImageDirRoot);
            }
            catch (Exception ex)
            {
                ler.IsErr = true;
                ler.Message = ex.Message;
                err.execute(ex.Message, LogWriter.TYPE_ER, true, log);
                return;
            }

            // 店舗リストをDGVにセット
            ((DataGridViewComboBoxColumn)dgvBatch.Columns[ShopCol]).DataSource = C.SS.List;
            ((DataGridViewComboBoxColumn)dgvBatch.Columns[ShopCol]).DisplayMember = "Name";

            // ゴミ箱マークをDGVにセット
            Icon ic = new Icon("./ゴミ箱.ico");
            DataGridViewImageColumn d = (DataGridViewImageColumn)dgvBatch.Columns["Delete"];
            d.Image = ic.ToBitmap();

            if (FileAccessor.isExistDir(Properties.Settings.Default.FbdPath))
            {
                txtDir.Text = Properties.Settings.Default.FbdPath;
                C.SetDir(txtDir.Text);
            }

            // 処理の進捗。ExportとCopyで、1つのファイルの処理が終わると発生
            C.Progress += new BatchesEventHandler(this.Batches_Progress);

            log.write(LogWriter.TYPE_EV, "起動完了");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ウィンドウの位置・サイズを復元
            Bounds = Properties.Settings.Default.Bounds;
            WindowState = Properties.Settings.Default.WindowState;
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            if (ler.IsErr) this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.write(LogWriter.TYPE_EV, "終了‥‥");

            // ウィンドウの位置・サイズを保存
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.Bounds = Bounds;
            }
            else
            {
                Properties.Settings.Default.Bounds = RestoreBounds;
            }
            Properties.Settings.Default.WindowState = WindowState;
            Properties.Settings.Default.Save();

            log.write(LogWriter.TYPE_EV, "終了完了");
        }

        private void refreshDGV()
        {
            dgvBatch.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBatch_CellValueChanged);

            try
            {
                dgvBatch.Rows.Clear();
                C.BS.List.ForEach(b =>
                {
                    int i = dgvBatch.Rows.Add();
                    dgvBatch.Rows[i].Cells[BatchNoCol].Value = b.No;
                    if (b.shop != null) dgvBatch.Rows[i].Cells[ShopCol].Value = b.shop.Name;
                });
            }
            catch (Exception ex)
            {
                err.execute(ex.Message, LogWriter.TYPE_ER, true, log);
                return;
            }

            dgvBatch.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBatch_CellValueChanged);
        }

        private void rollbackDGV(List<Batch> l)
        {
            C.BS.ResetList(l);
            refreshDGV();
        }

        #endregion

        #region "ドラッグアンドドロップで表を並び替え"
        //// 参考：
        //// https://social.msdn.microsoft.com/Forums/ja-JP/e59a4043-6298-4653-809f-5c8fcf04f2e6/datagridview?forum=csharpgeneralja
        ////

        //// Cell 上で Drag を始めたのか、
        //// 列幅変更時の Drag で Cell 領域に入ったのかを区別するためのフラグ
        //private int _OwnBeginGrabRowIndex = -1;

        //private void dgvBatch_MouseDown(object sender, MouseEventArgs e)
        //{
        //    _OwnBeginGrabRowIndex = -1;
        //    if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
        //    DataGridView.HitTestInfo hit = dgvBatch.HitTest(e.X, e.Y);
        //    if (hit.Type != DataGridViewHitTestType.Cell) return;
        //    // クリック時などは -1 に戻らないが問題なし
        //    _OwnBeginGrabRowIndex = hit.RowIndex;
        //}

        //private void dgvBatch_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
        //    if (_OwnBeginGrabRowIndex == -1) return;

        //    // ドラッグ＆ドロップの開始
        //    dgvBatch.DoDragDrop(_OwnBeginGrabRowIndex, DragDropEffects.Move);
        //}

        //private bool _DropDestinationIsValid;
        //private int _DropDestinationRowIndex;
        //private bool _DropDestinationIsNextRow;

        //private void dgvBatch_DragOver(object sender, DragEventArgs e)
        //{
        //    if (e.Effect == DragDropEffects.Copy) return; // ファイル選択の時

        //    e.Effect = DragDropEffects.Move;

        //    Point clientPoint = dgvBatch.PointToClient(new Point(e.X, e.Y));
        //    Console.WriteLine("マウス座標" + e.X + "," + e.Y);
        //    Console.WriteLine("クライアント座標" + clientPoint.X + "," + clientPoint.Y);

        //    // 上下のみに着目するため、横方向は無視する
        //    //clientPoint.X = 1;
        //    DataGridView.HitTestInfo hit = dgvBatch.HitTest(clientPoint.X, clientPoint.Y);

        //    Console.WriteLine("hittest結果" + hit.Type + "," + hit.RowIndex);


        //    int from, to; bool next;
        //    bool valid = DecideDropDestinationRowIndex(
        //    dgvBatch, e, out from, out to, out next);

        //    // ドロップ先マーカーの表示・非表示の制御
        //    bool needRedraw = (valid != _DropDestinationIsValid);
        //    if (valid)
        //    {
        //        needRedraw = needRedraw
        //        || (to != _DropDestinationRowIndex)
        //        || (next != _DropDestinationIsNextRow);
        //    }
        //    if (needRedraw)
        //    {
        //        if (_DropDestinationIsValid)
        //            dgvBatch.InvalidateRow(_DropDestinationRowIndex);
        //        if (valid)
        //            dgvBatch.InvalidateRow(to);
        //    }

        //    _DropDestinationIsValid = valid;
        //    _DropDestinationRowIndex = to;
        //    _DropDestinationIsNextRow = next;
        //}

        //private void dgvBatch_DragLeave(object sender, EventArgs e)
        //{
        //    if (_DropDestinationIsValid)
        //    {
        //        _DropDestinationIsValid = false;
        //        dgvBatch.InvalidateRow(_DropDestinationRowIndex);
        //    }
        //}

        //// 実際の並べ替えは、DragDropイベントで行う
        //// DragDropイベントはバッチ選択でも使用しているのでそちらを参照
        ////private void dgvBatch_DragDrop(object sender, DragEventArgs e)
        ////{
        ////}

        //private void dgvBatch_RowPostPaint(
        // object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    // ドロップ先のマーカーを描画
        //    if (_DropDestinationIsValid
        // && e.RowIndex == _DropDestinationRowIndex)
        //    {
        //        using (Pen pen = new Pen(Color.Red, 4))
        //        {
        //            int y =
        //            !_DropDestinationIsNextRow
        //            ? e.RowBounds.Y + 2 : e.RowBounds.Bottom - 2;
        //            e.Graphics.DrawLine(
        //            pen, e.RowBounds.X, y, e.RowBounds.X + 50, y);
        //        }
        //    }
        //}

        //// ドロップ先の行の決定
        //private bool DecideDropDestinationRowIndex(
        //    DataGridView grid
        //    , DragEventArgs e
        //    , out int from
        //    , out int to
        //    , out bool next)
        //{
        //    from = (int)e.Data.GetData(typeof(int));
        //    // 元の行が追加用の行であれば、常に false
        //    if (grid.NewRowIndex != -1 && grid.NewRowIndex == from)
        //    {
        //        to = 0; next = false;
        //        return false;
        //    }

        //    Point clientPoint = grid.PointToClient(new Point(e.X, e.Y));
        //    //Console.WriteLine("マウス座標" + e.X + "," + e.Y);
        //    //Console.WriteLine("クライアント座標" + clientPoint.X + "," + clientPoint.Y);

        //    // 上下のみに着目するため、横方向は無視する
        //    clientPoint.X = 1;
        //    DataGridView.HitTestInfo hit = grid.HitTest(clientPoint.X, clientPoint.Y);

        //    //Console.WriteLine("hittest結果" + hit.Type + "," +  hit.RowIndex);

        //    to = hit.RowIndex;
        //    if (to == -1)
        //    {
        //        int top = grid.ColumnHeadersVisible ? grid.ColumnHeadersHeight : 0;
        //        top += 1; // ...
        //        if (top > clientPoint.Y)
        //            // ヘッダへのドロップ時は表示中の先頭行とする
        //            to = grid.FirstDisplayedCell.RowIndex;
        //        else
        //            // 最終行へ
        //            to = grid.Rows.Count - 1;
        //    }
        //    // 追加用の行は無視
        //    if (to == grid.NewRowIndex) to--;

        //    next = (to > from);
        //    return (from != to);
        //}

        //// データの移動
        //private int MoveDataValue(DataGridView grid, int from, int to, bool next)
        //{
        //    Batch b = C.GetBatch(from); // 移動するデータ
        //    C.RemoveAt(from); // リストから削除
        //    if (to > from) to--;

        //    // 移動先へ追加
        //    if (next) to++;
        //    if (to > C.BS.List.Count)
        //    {
        //        to = C.BS.List.Count;
        //    }
        //    C.InsertBatch(to, b);

        //    refreshDGV();
        //    return to;
        //}

        #endregion

        #region "ドラッグアンドドロップでCSVファイルを選択"
        private void dgvBatch_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void dgvBatch_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy) // CSVファイルのドロップ
            {
                StringBuilder esb = new StringBuilder(); // エラー情報

                string[] fs = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                foreach (string f in fs.OrderBy(s => Path.GetFileName(s)))
                {
                    log.write(LogWriter.TYPE_EV, "CSVを追加 [" + f + "]");
                    try
                    {
                        C.AddBatch(f);
                    }
                    catch (Exception ex)
                    {
                        esb.AppendLine(ex.Message);
                        err.execute(ex.Message, LogWriter.TYPE_ER, false, log);
                    }
                }
                try
                {
                    refreshDGV();
                }
                catch (Exception ex)
                {
                    err.execute(ex.Message, LogWriter.TYPE_ER, true, log);
                    return;
                }
                if (esb.Length > 0)
                {
                    FileAccessor.saveFile(ResultText, esb.ToString(), false);
                    MessageBox.Show("追加処理中にエラーが発生しました。" + nr + nr + esb.ToString());
                }

            }
            else　// 並び替え時のドロップ
            {
                //int from, to; bool next;
                //if (!DecideDropDestinationRowIndex(
                //dgvBatch, e, out from, out to, out next))
                //    return;

                //_DropDestinationIsValid = false;

                //// データの移動
                //to = MoveDataValue(dgvBatch, from, to, next);
                //dgvBatch.CurrentCell = dgvBatch[dgvBatch.CurrentCell.ColumnIndex, to];
                //dgvBatch.Invalidate();
            }
        }
        #endregion

        #region "店舗の選択"
        private void dgvBatch_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return; // ヘッダをクリックした場合

            if (e.ColumnIndex == ShopCol)　// 店舗の選択
            {
                // 店舗のセルを選択した際、コンボボックスを開くために3回クリックしないといけない
                // 一度のクリックで開くようにする
                DataGridViewComboBoxCell c = (DataGridViewComboBoxCell)dgvBatch[e.ColumnIndex, e.RowIndex];
                SendKeys.Send("{F4}");
            }
        }

        private void dgvBatch_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // 店舗のリストが変更されたときに処理させる
            if (dgvBatch.CurrentCellAddress.X == ShopCol && dgvBatch.IsCurrentCellDirty)
            {
                dgvBatch.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        
        private void dgvBatch_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == ShopCol && (string)dgvBatch[e.ColumnIndex, e.RowIndex].Value != "")
                {
                    try
                    {
                        string n = (string)dgvBatch[e.ColumnIndex, e.RowIndex].Value;
                        int BatchNo = (int)dgvBatch[0, e.RowIndex].Value;
                        C.SetShopInfo(BatchNo, n);

                        refreshDGV();
                        dgvBatch.Rows[e.RowIndex].Selected = true;
                        log.write(LogWriter.TYPE_EV, "バッチ" + BatchNo + "に店舗" + n + "をセット");
                    }
                    catch (Exception ex)
                    {
                        /* 
                         * 本来、入力値のチェックはDirtyStateChangedで行いたいが
                         * DirtyStateChangedが呼び出されるタイミングでは、コンボボックスでどの値が選択されたのかが分からない
                         * 結果が確定した後(CellValueChanged)で入力値のチェックを行うこととする。
                         */
                        err.execute("店舗の選択中にエラーが発生しました。" + nr + ex.Message, LogWriter.TYPE_ER, true, log);
                        if (dgvBatch.IsCurrentCellDirty)
                        {
                            refreshDGV(); // データバインドしなおす。
                            dgvBatch.Rows[e.RowIndex].Selected = true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 画面ロード時にイベントが呼ばれると例外が発生するが、無視する
            }
        }
        #endregion

        #region "レコードの削除"
        private void dgvBatch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return; // ヘッダをクリックした場合

            if (e.ColumnIndex == DeleteCol) // 削除アイコン
            {
                try
                {
                    C.RemoveBatchByNo((int)dgvBatch[BatchNoCol, e.RowIndex].Value);
                    log.write(LogWriter.TYPE_EV, "バッチ" + dgvBatch[BatchNoCol, e.RowIndex].Value + "を削除");
                    refreshDGV();
                }
                catch (Exception ex)
                {
                    err.execute("バッチの削除  " + dgvBatch[BatchNoCol, e.RowIndex].Value + "に失敗しました" + nr + ex.Message, LogWriter.TYPE_ER, true, log);
                    return;
                }
            }
        }
        #endregion

        #region "ファイル名入力"
        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            try
            {
                C.SetFileName(txtFile.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        #endregion

        #region "出力先フォルダの選択"
        private void btnSelectDIr_Click(object sender, EventArgs e)
        {
            fbd.SelectedPath = Properties.Settings.Default.FbdPath;
            if (fbd.ShowDialog() != DialogResult.OK) return;
            txtDir.Text = fbd.SelectedPath;
            try
            {
                C.SetDir(txtDir.Text);
            }
            catch (Exception ex)
            {
                err.execute(ex.Message, LogWriter.TYPE_ER, true, log);
            }
            Properties.Settings.Default.FbdPath = fbd.SelectedPath;
            Properties.Settings.Default.Save();
            log.write(LogWriter.TYPE_EV, "出力先フォルダ：" + txtDir.Text);
        }
        #endregion

        #region "出力実行"
        private void btnExec_Click(object sender, EventArgs e)
        {
            // 処理失敗時を考慮し、ソート前の状態を保持しておく
            var tl = new List<Batch>(); 
            C.BS.List.ForEach(b => tl.Add(b));

            try // バッチリストを店舗No順にソートする
            {
                C.BS.sort();
                log.write(LogWriter.TYPE_EV, "バッチリストを並べ替え");
            }
            catch (Exception ex)
            {
                MessageBox.Show("リストの並び替え処理中にエラーが発生しました。" + nr + ex.Message);
                return;
            }

            try
            {
                C.SetAllRenamedDirs(); // 画像フォルダ名を確定
                log.write(LogWriter.TYPE_EV, "画像フォルダ名を確定");
            }
            catch (Exception ex)
            {
                MessageBox.Show("画像フォルダ名の設定中にエラーが発生しました。処理を中断します。" + nr + ex.Message);
                rollbackDGV(tl);
                return;
            }

            try
            {
                if (FileAccessor.isExistFile(txtDir.Text + "\\" + C.Book + ".xlsx"))
                    if (MessageBox.Show("同名のファイルが既に存在します。上書きしてよろしいですか？" + nr + C.Book + ".xlsx"
                            , "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        throw new Exception();
                    }

                string d = "";
                if (C.IsExistDirs(ref d))
                    if (MessageBox.Show("指定されたフォルダが存在します。上書きしてよろしいですか？" + nr + d
                            , "", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        throw new Exception();
                    }
            }
            catch (Exception)
            {
                MessageBox.Show("処理を中断します。");
                rollbackDGV(tl);
                return;
            }

            log.write(LogWriter.TYPE_EV, "出力開始‥‥");
            Cursor = Cursors.WaitCursor;

            try
            {
                // プログレスバーの準備
                pbr.Minimum = 0;
                pbr.Maximum = C.BS.List.Count * 2;
                pbr.Value = 0;

                // 処理の実行
                C.Execute(true, true);
            }
            catch (Exception ex)
            {

                err.execute(ex.Message, LogWriter.TYPE_ER, true, log);
                Cursor = Cursors.Default;
                pbr.Value = 0;
                rollbackDGV(tl);
                return;
            }

            refreshDGV();　//　表示の更新

            Cursor = Cursors.Default;
            pbr.Value = 0;

            // 処理結果のサマリー
            FileAccessor.saveFile(ResultText, "総件数=" + C.RecordCount
                + nr + C.sbReport // サマリーだけ表示
                , false);

            MessageBox.Show(
                C.BS.Reports.Where(
                    r => r.Type == ReportType.Error).ToList().Count > 0
                        ? "納品物の作成中にエラーが発生しました。処理結果を確認してください。"
                        : "納品物の作成が完了しました。処理結果を表示します。");

            System.Diagnostics.Process.Start(ResultText);

            log.write(LogWriter.TYPE_EV, "出力完了 件数=" + C.RecordCount.ToString()
                        + nr + C.BS.ReportToString());
        }

        // BatchesEventHandlerの実装
        private void Batches_Progress(object sender, BatchesEventArgs e)
        {
            try
            {
                pbr.Value++; // プログレスバーを進める
                System.Threading.Thread.Sleep(100); // 表示が遅れるケースへの対策
            }
            catch (Exception)
            {
                // 例外は無視してよい
            }
        }
        #endregion

        #region "メニューバー"
        private void 店舗の情報の編集_Click(object sender, EventArgs e)
        {
            // frmShops f = new frmShops(C.SS);
            log.write(LogWriter.TYPE_EV, "……店舗情報編集画面の呼び出し……");
            frmShops f = new frmShops(); //　インスタンスは渡さない→店舗情報編集後、再起動が必要
            f.ShowDialog();
            if (f.saved)
            {
                log.write(LogWriter.TYPE_EV, "店舗情報を保存したため、再起動を促す。");
                DialogResult r = MessageBox.Show("変更した内容はプログラムを再起動した後に反映されます。"
                    + nr + "お手数ですがプログラムを再起動してください。","",MessageBoxButtons.OK);

                log.write(LogWriter.TYPE_EV, "再起動：[" + r.ToString() + "]");
            }
            else
            {
                log.write(LogWriter.TYPE_EV, "店舗情報未保存");
            }
            log.write(LogWriter.TYPE_EV, "……店舗情報編集終了……");
        }
        #endregion 
    }
}
