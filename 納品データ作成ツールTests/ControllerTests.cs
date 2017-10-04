using Microsoft.VisualStudio.TestTools.UnitTesting;
using 納品データ作成ツール;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EOSTools.Common;
using ClosedXML.Excel;

namespace 納品データ作成ツール.Tests
{
    [TestClass()]
    public class ControllerTests
    {
        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 初期化_店舗マスタファイルのパスが違う()
        {
            var C = new Controller("afafafa", Common.RootImageDir);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 初期化_画像のルートフォルダのパスが違う()
        {
            Common.CreateShopList();
            var C = new Controller(Common.ShopsFile, "afafaafa");
        }

        [TestMethod()]
        public void 初期化_正常()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var C = new Controller(Common.ShopsFile, Common.RootImageDir);
            Assert.AreEqual(0, C.BS.List.Count);
            Assert.AreEqual(10, C.SS.List.Count);
        }


        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void バッチの追加_バッチに異常()
        {
            Controller C = null;
            string f = "";
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir;
                List<string> l = Common.CreateImageDir(1);
                f = t + int.Parse(l[0]).ToString("0000") + ".csv"; // ファイル名が不正(末尾に-SEQ.csvがない)

                Common.CreateShopList();

                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                               , false);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.AddBatch(f);
        }

        [TestMethod()]
        public void バッチの追加_単一()
        {
            Common.ClearTestDir();

            string t = Common.TestDir + @"\";
            List<string> l = Common.CreateImageDir(1);
            string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";

            Common.CreateShopList();

            FileAccessor.saveFile( f
                           , Common.CreateDummyCSVData(int.Parse(l[0]),10)
                           , false);

            var C = new Controller(Common.ShopsFile, Common.RootImageDir);

            C.AddBatch(f);
            Assert.AreEqual(1, C.BS.List.Count);
            Assert.AreEqual(int.Parse(l[0]), C.BS.List[0].No );
        }

        [TestMethod()]
        public void バッチの追加_複数()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var C = new Controller(Common.ShopsFile, Common.RootImageDir);

            string t = Common.TestDir + @"\";
            List<string> l = Common.CreateImageDir(10);

            for (int i = 0; i < 10; i++)
            {
                string f = t + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[i]), 10)
                               , false);
                C.AddBatch(f);
                Assert.AreEqual(int.Parse(l[i]), C.BS.List[i].No);
            }
            Assert.AreEqual(10, C.BS.List.Count);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void バッチの削除_バッチが範囲内にない()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                Common.CreateShopList();
                C = new Controller(Common.ShopsFile, Common.RootImageDir);

                string t = Common.TestDir + @"\";
                List<string> l = Common.CreateImageDir(10);
                for (int i = 0; i < 10; i++)
                {
                    string f = t + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                    FileAccessor.saveFile(f
                                   , Common.CreateDummyCSVData(int.Parse(l[i]), 10)
                                   , false);
                    C.AddBatch(f);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.RemoveBatchByNo(99999); // 存在しないバッチ番号
        }

        [TestMethod()]
        public void バッチの削除_正常()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var C = new Controller(Common.ShopsFile, Common.RootImageDir);

            string t = Common.TestDir + @"\";
            List<string> l = Common.CreateImageDir(10);

            for (int i = 0; i < 10; i++)
            {
                string f = t + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[i]), 10)
                               , false);
                C.AddBatch(f);
            }

            C.RemoveBatchByNo(int.Parse(l[0]));
            C.RemoveBatchByNo(int.Parse(l[4]));
            C.RemoveBatchByNo(int.Parse(l[9]));

            Assert.AreEqual(int.Parse(l[1]), C.BS.List[0].No); // Batchリストの先頭は、元の2番目のバッチである 
        }


        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗情報の設定_存在しないバッチ番号()
        {
            Controller C = null;
            shops ss = null;
            try
            {
                Common.ClearTestDir();

                Common.CreateShopList();
                ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                string t = Common.TestDir + @"\";
                List<string> l = Common.CreateImageDir(1);
                string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                               , false);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.AddBatch(f);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.SetShopInfo(99999, ss.List[0].Name); // 存在しないバッチ
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗情報の設定_存在しない店舗名()
        {
            Controller C = null;
            List<string> l = null;
            try
            {
                Common.ClearTestDir();

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                string t = Common.TestDir + @"\";
                l = Common.CreateImageDir(1);
                string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                               , false);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);

                C.AddBatch(f);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.SetShopInfo(int.Parse(l[0]), "ヴィーナスフォート"); // 存在しない店舗名
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗情報の設定_店舗の重複()
        {
            Controller C = null;
            shops ss = null;
            List<string> l = null;
            try
            {
                Common.ClearTestDir();

                Common.CreateShopList();
                ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                string t = Common.TestDir + @"\";
                l = Common.CreateImageDir(2);
                string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                string f2 = t + int.Parse(l[1]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                               , false);
                FileAccessor.saveFile(f2
                               , Common.CreateDummyCSVData(int.Parse(l[1]), 10)
                               , false);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.AddBatch(f);
                C.AddBatch(f2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            C.SetShopInfo(int.Parse(l[0]), ss.List[0].Name);
            C.SetShopInfo(int.Parse(l[1]), ss.List[0].Name); // 同一の店舗名
        }

        [TestMethod()]
        public void 店舗情報の設定_正常()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            string t = Common.TestDir + @"\";
            List<string> l = Common.CreateImageDir(1);
            string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f
                           , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                           , false);

            var C = new Controller(Common.ShopsFile, Common.RootImageDir);

            C.AddBatch(f);
            C.SetShopInfo(int.Parse(l[0]), ss.List[0].Name);

            Assert.AreEqual(ss.List[0].Name, C.BS.List[0].shop.Name);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 画像フォルダ名の確定_店舗未設定()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                string t = Common.TestDir + @"\";
                List<string> l = Common.CreateImageDir(1);
                string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                               , false);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);

                C.AddBatch(f);
                //C.SetShopInfo(int.Parse(l[0]), ss.List[0].Name); // 店舗は設定しない
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.SetAllRenamedDirs();
        }

        [TestMethod()]
        public void 画像フォルダ名の確定_正常()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            string t = Common.TestDir + @"\";
            List<string> l = Common.CreateImageDir(2);
            string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
            string f2 = t + int.Parse(l[1]).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f
                           , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                           , false);
            FileAccessor.saveFile(f2
                           , Common.CreateDummyCSVData(int.Parse(l[1]), 10)
                           , false);

            var C = new Controller(Common.ShopsFile, Common.RootImageDir);

            C.AddBatch(f);
            C.AddBatch(f2);
            C.SetShopInfo(int.Parse(l[0]), ss.List[0].Name);
            C.SetShopInfo(int.Parse(l[1]), ss.List[1].Name);
            C.SetAllRenamedDirs();

            Assert.AreEqual("01_" + ss.List[0].Code.ToString("000"), C.BS.List[0].RenamedImageDir);
            Assert.AreEqual("02_" + ss.List[1].Code.ToString("000"), C.BS.List[1].RenamedImageDir);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 出力先フォルダの設定_存在しないフォルダを指定()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();
                Common.CreateShopList();
                C = new Controller(Common.ShopsFile, Common.RootImageDir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.SetDir(@"c:\test\test\test\");
        }

        [TestMethod()]
        public void 出力先フォルダの設定_正常()
        {
            Common.ClearTestDir();
            Common.CreateShopList();
            var C = new Controller(Common.ShopsFile, Common.RootImageDir);
            C.SetDir(Common.TestDir);
        }

        [TestMethod()]
        public void ファイル名の設定_正常()
        {
            Common.ClearTestDir();

            Common.CreateShopList();

            string t = Common.TestDir;
            string f = t + @"\test.xlsx";

            var C = new Controller(Common.ShopsFile, Common.RootImageDir);
            C.SetDir(t);
            C.SetFileName("test");

            Assert.AreEqual(f, C.Dir + "\\" + C.Book + ".xlsx");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void エクスポート_出力先未指定()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir + @"\";
                string e = t + "test.xlsx";
                FileAccessor.deleteFile(e);

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                try
                {
                    C.SetFileName("test"); // ここでの例外は無視
                }
                catch (Exception)
                {
                }

                List<string> l = Common.CreateImageDir(10);

                for (int i = 0; i < 10; i++)
                {
                    string f = t + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                    FileAccessor.saveFile(f
                                   , Common.CreateDummyCSVData(int.Parse(l[i]), 10)
                                   , false);
                    C.AddBatch(f);
                    C.BS.List[i].setShop(ss.List[i]);
                    C.SetAllRenamedDirs();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.Execute(true, true);
        }


        [TestMethod()]
        public void エクスポート_出力先フォルダが既存_上書き可()
        {
            Common.ClearTestDir();

            string t = Common.TestDir + @"\";

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            var C = new Controller(Common.ShopsFile, Common.RootImageDir);
            C.SetDir(t);
            C.SetFileName("test"); 

            List<string> l = Common.CreateImageDir(1);

            string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f
                            , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                            , false);
            C.AddBatch(f);
            C.BS.List[0].setShop( ss.List[0]);

            // 出力先フォルダを先に作っておく
            string t2 =
                t + @"\image\"
                + C.BS.List[0].shop.Order.ToString("00")
                + "_" + C.BS.List[0].shop.Code.ToString("000");

            if (!FileAccessor.isExistDir(t2)) FileAccessor.createDir(t2);

            // 適当なファイルを作る
            FileAccessor.saveFile(t2 + "\\test.txt","aaa");

            C.SetAllRenamedDirs();
            C.Execute(true, true);　// ブック上書き可、フォルダ上書き可

            Assert.AreEqual(true, FileAccessor.isExistFile(t2 + "\\test.txt")); // 適当なファイルが残っている
            Assert.AreEqual(true, FileAccessor.isExistDir(t2));
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void エクスポート_出力先フォルダが既存_上書き不可()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir + @"\";

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.SetDir(t);
                C.SetFileName("test");

                List<string> l = Common.CreateImageDir(1);

                string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                                , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                                , false);
                C.AddBatch(f);
                C.BS.List[0].setShop(ss.List[0]);

                // 出力先フォルダを先に作っておく
                string t2 =
                    t + @"\image\"
                    + C.BS.List[0].shop.Order.ToString("00")
                    + "_" + C.BS.List[0].shop.Code.ToString("000");

                if (!FileAccessor.isExistDir(t2)) FileAccessor.createDir(t2);

                // 適当なファイルを作る
                FileAccessor.saveFile(t2 + "\\test.txt", "aaa");

                C.SetAllRenamedDirs();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.Execute(true, false);　// ブック上書き可、フォルダ上書き不可
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void エクスポート_ファイル名未指定()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir + @"\";

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.SetDir(t);
                C.SetFileName(""); // ファイル名が未指定

                List<string> l = Common.CreateImageDir(1);

                string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                                , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                                , false);
                C.AddBatch(f);
                C.BS.List[0].setShop(ss.List[0]);

                C.SetAllRenamedDirs();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.Execute(true, true);　// ブック上書き可、フォルダ上書き不可
        }

        [TestMethod()]
        public void エクスポート_ブックが既存_上書き可()
        {
            Common.ClearTestDir();

            string t = Common.TestDir + @"\";
            string e = t + @"\test.xlsx";

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            var C = new Controller(Common.ShopsFile, Common.RootImageDir);
            C.SetDir(t);
            C.SetFileName("test");

            List<string> l = Common.CreateImageDir(1);

            string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f
                            , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                            , false);
            C.AddBatch(f);
            C.BS.List[0].setShop(ss.List[0]);

            var x = new XLWorkbook();
            x.AddWorksheet("test1");
            x.SaveAs(e); // ブックを作っておく

            C.SetAllRenamedDirs();
            C.Execute(true, true); // ブック上書き可、フォルダ上書き不可

            x = new XLWorkbook(e);
            // 行数が一致するか
            Assert.AreEqual(C.RecordCount + 1, x.Worksheet(1).LastCellUsed().Address.RowNumber);

            // 最後のセルの値が一致するか
            Assert.AreEqual(C.BS.List[C.BS.List.Count - 1].Dt.Rows[C.BS.List[C.BS.List.Count - 1].Dt.Rows.Count - 1][Common.Headers.Split(',').Length - 1].ToString()
                , x.Worksheet(1).LastCellUsed().Value);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void エクスポート_ブックが既存_上書き不可()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir + @"\";
                string e = t + "test.xlsx";

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.SetDir(t);
                C.SetFileName("test");

                if (!FileAccessor.isExistFile(e)) FileAccessor.saveFile(e, "", false);

                List<string> l = Common.CreateImageDir(1);

                string f = t + int.Parse(l[0]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                                , Common.CreateDummyCSVData(int.Parse(l[0]), 10)
                                , false);
                C.AddBatch(f);
                C.BS.List[0].setShop(ss.List[0]);
                C.SetAllRenamedDirs();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.Execute(false, true);　// ブック上書き可、フォルダ上書き不可
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void エクスポート_バッチが選択されていない()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir + @"\";
                string e = t + "test.xlsx";

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.SetDir(t);
                C.SetFileName("test");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.Execute(true, true);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void エクスポート_店舗が未設定のバッチ()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir + @"\";
                string e = t + "test.xlsx";

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.SetDir(t);
                C.SetFileName("test");

                List<string> l = Common.CreateImageDir(10);

                for (int i = 0; i < 10; i++)
                {
                    string f = t + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                    FileAccessor.saveFile(f
                                   , Common.CreateDummyCSVData(int.Parse(l[i]), 10)
                                   , false);
                    C.AddBatch(f);

                    if (i % 2 == 0) C.BS.List[i].setShop(ss.List[i]); // 店舗情報を設定しないバッチもある
                }
                C.SetAllRenamedDirs();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.Execute(true, true);

        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void エクスポート_画像フォルダ名が未確定のバッチ()
        {
            Controller C = null;
            try
            {
                Common.ClearTestDir();

                string t = Common.TestDir + @"\";
                string e = t + "test.xlsx";

                Common.CreateShopList();
                var ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                C = new Controller(Common.ShopsFile, Common.RootImageDir);
                C.SetDir(t);
                C.SetFileName("test");

                List<string> l = Common.CreateImageDir(10);

                for (int i = 0; i < 10; i++)
                {
                    string f = t + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                    FileAccessor.saveFile(f
                                   , Common.CreateDummyCSVData(int.Parse(l[i]), 10)
                                   , false);
                    C.AddBatch(f);

                    if (i % 2 == 0) C.BS.List[i].setShop(ss.List[i]); // 店舗情報を設定しないバッチもある
                }
                // C.SetAllRenamedDirs(); 画像フォルダ名が未設定
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            C.Execute(true, true);

        }

        [TestMethod()]
        public void エクスポート_正常()
        {
            Common.ClearTestDir();

            string t = Common.TestDir + @"\";
            string e = t + "test.xlsx";

            Common.CreateShopList();
            var ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            var C = new Controller(Common.ShopsFile, Common.RootImageDir);
            C.SetDir(t);
            C.SetFileName("test");

            List<string> l = Common.CreateImageDir(10);

            for (int i = 0; i < 10; i++)
            {
                string f = t + int.Parse(l[i]).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f
                               , Common.CreateDummyCSVData(int.Parse(l[i]), 10)
                               , false);
                C.AddBatch(f);
                C.BS.List[i].setShop(ss.List[i]);
            }

            C.SetAllRenamedDirs();
            C.Execute(true, true);

            var x = new XLWorkbook(e);

            // 行数が一致するか
            Assert.AreEqual(C.RecordCount + 1, x.Worksheet(1).LastCellUsed().Address.RowNumber);

            // 最後のセルの値が一致するか
            Assert.AreEqual(
                C.BS.List[C.BS.List.Count - 1].Dt.Rows[C.BS.List[C.BS.List.Count - 1].Dt.Rows.Count - 1][Common.Headers.Split(',').Length - 1].ToString()
                , x.Worksheet(1).LastCellUsed().Value);
        }
    }
}