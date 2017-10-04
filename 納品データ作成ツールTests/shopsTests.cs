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
            shops ss = new shops();
            ss.ReadDef(@".\shoplist.xml"); // 存在しないファイル
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void マスタファイルが不正()
        {
            shops ss = new shops();
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
            ss.ReadDef(x);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗が一つも記述されていない()
        {
            shops ss = new shops();
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
            ss.ReadDef(x);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗Codeが記述されていないレコードがある()
        {
            shops ss = new shops();
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
            ss.ReadDef(x);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public void 店舗Nameが記述されていないレコードがある()
        {
            shops ss = new shops();
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
            ss.ReadDef(x);
        }

        [TestMethod()]
        public void 複数の店舗が記述されている()
        {
            Common.ClearTestDir();

            shops ss = new shops();
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
            ss.ReadDef(x);

            Assert.AreEqual(1, ss.List[0].Order); // 1番目の店舗番号
            Assert.AreEqual(206, ss.List[3].Code); // 4番目の店舗コード
            Assert.AreEqual("インタッチ名古屋", ss.List[6].Name); // 7番目の店舗名
        }

    }
}