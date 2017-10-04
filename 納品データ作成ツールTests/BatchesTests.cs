using Microsoft.VisualStudio.TestTools.UnitTesting;
using 納品データ作成ツール;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ClosedXML.Excel;
using EOSTools.Common;
using Excel = Microsoft.Office.Interop.Excel;



namespace 納品データ作成ツール.Tests
{
    [TestClass()]

    public class BatchesTests
    {
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 同じバッチ番号のバッチを2回追加する()
        {
            Batches bs = null;
            string f = "";
            try
            {
                Common.ClearTestDir();
                List<string> l = Common.CreateImageDir(1);
                f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);
                bs = new Batches();
                bs.add(new Batch(f));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            bs.add(new Batch(f));
        }

        [TestMethod()]
        public void 異なるバッチ番号のバッチを追加する()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(2);
            string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
            string f2 = Common.TestDir + @"\" + int.Parse(l[1]).ToString("0000") + "-SEQ.csv";

            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);
            FileAccessor.saveFile(f2, Common.CreateDummyCSVData(int.Parse(l[1]), 10).ToString(), false);

            var bs = new Batches();
            bs.add(new Batch(f));
            bs.add(new Batch(f2));

            Assert.AreEqual(int.Parse(l[0]), bs.List[0].No);
            Assert.AreEqual(10, bs.List[1].Count);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void バッチ削除_indexによる_範囲外のindexを指定()
        {
            Batches bs = null;
            try
            {
                Common.ClearTestDir();

                List<string> l = Common.CreateImageDir(1);
                string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);

                bs = new Batches();
                bs.add(new Batch(f));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            bs.removeAt(1); // 存在しないindex
        }

        [TestMethod()]
        public void バッチ削除_indexによる_範囲内のindexを指定()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(2);
            string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
            string f2 = Common.TestDir + @"\" + int.Parse(l[1]).ToString("0000") + "-SEQ.csv";

            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);
            FileAccessor.saveFile(f2, Common.CreateDummyCSVData(int.Parse(l[1]), 10).ToString(), false);

            var bs = new Batches();
            bs.add(new Batch(f));
            bs.add(new Batch(f2));

            bs.removeAt(0); // 1番目の要素を削除

            Assert.AreEqual(1, bs.List.Count);
            Assert.AreEqual(10, bs.List[0].Count);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void バッチ削除_bachNoによる_存在しない番号を指定()
        {
            Batches bs = null;
            try
            {
                Common.ClearTestDir();
                List<string> l = Common.CreateImageDir(1);
                string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);

                bs = new Batches();
                bs.add(new Batch(f));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            bs.removeByNo(99999); // 存在しないindex
        }

        [TestMethod()]
        public void バッチ削除_bachNoによる_存在する番号を指定()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(2);
            string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
            string f2 = Common.TestDir + @"\" + int.Parse(l[1]).ToString("0000") + "-SEQ.csv";

            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);
            FileAccessor.saveFile(f2, Common.CreateDummyCSVData(int.Parse(l[1]), 10).ToString(), false);

            var bs = new Batches();
            bs.add(new Batch(f));
            bs.add(new Batch(f2));

            bs.removeByNo(int.Parse(l[0])); // 1番目の要素を削除

            Assert.AreEqual(1, bs.List.Count);
            Assert.AreEqual(10, bs.List[0].Count);
        }

        [TestMethod()]
        public void バッチソート_全てのバッチにshopが未指定()
        {
            Common.ClearTestDir();

            var bs = new Batches();
            List<string> l = Common.CreateImageDir(10); // lの順番は降順
            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv"; FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 5).ToString(), false);
                bs.add(new Batch(f));
            }
            bs.sort(); // 昇順に直す

            Assert.AreEqual(int.Parse(l[9]), bs.List[0].No);
            Assert.AreEqual(int.Parse(l[0]), bs.List[9].No);
        }

        [TestMethod()]
        public void バッチソート_全てのバッチにshopが指定済み()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            var bs = new Batches();
            List<string> l = Common.CreateImageDir(10); // lの順番は降順
            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 5).ToString(), false);
                Batch b = new Batch(f);
                b.setShop(ss.List[9 - i]); // 店舗情報をセット 店舗は逆順にセットする
                bs.add(b);
            }

            bs.sort(); // 店舗情報の昇順に直す

            Assert.AreEqual(ss.List[0].Name, bs.List[0].shop.Name);
            Assert.AreEqual(ss.List[9].Name, bs.List[9].shop.Name);
        }


        [TestMethod()]
        public void バッチソート_shop指定済と未指定が混在()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            var bs = new Batches();
            List<string> l = Common.CreateImageDir(10); // lの順番は降順
            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv"; FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 5).ToString(), false);
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 5).ToString(), false);
                var b = new Batch(f);
                if (i % 2 == 0) b.setShop(ss.List[9 - i]); // 店舗情報をセット 
                bs.add(b); 
            }

            bs.sort(); // 店舗情報の昇順に直す

            Assert.AreEqual(int.Parse(l[9]), bs.List[0].No); // 10番目(店舗情報がないバッチの最小値)が先頭
            Assert.AreEqual(int.Parse(l[7]), bs.List[1].No);
            Assert.AreEqual(int.Parse(l[5]), bs.List[2].No);
            Assert.AreEqual(int.Parse(l[3]), bs.List[3].No);
            Assert.AreEqual(int.Parse(l[1]), bs.List[4].No);
            Assert.AreEqual(ss.List[1].Name, bs.List[5].shop.Name);
            Assert.AreEqual(ss.List[3].Name, bs.List[6].shop.Name);
            Assert.AreEqual(ss.List[5].Name, bs.List[7].shop.Name);
            Assert.AreEqual(ss.List[7].Name, bs.List[8].shop.Name);
            Assert.AreEqual(ss.List[9].Name, bs.List[9].shop.Name);
        }

        [TestMethod()]
        public void バッチリストの置き換え()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);
            var bs = new Batches();
            List<string> l = Common.CreateImageDir(10); // lの順番は降順

            List<Batch> nbl = new List<Batch>(); // 置換用のリスト

            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 5).ToString(), false);
                Batch b = new Batch(f);
                b.setShop(ss.List[9 - i]); // 店舗情報をセット 店舗は逆順にセットする
                bs.add(b);
                nbl.Add(b);
            }
            bs.sort(); // 店舗情報の昇順に直す

            bs.ResetList(nbl); // リストを置換

            Assert.AreEqual(ss.List[9].Name, bs.List[0].shop.Name);
            Assert.AreEqual(ss.List[0].Name, bs.List[9].shop.Name);
        }

        [TestMethod()]
        public void EXCELへエクスポート_指定したブックが存在しない()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(1);
            string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";

            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 100).ToString(), false);

            var bs = new Batches();
            bs.add(new Batch(f));

            var r = new Random();
            string p = Common.TestDir + @"\" + new Random().Next(9999).ToString("0000") + ".xlsx";
            FileAccessor.deleteFile(p);

            bs.Export(p);

            Assert.AreEqual(true, FileAccessor.isExistFile(p)); // ブックが作成されている

            var x = new XLWorkbook(p);

            Assert.AreEqual(101, x.Worksheet(1).LastRowUsed(false).RowNumber());
            Assert.AreEqual("メンバーID", x.Worksheet(1).FirstCellUsed(true).Value);
            Assert.AreEqual(int.Parse(l[0]).ToString("00000000"), x.Worksheet(1).Cell(2,1).Value);
            Assert.AreEqual("最終セル", x.Worksheet(1).LastCellUsed(false).Value);

            //FileAccessor.deleteFile(p); // ファイルは消す
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void EXCELへエクスポート_指定したブックが使用中()
        {
            Batches bs = null;
            string p = "";
            Excel.Application ex = null;
            Excel.Workbook wb = null;
            try
            {
                Common.ClearTestDir();

                List<string> l = Common.CreateImageDir(1);
                string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 100).ToString(), false);

                bs = new Batches();
                bs.add(new Batch(f));

                var r = new Random();
                p = Common.TestDir + @"\" + new Random().Next(9999).ToString("0000") + ".xlsx";
                var x = new XLWorkbook();
                x.Worksheets.Add("sheet1");
                x.SaveAs(p);

                // ブックをCOMで開いて使用中にする
                ex = new Excel.Application();
                ex.Visible = false;
                wb = ex.Workbooks.Open(p);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                bs.Export(p);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //後始末
                wb.Close();
                ex.Quit();
            }
        }

        [TestMethod()]
        public void EXCELへエクスポート_正常なブックに出力する()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(1);
            string f = Common.TestDir + @"\" + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";

            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[0]), 100).ToString(), false);

            var bs = new Batches();
            bs.add(new Batch(f));

            var r = new Random();
            string p = Common.TestDir + @"\" + new Random().Next(9999).ToString("0000") + ".xlsx";
            FileAccessor.deleteFile(p);　// 同名のファイルがある場合は削除

            var x = new XLWorkbook(); // ブックを作る
            x.Worksheets.Add("sheet1");
            x.SaveAs(p);　

            bs.Export(p);　// 既存のブックに書き込み

            x = new XLWorkbook(p);

            // 一枚目のシートにすべてのデータが出力されている
            Assert.AreEqual(101, x.Worksheet(1).LastRowUsed(false).RowNumber());　// 101件(タイトル行含む)
            Assert.AreEqual("メンバーID", x.Worksheet(1).FirstCellUsed(true).Value);
            Assert.AreEqual(int.Parse(l[0]).ToString("00000000"), x.Worksheet(1).Cell(2, 1).Value); // 最初のセル
            Assert.AreEqual("最終セル", x.Worksheet(1).Cell(101,20).Value); // 最後のセル

        }

        [TestMethod()]
        public void EXCELへエクスポート_複数バッチの出力_出力内容()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(3); // 3バッチ
            var bs = new Batches();
            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 100).ToString(), false);
                bs.add(new Batch(f));
            }

            var r = new Random();
            string p = Common.TestDir + @"\" + new Random().Next(9999).ToString("0000") + ".xlsx";

            var x = new XLWorkbook(); // ブックを作る
            x.Worksheets.Add("sheet1");
            x.SaveAs(p);

            bs.Export(p);　// 　作ったブックに書き込み

            x = new XLWorkbook(p);
            Assert.AreEqual(301, x.Worksheet(1).LastRowUsed(false).RowNumber());　// 301件(タイトル行含む)
            Assert.AreEqual("メンバーID", x.Worksheet(1).FirstCellUsed(true).Value);
            Assert.AreEqual(int.Parse(l[0]).ToString("00000000"), x.Worksheet(1).Cell(2, 1).Value); // 最初のセル
            Assert.AreEqual(int.Parse(l[2]).ToString("00000000"), x.Worksheet(1).Cell(301, 1).Value); // 最後の行の1列目
            Assert.AreEqual("00000099", x.Worksheet(1).Cell(301, 2).Value); // 最後の行の2列目
            Assert.AreEqual("最終セル", x.Worksheet(1).Cell(301, 20).Value); // 最後のセル

        }

        [TestMethod()]
        public void EXCELへエクスポート_複数バッチの出力_見出し行()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(3); // 3バッチ
            var bs = new Batches();
            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 100).ToString(), false);
                bs.add(new Batch(f));
            }

            var r = new Random();
            string p = Common.TestDir + @"\" + new Random().Next(9999).ToString("0000") + ".xlsx";
            FileAccessor.deleteFile(p);　// 同名のファイルがある場合は削除

            var x = new XLWorkbook(); // ブックを作る
            x.Worksheets.Add("sheet1");
            x.SaveAs(p);

            bs.Export(p);　// 　作ったブックに書き込み

            x = new XLWorkbook(p);
            string[] h = Common.Headers.Split(',');
            for (int i = 0; i < h.Length; i++)
            {
                Assert.AreEqual(h[i], x.Worksheet(1).Cell(1,i+1 ).Value);
            }
        }


        private int ProgressCount = 0;
        // BatchesEventHandlerの実装
        private void Batches_Progress(object sender, BatchesEventArgs e)
        {
            ProgressCount++;
        }

        [TestMethod()]
        public void EXCELへエクスポート_複数バッチの出力_Progressイベント()
        {
            Common.ClearTestDir();

            List<string> l = Common.CreateImageDir(10); // 10バッチ
            var bs = new Batches();
            bs.Progress += new BatchesEventHandler(Batches_Progress); // イベントハンドラの登録 

            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 5).ToString(), false);
                bs.add(new Batch(f));
            }

            var r = new Random();
            string p = Common.TestDir + @"\" + new Random().Next(9999).ToString("0000") + ".xlsx";

            var x = new XLWorkbook(); // ブックを作る
            x.Worksheets.Add("sheet1");
            x.SaveAs(p);

            bs.Export(p);　// 作ったブックに書き込み

            Assert.AreEqual(10, ProgressCount);
        }

        [TestMethod()]
        public void 画像フォルダのコピー_複数バッチ_出力内容()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            shops ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            List<string> l = Common.CreateImageDir(3); // 3バッチ
            var bs = new Batches();
            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 100).ToString(), false);
                Batch b = new Batch(f);
                b.setShop(ss.List[9 - i]); // 店舗情報をセット 逆順にセット
                b.setRenamedDir(i); // フォルダ名をセット
                bs.add(b);
            }
            string dist = Common.TestDir + @"\image\";
            FileAccessor.createDir(dist);
            bs.CopyImageDir(Common.RootImageDir, dist);

            for (int i = 0; i < 3; i++)
            {
                string d = (3 - i).ToString("00") + "_" + ss.List[7 + i].Code.ToString("000");
                Assert.AreEqual(true,FileAccessor.isExistDir(dist + d)); // フォルダができている
                for (int j = 0; j < 10; j++) 
                {
                    Assert.AreEqual(true, FileAccessor.isExistFile(dist + d +"\\" + i.ToString() + ".tif"));　// ファイルができている
                }
            }
        }

        [TestMethod()]
        public void 画像フォルダのコピー_複数バッチ_Progressイベント()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            shops ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            List<string> l = Common.CreateImageDir(10); 
            var bs = new Batches();
            bs.Progress += new BatchesEventHandler(this.Batches_Progress); // イベントハンドラの登録 

            for (int i = 0; i < l.Count; i++)
            {
                string f = Common.TestDir + @"\" + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(l[i]), 100).ToString(), false);
                Batch b = new Batch(f);
                b.setShop(ss.List[new Random().Next(ss.List.Count)]); // 店舗情報をセット
                b.setRenamedDir(i); // フォルダ名をセット
                bs.add(b);
            }

            string dist = Common.TestDir + @"\image\";
            FileAccessor.createDir(dist);
            bs.CopyImageDir(Common.RootImageDir, dist);

            Assert.AreEqual(10, ProgressCount);
        }
    }
}