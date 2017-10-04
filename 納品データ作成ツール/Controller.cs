using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EOSTools.Common;
using static 納品データ作成ツール.Properties.Settings;

namespace 納品データ作成ツール
{
    public class Controller
    {
        private string nr = Environment.NewLine;

        public Batches BS { private set; get; }
        public shops SS { private set; get; }
        public string Dir { private set; get; }
        public string Book{ private set; get; }
        public StringBuilder sbReport { private set; get; }

        public string ShopListFile { private set; get; }
        public string RootImageDir { private set; get; }

        public int RecordCount { private set; get; } // レコードの総件数

        public event BatchesEventHandler Progress; // 処理進捗状況をイベントとして通知

        private const string ImageDir = "\\image";

        protected virtual void OnProgress(BatchesEventArgs e)
        {
            if (Progress != null) Progress(this, e);
        }

        private void Batches_Progress(object sender, BatchesEventArgs e)
        {
            OnProgress(e); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f">店舗情報のマスタファイルのフルパス</param>
        /// <param name="d">画像のコピー元のルートフォルダのフルパス</param>
        public Controller(string f, string d)
        {
            if (!FileAccessor.isExistFile(f)) throw new Exception("店舗マスタファイルが存在しません" + nr + f);
            if (!FileAccessor.isExistDir(d)) throw new Exception("画像のルートフォルダが存在しません" + nr + d);

            // DB接続テスト
            DBAccessor wmem = new DBAccessor();
            try
            {
                wmem.open(Default.DBServer, Default.DBName, Default.DBUser, Default.DBPassword);
            }
            catch (Exception ex)
            {
                throw new Exception("DBへの接続に失敗しました" + nr + ex.Message);
            }


            ShopListFile = f;
            RootImageDir = d;

            // バッチリストを初期化
            try
            {
                BS = new Batches();
                BS.Progress += Batches_Progress; // BatchListのイベント発生時に、自分のイベントも発生させる
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // 店舗情報を初期化
            try
            {
                SS = new shops();
                SS.ReadDef(ShopListFile);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddBatch(string FileName)
        {
            try
            {
                BS.add(new Batch(FileName));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public Batch GetBatch(int i)
        //{
        //    try
        //    {
        //        return BS.List[i];
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //public void InsertBatch(int i, Batch B)
        //{
        //    try
        //    {
        //        BS.insertAt(i, B);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        //public void RemoveAt(int i)
        //{
        //    try
        //    {
        //        BS.removeAt(i);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public void RemoveBatchByNo(int BatchNo)
        {
            try
            {
                BS.removeByNo(BatchNo);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// すべてのbatchのRenamedImageDirをセットする
        /// </summary>
        public void SetAllRenamedDirs()
        {
            BS.List.Select((Batch v, int i) => new { b = v, o = i})
                   .ToList().ForEach(a => {
                       if (a.b.shop == null) throw new Exception("店舗情報がセットされていないバッチがあります。 [" + a.b.No + "]");
                       a.b.setRenamedDir(a.o);
                   });
        }

        public void SetShopInfo(int BatchNo, string ShopName)
        {
            // 店舗名の重複チェック
            if (BS.List.Where(b =>
                    {
                        if (b.shop != null && b.shop.Name == ShopName) return true;
                        return false;
                    }).ToList().Count > 0)
            {
                throw new Exception("この店舗はすでに指定されています");
            }

            try
            {
                BS.List.Where(b => b.No == BatchNo).First().setShop(
                          SS.List.Where(s => s.Name == ShopName).First());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SetDir(string D)
        {
            if (!FileAccessor.isExistDir(D)) throw new Exception("指定されたフォルダが存在しません" + nr + D);
            Dir = D;
        }

        /// <summary>
        /// すべてのバッチの画像フォルダについて、既存のものがあるか
        /// </summary>
        public bool IsExistDirs(ref string d)
        {
            if (Dir == null || Dir == "") throw new Exception("検索対象のフォルダが指定されていません");
            foreach (Batch b in BS.List)
            {
                d = Dir + ImageDir + "\\" + b.RenamedImageDir;
                if (FileAccessor.isExistDir(d)) return true; // 既存フォルダが見つかればtrue
            }
            return false;
        }

        /// <summary>
        /// 店舗情報がすべてのバッチにセットされているか
        /// </summary>
        public bool IsSetShopinfo()
        {
            if (BS.List.Where(b => b.shop == null).ToList().Count > 0) return false;
            return true;
        }

        /// <summary>
        /// 出力先フォルダがすべてのバッチにセットされているか
        /// </summary>
        public bool IsSetRenamedDir()
        {
            if (BS.List.Where(b => b.RenamedImageDir == "").ToList().Count > 0) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="F">拡張子を除いたファイル名</param>
        public void SetFileName(string F)
        {
            Book = F;
        }
                
        public void Execute(bool isOverwriteBook, bool isOverwriteDir)
        {
            if (!FileAccessor.isExistDir(Dir)) throw new Exception("出力先フォルダが存在しません。" + nr + Dir);
            if (BS.List.Count < 1) throw new Exception("バッチ(CSVファイル)が選択されていません。" + nr + Book);
            if (!IsSetShopinfo()) throw new Exception("店舗が選択されていないバッチがあります。");
            if (!IsSetRenamedDir()) throw new Exception("出力先フォルダが選択されていないバッチがあります。");

            if(!FileAccessor.isExistDir(Dir + ImageDir)) FileAccessor.createDir(Dir + ImageDir);

            foreach (Batch b in BS.List)
            {
                var d = Dir + ImageDir + "\\" + b.RenamedImageDir;
                if (!isOverwriteDir && FileAccessor.isExistDir(d))
                    throw new Exception(d + "が既に存在しています。");
            }
            if (Book == "" || Book == null) throw new Exception("出力先ファイル名が指定されていません。");
            if (!isOverwriteBook && FileAccessor.isExistFile(Dir + "\\" + Book + ".xlsx")) throw new Exception("同名のファイルが存在します" + nr + Book);
            
            try
            {
                // 処理の実行
                RecordCount = 0;
                BS.Reports.Clear();
                BS.Export(Dir + "\\" + Book + ".xlsx");
                BS.CopyImageDir(RootImageDir, Dir + ImageDir);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            BS.List.ForEach(b => RecordCount += b.Count);

            if (RecordCount > 0)
            {
                sbReport = new StringBuilder();
                try
                {
                    // エラーの情報はレポートに詳細を出力する
                    if (BS.Reports.Any(r => r.Type == ReportType.Error))
                    {
                        sbReport.AppendLine(nr + "処理中にエラーが発生しました。以下のバッチは処理されませんでした。" + nr);
                        BS.Reports.Where(r => r.Type == ReportType.Error)
                            .ToList().ForEach(r => sbReport.AppendLine(r.ToString()));
                    }

                    sbReport.AppendLine(nr + "以下のバッチは正常に処理されました。" + nr);
                    sbReport.AppendLine(
                        "バッチ番号"
                        + ",店舗コード"
                        + ",店舗名称"
                        + ",フォルダ名"
                        + ",件数"
                    );

                    BS.List.ForEach(b =>
                            {
                                if (!BS.Reports.Where(r => r.Batch.No == b.No).Any(r => r.Type == ReportType.Error))
                                {
                                  sbReport.AppendLine(
                                      b.No.ToString()
                                      + "," + b.shop.Code
                                      + "," + b.shop.Name
                                      + "," + b.RenamedImageDir
                                      + "," + b.Count
                                      );
                                 }
                            }
                        );
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
