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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // テストの準備（Form1 には DataGridView が一つ必要）
            DataTable table = new DataTable();
            table.Columns.Add("Col1", typeof(string));
            table.Columns.Add("Col2", typeof(string));
            for (int i = 1; i <= 5; i++)
                table.Rows.Add(i.ToString() + "-1", i.ToString() + "-2");
            dataGridView1.DataSource = table;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.AllowDrop = true;
        }

        // Cell 上で Drag を始めたのか、
        // 列幅変更時の Drag で Cell 領域に入ったのかを区別するためのフラグ
        private int _OwnBeginGrabRowIndex = -1;

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            _OwnBeginGrabRowIndex = -1;
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
            DataGridView.HitTestInfo hit = dataGridView1.HitTest(e.X, e.Y);
            if (hit.Type != DataGridViewHitTestType.Cell) return;
            // クリック時などは -1 に戻らないが問題なし
            _OwnBeginGrabRowIndex = hit.RowIndex;
        }

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
            if (_OwnBeginGrabRowIndex == -1) return;
            // ドラッグ＆ドロップの開始
            dataGridView1.DoDragDrop(_OwnBeginGrabRowIndex, DragDropEffects.Move);
        }

        private bool _DropDestinationIsValid;
        private int _DropDestinationRowIndex;
        private bool _DropDestinationIsNextRow;

        private void dataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

            int from, to; bool next;
            bool valid = DecideDropDestinationRowIndex(
            dataGridView1, e, out from, out to, out next);

            // ドロップ先マーカーの表示・非表示の制御
            bool needRedraw = (valid != _DropDestinationIsValid);
            if (valid)
            {
                needRedraw = needRedraw
                || (to != _DropDestinationRowIndex)
                || (next != _DropDestinationIsNextRow);
            }
            if (needRedraw)
            {
                if (_DropDestinationIsValid)
                    dataGridView1.InvalidateRow(_DropDestinationRowIndex);
                if (valid)
                    dataGridView1.InvalidateRow(to);
            }

            _DropDestinationIsValid = valid;
            _DropDestinationRowIndex = to;
            _DropDestinationIsNextRow = next;
        }

        private void dataGridView1_DragLeave(object sender, EventArgs e)
        {
            if (_DropDestinationIsValid)
            {
                _DropDestinationIsValid = false;
                dataGridView1.InvalidateRow(_DropDestinationRowIndex);
            }
        }

        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            int from, to; bool next;
            if (!DecideDropDestinationRowIndex(
            dataGridView1, e, out from, out to, out next))
                return;

            _DropDestinationIsValid = false;

            // データの移動
            to = MoveDataValue(from, to, next);
            dataGridView1.CurrentCell =
            dataGridView1[dataGridView1.CurrentCell.ColumnIndex, to];

            dataGridView1.Invalidate();
        }

        private void dataGridView1_RowPostPaint(
         object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // ドロップ先のマーカーを描画
            if (_DropDestinationIsValid
         && e.RowIndex == _DropDestinationRowIndex)
            {
                using (Pen pen = new Pen(Color.Red, 4))
                {
                    int y =
                    !_DropDestinationIsNextRow
                    ? e.RowBounds.Y + 2 : e.RowBounds.Bottom - 2;
                    e.Graphics.DrawLine(
                    pen, e.RowBounds.X, y, e.RowBounds.X + 50, y);
                }
            }
        }

        // ドロップ先の行の決定
        private bool DecideDropDestinationRowIndex(
         DataGridView grid, DragEventArgs e,
         out int from, out int to, out bool next)
        {
            from = (int)e.Data.GetData(typeof(int));
            // 元の行が追加用の行であれば、常に false
            if (grid.NewRowIndex != -1 && grid.NewRowIndex == from)
            {
                to = 0; next = false;
                return false;
            }

            Point clientPoint = grid.PointToClient(new Point(e.X, e.Y));
            // 上下のみに着目するため、横方向は無視する
            clientPoint.X = 1;
            DataGridView.HitTestInfo hit =
            grid.HitTest(clientPoint.X, clientPoint.Y);

            to = hit.RowIndex;
            if (to == -1)
            {
                int top = grid.ColumnHeadersVisible ? grid.ColumnHeadersHeight : 0;
                top += 1; // ...
                if (top > clientPoint.Y)
                    // ヘッダへのドロップ時は表示中の先頭行とする
                    to = grid.FirstDisplayedCell.RowIndex;
                else
                    // 最終行へ
                    to = grid.Rows.Count - 1;
            }
            // 追加用の行は無視
            if (to == grid.NewRowIndex) to--;

            next = (to > from);
            return (from != to);
        }

        // データの移動
        private int MoveDataValue(int from, int to, bool next)
        {
            DataTable table = (DataTable)dataGridView1.DataSource;
            // 移動するデータの退避（計算列があればたぶんダメ）
            object[] rowData = table.Rows[from].ItemArray;
            DataRow row = table.NewRow();
            row.ItemArray = rowData;
            // 移動元から削除
            table.Rows.RemoveAt(from);
            if (to > from) to--;
            // 移動先へ追加
            if (next) to++;
            if (to <= table.Rows.Count)
                table.Rows.InsertAt(row, to);
            else
                table.Rows.Add(row);
            return table.Rows.IndexOf(row);
        }

    }
}
