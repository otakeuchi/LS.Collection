using Microsoft.VisualStudio.TestTools.UnitTesting;
using 納品データ作成ツール;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EOSTools.Common;

namespace 納品データ作成ツール.Tests
{
    [TestClass()]
    public class shopsTests
    {
        [TestMethod()]
        [ExpectedException(typeof(Exception))]　// 期待値：例外
        public void 店舗マスタファイルがない()
        {
            shops ss = new shops(@".\shoplist.xml"); // 存在しないファイル
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void マスタファイルが不正()
        {
            string x = Common.TestDir + @"\shoplist.xml";
            try
            {
                Common.ClearTestDir();
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"SJIS\"?>");
                sb.AppendLine("<Shops>");
                sb.AppendLine("<Shop Code=\"203\" Name=\"TDCラクーア\" />");
                sb.AppendLine("<Shop Code=\"204\" Name=\"新宿ミロード\" >"); //XMLが不正(タグが閉じてない）
                sb.AppendLine("</Shops>");
                FileAccessor.saveFile(x, sb.ToString(), false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            shops ss = new shops(x);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗が一つも記述されていない()
        {
            string x = Common.TestDir + @"\shoplist.xml";
            try
            {
                Common.ClearTestDir();
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"SJIS\"?>");
                sb.AppendLine("<Shops>");
                sb.AppendLine("</Shops>");
                FileAccessor.saveFile(x, sb.ToString(), false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            shops ss = new shops(x);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗Codeが記述されていないレコードがある()
        {
            string x = Common.TestDir + @"\shoplist.xml";
            try
            {
                Common.ClearTestDir();
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"SJIS\"?>");
                sb.AppendLine("<Shops>");
                sb.AppendLine("<Shop Code=\"203\" Name=\"TDCラクーア\" />");
                sb.AppendLine("<Shop Name=\"新宿ミロード\" />"); //店舗Codeがない
                sb.AppendLine("<Shop Code=\"205\" Name=\"イクスピアリ\" />");
                sb.AppendLine("</Shops>");
                FileAccessor.saveFile(x, sb.ToString(), false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            shops ss = new shops(x);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗Nameが記述されていないレコードがある()
        {
            string x = Common.TestDir + @"\shoplist.xml";

            try
            {
                Common.ClearTestDir();
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"SJIS\"?>");
                sb.AppendLine("<Shops>");
                sb.AppendLine("<Shop Code=\"203\" Name=\"TDCラクーア\" />");
                sb.AppendLine("<Shop Code=\"204\" />"); //店舗名称がない
                sb.AppendLine("<Shop Code=\"205\" Name=\"イクスピアリ\" />");
                sb.AppendLine("</Shops>");

                FileAccessor.saveFile(x, sb.ToString(), false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            shops ss = new shops(x);
        }

        [TestMethod()]
        public void 複数の店舗が記述されている()
        {
            Common.ClearTestDir();

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"SJIS\"?>");
            sb.AppendLine("<Shops>");
            sb.AppendLine("<Shop Code=\"203\" Name=\"TDCラクーア\" />");
            sb.AppendLine("<Shop Code=\"204\" Name=\"新宿ミロード\"/>");
            sb.AppendLine("<Shop Code=\"205\" Name=\"イクスピアリ\" />");
            sb.AppendLine("<Shop Code=\"206\" Name=\"アトレ上野\" />");
            sb.AppendLine("<Shop Code=\"424\" Name=\"近鉄あべのハルカス\" />");
            sb.AppendLine("<Shop Code=\"351\" Name=\"インタッチ二子玉川\" />");
            sb.AppendLine("<Shop Code=\"822\" Name=\"インタッチ名古屋\" />");
            sb.AppendLine("</Shops>");

            string x = Common.TestDir + @"\shoplist.xml";
            FileAccessor.saveFile(x, sb.ToString(), false);
            shops ss = new shops(x);

            Assert.AreEqual(1, ss.List[0].Order); // 1番目の店舗番号
            Assert.AreEqual(206, ss.List[3].Code); // 4番目の店舗コード
            Assert.AreEqual("インタッチ名古屋", ss.List[6].Name); // 7番目の店舗名
        }

        [TestMethod()]
        public void 店舗の追加()
        {
            Common.ClearTestDir();
            Common.CreateShopList();
            shops ss = new shops(Common.ShopsFile);
            shop s = new shop();
            s.Code = 999;
            s.Name = "test";
            ss.Add(s);

            Assert.AreEqual(999, ss.List[ss.List.Count - 1].Code);
            Assert.AreEqual(ss.List.Count, ss.List[ss.List.Count - 1].Order);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗の削除_インデックスの範囲外()
        {
            shops ss = null;
            try
            {
                Common.ClearTestDir();
                Common.CreateShopList();
                ss = new shops(Common.ShopsFile);
            }
            catch (Exception)
            {
            }

            ss.RemoveAt(ss.List.Count);
        }

        [TestMethod()]
        public void 店舗の削除_正常()
        {
            shops ss = null;
            int LastCode;
            string LastName;
            Common.ClearTestDir();
            Common.CreateShopList();
            ss = new shops(Common.ShopsFile);
            LastCode =  ss.List.Last().Code;
            LastName = ss.List.Last().Name;

            ss.RemoveAt(ss.List.Count - 1);

            Assert.AreNotEqual(LastCode, ss.List.Last().Code);
            Assert.AreNotEqual(LastName, ss.List.Last().Name);
            Assert.AreEqual(ss.List.Count, ss.List.Last().Order);

        }

        [TestMethod()]
        public void 店舗の移動_後方()
        {
            shops ss = null;

            Common.ClearTestDir();
            Common.CreateShopList();

            ss = new shops(Common.ShopsFile);

            shop first = ss.List[0];
            shop second = ss.List[1];
            shop last = ss.List[ss.List.Count - 1];

            ss.Move(0, ss.List.Count - 1); // 1番目を10番目に

            Assert.AreEqual(second.Code, ss.List[0].Code);
            Assert.AreEqual(last.Code, ss.List[ss.List.Count - 2].Code);
            Assert.AreEqual(first.Code, ss.List[ss.List.Count - 1].Code);

            Assert.AreEqual(1, ss.List[0].Order);
            Assert.AreEqual(ss.List.Count, ss.List[ss.List.Count - 1].Order);
        }


        [TestMethod()]
        public void 店舗の移動_後方2()
        {
            shops ss = null;

            Common.ClearTestDir();
            Common.CreateShopList();

            ss = new shops(Common.ShopsFile);

            shop second = ss.List[1];
            shop third = ss.List[2];
            shop fourth = ss.List[3];

            ss.Move(1, 2); // 2番目を3番目に

            Assert.AreEqual(third.Code, ss.List[1].Code);
            Assert.AreEqual(second.Code, ss.List[2].Code);
            Assert.AreEqual(fourth.Code, ss.List[3].Code);

            Assert.AreEqual(2, ss.List[1].Order);
            Assert.AreEqual(3, ss.List[2].Order);
            Assert.AreEqual(4, ss.List[3].Order);
        }

        [TestMethod()]
        public void 店舗の移動_前方()
        {
            shops ss = null;

            Common.ClearTestDir();
            Common.CreateShopList();

            ss = new shops(Common.ShopsFile);

            shop first = ss.List[0];
            shop last = ss.List[ss.List.Count - 1];
            shop secondlast = ss.List[ss.List.Count - 2];

            ss.Move(9, 0); // 10番目を1番目に

            Assert.AreEqual(last.Code, ss.List[0].Code);
            Assert.AreEqual(first.Code, ss.List[1].Code);
            Assert.AreEqual(secondlast.Code, ss.List[ss.List.Count - 1].Code);

            Assert.AreEqual(1, ss.List[0].Order);
            Assert.AreEqual(ss.List.Count, ss.List[ss.List.Count - 1].Order);

        }

        [TestMethod()]
        public void 店舗の移動_前方2()
        {
            shops ss = null;

            Common.ClearTestDir();
            Common.CreateShopList();

            ss = new shops(Common.ShopsFile);

            shop fifth = ss.List[4];
            shop sixth = ss.List[5];
            shop seventh = ss.List[6];

            ss.Move(5, 4); // 6番目を5番目に

            Assert.AreEqual(sixth.Code, ss.List[4].Code);
            Assert.AreEqual(fifth.Code, ss.List[5].Code);
            Assert.AreEqual(seventh.Code, ss.List[6].Code);

            Assert.AreEqual(5, ss.List[4].Order);
            Assert.AreEqual(6, ss.List[5].Order);
            Assert.AreEqual(7, ss.List[6].Order);
        }

        [TestMethod()]
        public void シリアライズ()
        {
            shops ss = null;

            Common.ClearTestDir();
            Common.CreateShopList();

            ss = new shops(Common.ShopsFile);

            shop first = ss.List[0];
            shop fifth = ss.List[4];
            shop tenth = ss.List[9];

            ss.serialize();

            XmlDocument x = new XmlDocument();
            x.Load(Common.ShopsFile);
            XmlNodeList nl = x.SelectNodes("/Shops/Shop");

            Assert.AreEqual(first.Name, nl[0].Attributes["Name"].Value);
            Assert.AreEqual(first.Code.ToString(), nl[0].Attributes["Code"].Value.ToString());

            Assert.AreEqual(fifth.Name, nl[4].Attributes["Name"].Value);
            Assert.AreEqual(fifth.Code.ToString(), nl[4].Attributes["Code"].Value.ToString());

            Assert.AreEqual(tenth.Name, nl[9].Attributes["Name"].Value);
            Assert.AreEqual(tenth.Code.ToString(), nl[9].Attributes["Code"].Value.ToString());
        }

    }
}
