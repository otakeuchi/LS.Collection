using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EOSTools.Common;
using System.Data;
using System.Collections.Generic;
using System.Text;


namespace 納品データ作成ツール.Tests
{
    [TestClass()]
    public class BatchTests
    {

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 存在しないCSVファイルのパスを指定する()
        {
            new Batch(@".\単体テストデータ\9999-SEQ.csv");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void CSVファイルの名称が不正_末尾がSEQでない()
        {
            Common.ClearTestDir();
            string bn = Common.CreateImageDir(1)[0];
            string f = Common.TestDir + @"\" + bn.Replace("0000", "") + ".csv"; // -SEQがない
            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);
            new Batch(f);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 指定されたのがCSVファイルではない()
        {
            string f = "";
            try
            {
                Common.ClearTestDir();

                string bn = Common.CreateImageDir(1)[0];
                f = Common.TestDir + @"\" + bn.Replace("0000", "") + "-SEQ.csv";

                byte[] bs = new byte[] { 0x13, 0x40, 0xA2, 0x6f, 0x0e, 0x0e, 0x8e, 0xFF, 0x84, 0xf0 }; // バイナリデータのファイル
                System.IO.FileStream fs = new System.IO.FileStream(
                    f,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.Write);
                fs.Write(bs, 0, bs.Length);
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            new Batch(f);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 空のファイルが指定された()
        {
            string f = "";
            try
            {
                Common.ClearTestDir();
                string bn = Common.CreateImageDir(1)[0];
                f = Common.TestDir + @"\" + bn.Replace("0000", "") + "-SEQ.csv";
                FileAccessor.saveFile(f, "", false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            new Batch(f);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void CSVファイルの列数が20ではない()
        {
            string f = "";
            try
            {
                Common.ClearTestDir();

                string bn = Common.CreateImageDir(1)[0];
                var sb = new StringBuilder();
                // 列数が19列
                sb.AppendLine("171229111,,古池達子,コイケサトコ,1570067,東京都,世田谷区,喜多見８－４－８,,annie-k@nifty.com,,0334150838,00000924,0,,■,,204,,");
                sb.AppendLine("171228595,,根本光代,ネモトミツヨ,1160003,東京都,荒川区,南千住７－２４－２４,南千住スカイハイツ６０３,,nm0108@ezweb.ne.jp,09081002806,19670108,1,,1,1,204,,");
                sb.AppendLine("171232429,,亀井由美子,カメイユミコ,,,,,,pdmusic47453@gmail.com,,09072187470,19780328,0,,0,,204,,");
                f = Common.TestDir + @"\" + bn.Replace("0000", "") + "-SEQ.csv";
                FileAccessor.saveFile(f, sb.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            new Batch(f);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void バッチがDBに存在しない()
        {
            string f = "";
            try
            {
                Common.ClearTestDir();
                string bn = Common.CreateImageDir(1)[0];
                int bi = int.Parse(bn) + 1; //存在する最大値より1つ大きい値
                f = Common.TestDir + @"\" + bi.ToString() + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            new Batch(f);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 指定されたCSVファイル名に対応する画像フォルダが存在しない()
        {
            string f = "";
            try
            {
                Common.ClearTestDir();

                string bn = Common.CreateImageDir(1)[0];
                f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString());

                FileAccessor.deleteDir(Common.RootImageDir + "\\" + bn + "REN"); // フォルダを削除
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            new Batch(f);
        }

        [TestMethod()]
        public void 正しいCSVファイルを指定する()
        {
            Common.ClearTestDir();

            string bn = Common.CreateImageDir(1)[0];
            string f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString());
            new Batch(f);
        }

        [TestMethod()]
        public void Shopオブジェクトをセット_上書き()
        {
            Common.ClearTestDir();

            Common.CreateShopList();
            shops ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            string bn = Common.CreateImageDir(1)[0];
            string f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);
            var b = new Batch(f);

            b.setShop(ss.List[0]); // TDCラクーア
            Assert.AreEqual(1, b.shop.Order);
            Assert.AreEqual(203, b.shop.Code);
            Assert.AreEqual("TDCラクーア", b.shop.Name);

            b.setShop(ss.List[9]); // インタッチ名古屋
            Assert.AreEqual(10, b.shop.Order);
            Assert.AreEqual(822, b.shop.Code);
            Assert.AreEqual("インタッチ名古屋", b.shop.Name);
        }


        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 画像フォルダのコピー_店舗情報がセットされていない()
        {
            Batch b = null;
            string bn = "";
            try
            {
                Common.ClearTestDir();
                bn = Common.CreateImageDir(1)[0];
                string f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);
                b = new Batch(f);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            b.CopyImageDir(Common.RootImageDir + "\\" + bn + "REN", Common.TestDir + @"\");
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 画像フォルダのコピー_コピー元フォルダが存在しない()
        {
            Batch b = null;
            try
            {
                Common.ClearTestDir();
                string bn = Common.CreateImageDir(1)[0];
                string f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
                FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);

                b = new Batch(f);

                Common.CreateShopList();
                shops ss = new shops();
                ss.ReadDef(Common.ShopsFile);

                b.setShop(ss.List[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            b.CopyImageDir(Common.RootImageDir + "\\へんなフォルダ" , Common.TestDir + @"\"); 
        }

        [TestMethod()]
        public void 画像フォルダのコピー_コピー成功()
        {
            Common.ClearTestDir();

            string bn = Common.CreateImageDir(1)[0];
            string f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);

            var b = new Batch(f);

            Common.CreateShopList();
            shops ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            b.setShop(ss.List[0]);
            b.setRenamedDir(0); // フォルダ名を確定

            b.CopyImageDir(Common.RootImageDir, Common.TestDir + @"\");

            // フォルダがコピーされている
            string d = Common.TestDir + @"\" + b.shop.Order.ToString("00") + "_" + b.shop.Code.ToString("000");
            Assert.AreEqual(true, FileAccessor.isExistDir(d));

            // ファイルがすべてコピーされている
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(true, FileAccessor.isExistFile(d + "\\" + i.ToString() + ".tif"));
            }
        }


        [TestMethod()]
        public void 画像フォルダのコピー_コピー先フォルダの上書き()
        {
            Common.ClearTestDir();

            string bn = Common.CreateImageDir(1)[0];
            string f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);

            var b = new Batch(f);

            Common.CreateShopList();
            shops ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            b.setShop(ss.List[0]);
            b.setRenamedDir(0); // フォルダ名を確定

            string d = Common.TestDir + @"\" + b.shop.Order.ToString("00") + "_" + b.shop.Code.ToString("000");

            // フォルダを作っておく
            if (FileAccessor.isExistDir(d))  FileAccessor.deleteDir(d);
            FileAccessor.createDir(d);
            // 適当なファイルを入れておく
            FileAccessor.saveFile(d + "\\temp.tif", "", false);

            b.CopyImageDir(Common.RootImageDir, Common.TestDir + @"\");

            // ファイルがすべてコピーされている
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(true, FileAccessor.isExistFile(d + "\\" + i.ToString() + ".tif"));
            }
            // 元からあるファイルが残っている
            Assert.AreEqual(true, FileAccessor.isExistFile(d + "\\temp.tif"));
        }

        [TestMethod()]
        public void 画像フォルダのコピー_画像以外のファイルが存在する()
        {
            Common.ClearTestDir();

            string bn = Common.CreateImageDir(1)[0];
            string f = Common.TestDir + @"\" + int.Parse(bn).ToString("0000") + "-SEQ.csv";
            FileAccessor.saveFile(f, Common.CreateDummyCSVData(int.Parse(bn), 5).ToString(), false);

            var b = new Batch(f);

            Common.CreateShopList();
            shops ss = new shops();
            ss.ReadDef(Common.ShopsFile);

            b.setShop( ss.List[0]);
            b.setRenamedDir(0); // フォルダ名を確定

            string d = Common.TestDir + @"\" + b.shop.Order.ToString("00") + "_" + b.shop.Code.ToString("000");

            // コピー元フォルダに適当なファイルを入れておく(画像以外)
            FileAccessor.saveFile(Common.RootImageDir + "\\" + int.Parse(bn).ToString("00000000") +  "REN\\temp.txt", "", false);

            b.CopyImageDir(Common.RootImageDir, Common.TestDir + @"\");

            // ファイルが消えている
            Assert.AreEqual(false, FileAccessor.isExistFile(d + "\\temp.txt"));
            // ファイルがすべてコピーされている
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(true, FileAccessor.isExistFile(d + "\\" + i.ToString() + ".tif"));
            }
        }
    }
}