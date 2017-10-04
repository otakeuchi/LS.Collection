using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using EOSTools.Common;
using static 納品データ作成ツール.Properties.Settings;
using ClosedXML.Excel;

namespace 納品データ作成ツール
{
    public class Batch
    {
        private string nr = Environment.NewLine;

        public int No { get; private set; } // バッチ番号
        public string FileName { get; private set; } // CSVファイルのフルパス
        public DataTable Dt { get; private set; } // CSVファイルの内容
        public int Count { get; private set; } // レコード数
        public string RenamedImageDir { get; private set; } // 画像フォルダ名(リネーム後)
        public shop shop { get; private set; } // 店舗の情報

        private string ImageDirTail = "REN"; // コピー元画像フォルダ名の接尾句。RENフォルダの画像名は「バーコード番号.tif」

        public class ColOverException : Exception { }
        public class EmptyFileException : Exception { }

        /// <summary>
        /// 初期化
        /// バッチ番号、ファイル名をセットする。
        /// ファイルの内容をDataTableに展開する
        /// </summary>
        /// <param name="csvpath">CSVファイルのフルパス</param>
        public Batch(string csvpath)
        {
            if(!FileAccessor.isExistFile(csvpath))
            {
                throw new Exception("CSVファイルが見つかりません。" + nr + "[" + csvpath + "]");
            }
            try
            {
                No = int.Parse(
                        csvpath.Substring(csvpath.LastIndexOf(@"\") + 1
                        , csvpath.LastIndexOf(Default.CSVFilenameTail)
                        - csvpath.LastIndexOf(@"\") - 1));
            }
            catch (Exception)
            {
                throw new Exception("CSVファイルの名称が正しくありません。" + nr + "[" + csvpath + "]");
            }

            FileName = csvpath;

            try
            {
                if(FileAccessor.readFile(csvpath).Length < 1 ) throw new EmptyFileException();

                Dt = new DataTable();
                Dt = FileAccessor.convertCSVtoDT(csvpath);
                Count = Dt.Rows.Count;
                // CSVの1列目はSEQ。SEQは不要なので削除する。列の数は20。
                Dt.Columns.RemoveAt(0);

                if (!checkColumnCount())
                {
                    throw new ColOverException();
                }

                // 列見出しをセット
                int i = 0;
                foreach (DataColumn c in Dt.Columns)
                {
                    c.Caption = Default.CSVHeaders.Split(',')[i];
                    i++;
                }                
            }
            catch (ColOverException)
            {
                throw new Exception("CSVの列数に過不足があります。" + nr + "[" + csvpath + "]");
            }
            catch (EmptyFileException)
            {
                throw new Exception("空のファイルが選択されました" + nr + "[" + csvpath + "]");
            }
            catch (Exception)
            {
                throw new Exception("指定されたファイルはCSVファイルではありません。" + nr + "[" + csvpath + "]");
            }

            try
            {
                if (!isExistBatch())
                {
                    throw new Exception("バッチが存在しません。" + nr + "[" + No.ToString("00000000") + "]");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (!isExistImageDir())
            {
                string d = Default.ImageDirRoot + "\\" + No.ToString("00000000") + "REN";
                throw new Exception("画像のフォルダが存在しません。" + nr + "[" + d + "]");
            }

            RenamedImageDir = "";
        }

        public bool checkColumnCount()
        {
            try
            {
                if (Dt.Columns.Count == Default.CSVHeaders.Split(',').Length)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DataTableの参照に失敗しました。" + nr + ex.Message);
            }
            return false;
        }

        public bool isExistBatch()
        {
            DBAccessor wmem = new DBAccessor();
            try
            {
                wmem.open(Default.DBServer, Default.DBName, Default.DBUser, Default.DBPassword);
            }
            catch (Exception ex)
            {
                throw new Exception("DBへの接続に失敗しました" + nr + ex.Message);
            }

            try
            {
                string zph = No.ToString("00000000");
                string q = "select batchNo from wmem.Batch where batchNo='" + zph + "'";
                if (wmem.getDataTable(q).Rows.Count > 0) return true;
            }
            catch (Exception ex)
            {
                throw new Exception("データベースからバッチの情報を取得できません" + nr + ex.Message);
            }
            return false;
        }

        public bool isExistImageDir()
        {
            string d = Default.ImageDirRoot + "\\" + No.ToString("00000000") + "REN";
            return FileAccessor.isExistDir(d);
        }

        public void setShop(shop s)
        {
            shop = s;
        }

        /// <summary>
        /// 画像出力先フォルダ名をセットする
        /// </summary>
        /// <param name="order">
        /// 接頭辞(0から始まる連番)。フォルダ名は
        /// [order]_[店舗コード]　となる
        /// </param>
        public void setRenamedDir(int order)
        {
            if (order < 0) throw new Exception("フォルダ名を設定するには、ソート順を指定してください");
            if (shop == null) throw new Exception("フォルダ名を設定するには、店舗情報を設定指定下さい");

            order++; // 連番は01から
            RenamedImageDir = order.ToString("00") + "_" +  shop.Code.ToString("000");
        }

        /// <summary>
        /// フォルダとファイルのコピー
        /// </summary>
        /// <param name="o"></param>
        /// <param name="d"></param>
        public void CopyImageDir(string o, string d)
        {
            if (shop == null) throw new Exception ("店舗の情報が指定されていません。画像フォルダのリネームの処理を中断します。");
            o += "\\" + No.ToString("00000000") + ImageDirTail;
            if (!FileAccessor.isExistDir(o))
                throw new Exception("画像のコピー元のフォルダが存在しません。" + nr + "[" + o + "]");
            if (!FileAccessor.isExistDir(d))
                throw new Exception("画像のコピー先のフォルダが存在しません。" + nr + "[" + d + "]");
            if (RenamedImageDir == "")
                throw new Exception("画像のコピー先のフォルダが入力されていません。");

            d += "\\" + RenamedImageDir;
            if (!FileAccessor.isExistDir(d)) FileAccessor.createDir(d);

            try
            {
                foreach (string f in Directory.GetFiles(o))
                {
                    // 拡張子で画像ファイルか判断。画像のみコピー
                    if (Default.ImageExt.Split(',').Count(e => e == Path.GetExtension(f).Remove(0,1)) > 0)
                    {
                        string t = d + "\\" + Path.GetFileName(f);
                        if (FileAccessor.isExistFile(t)) FileAccessor.deleteFile(t); 
                        File.Copy(f, t);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("画像フォルダのコピー中にエラーが発生しました。" + nr + ex.Message);
            }
        }
    }

    // 処理進捗状況をイベントとして通知
    public delegate void BatchesEventHandler(object sender, BatchesEventArgs e);

    public class BatchesEventArgs : EventArgs
    {
        public Batch Batch { get; private set; }
        public BatchesEventArgs(Batch b)
        {
            Batch = b;
        }
    }

    public class Batches
    {
        private string nr = Environment.NewLine;
        public List<Batch> List { get; private set; }
        public List<Report> Reports { get; private set; }

        public event BatchesEventHandler Progress;　// 処理進捗状況をイベントとして通知

        protected virtual void OnProgress(BatchesEventArgs e)
        {
            if (Progress != null) Progress(this, e);
        }

        public Batches()
        {
            List = new List<Batch>();
            Reports = new List<Report>();
        }

        public void add(Batch b)
        {
            if (!List.Exists(x => x.No == b.No))
            {
                List.Add(b);
            }
            else
            {
                throw new Exception("既に登録されているバッチです。" + nr + "[" + b.No + "]");
            }
        }

        public void removeAt(int i)
        {
            try
            {
                List.RemoveAt(i);
            }
            catch (Exception ex)
            {
                throw new Exception("削除する対象のインデックスがリストの範囲を超えています。[" + i + "]"
                    + nr + ex.Message);
            }
        }

        public void removeByNo(int No)
        {   
            try
            {
                if (!List.Exists(b => b.No == No)) throw new Exception();
                List = List.Where<Batch>(b => b.No != No ).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("削除する対象がリストに存在しません。[" + No + "]" + nr + ex.Message);
            }
        }

        public void sort()
        {
            // Listの整列。Shop.Noをキーとする
            // ShopがNULLの場合は、前方に配置する。
            // ShopがNULL同士の場合は、No(バッチ番号)を比較する
            List.Sort((a, b) => {
                if (a.shop == null && b.shop == null) return a.No - b.No;
                if (a.shop == null && b.shop != null) return -1;
                if (a.shop != null && b.shop == null) return 1;

                return a.shop.Order - b.shop.Order;
            });
        }

        public void ResetList(List<Batch> l)
        {
            if (l == null) throw new ArgumentNullException();
            if (List == null) throw new ArgumentNullException();
            try
            {
                List.Clear();
                List = l;
            }
            catch (Exception ex)
            {
                throw ex; 
            }
        }

        public void Export(string book)
        {
            XLWorkbook xl;
            try
            {
                if (!FileAccessor.isExistFile(book))
                {
                    xl = new XLWorkbook();
                }
                else
                {
                    xl = new XLWorkbook(book);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Excelファイルが開けません。[" + book + "]を開いている場合は閉じてから再度実行してください。" + nr + ex.Message);
            }

            IXLWorksheet s;
            try
            {
                if (xl.Worksheets.Count < 1) xl.Worksheets.Add("sheet1");
                s = xl.Worksheet(1) ; // indexは1から始まる。先頭のシートにデータをエクスポート
            }
            catch (Exception ex)
            {
                throw new Exception("Excelファイルが開けません。[" + book + "]を開いている場合は閉じてから再度実行してください。" + nr + ex.Message);
            }

            // DataTableの連結
            DataTable dt = new DataTable();
            foreach (Batch b in List)
            {
                try
                {
                    dt.Merge(b.Dt);
                }
                catch (Exception ex)
                {
                    Reports.Add(new Report() { Batch = b, Type = ReportType.Error, Message = ex.Message });
                }
                Reports.Add(new Report() { Batch = b, Type = ReportType.Success, Message = "ファイルの内容をコピーしました。" });
                BatchesEventArgs e = new BatchesEventArgs(b);
                OnProgress(e);
            }

            try
            {
                s.Clear();
                s.Cell("A1").InsertTable(dt, false); // 第二引数をFalseとすると、テーブルではなく範囲としてデータをインポート
                // すべてのセルに書式=Textを設定
                s.Range(s.FirstCellUsed().Address, s.LastCellUsed().Address).Style.NumberFormat.Format = "@";
                s.Range(s.FirstCellUsed().Address, s.LastCellUsed().Address).SetDataType(XLCellValues.Text);
                // フォント情報
                s.Style.Font.FontSize = Default.FontSize;
                s.Style.Font.FontName = Default.FontName;
            }
            catch (Exception ex)
            {
                throw new Exception("Excelのブックへのデータのエクスポートが失敗しました。" + nr + ex.Message);
            }

            try
            {
                xl.SaveAs(book);
            }
            catch (Exception ex)
            {
                throw new Exception("Excelのブックの保存に失敗しました。" + nr + ex.Message);
            }
        }

        public void CopyImageDir(string o, string d)
        {
            foreach (Batch b in List)
            {
                try
                {
                    b.CopyImageDir(o,d);
                }
                catch (Exception ex)
                {
                    Reports.Add(new Report() { Batch = b, Type = ReportType.Error, Message = ex.Message });
                }
                Reports.Add(new Report() { Batch = b, Type = ReportType.Success, Message = "画像フォルダをコピーしました。" });
                BatchesEventArgs e = new BatchesEventArgs(b);
                OnProgress(e);
            }
        }

        public string ReportToString()
        {
            StringBuilder sb = new StringBuilder();
            if(Reports.Count > 0) sb.AppendLine("結果,ファイル名,バッチ番号,店舗名,フォルダ名,件数,メッセージ");
            Reports.Where(r => r.Type == ReportType.Error).ForEach(r => sb.AppendLine(r.ToString()));
            Reports.Where(r => r.Type == ReportType.Success).ForEach(r => sb.AppendLine(r.ToString()));
            return sb.ToString();
        }
    }

    public enum ReportType
    {
        Error = 1,
        Success = 2
    }

    public class Report
    {
        private string nr = Environment.NewLine;

        public Batch Batch { get; set; }
        public ReportType Type { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            string s;
            s = Type == ReportType.Success ? "正常終了" : "エラー";
            s += "," + Batch.FileName;
            s += "," + Batch.No;
            s += "," + Batch.shop.Name;
            s += "," + Batch.RenamedImageDir;
            s += "," + Batch.Count.ToString();
            s += "," + Message;
            return s;
        }
    }
}
