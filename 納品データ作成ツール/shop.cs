using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using EOSTools.Common;

namespace 納品データ作成ツール
{
    public class shop
    {
        public int Order { get; set; } // 店舗マスタにおける記載順
        public int Code { get; set; } // 店舗コード
        public string Name { get; set; } // 店舗名称
    }

    public class shops
    {

        private string nr = Environment.NewLine;

        public List<shop> List { get; private set; }

        public shops()
        {
            List = new List<shop>();
        }

        public void ReadDef(string f)
        {
            if (!FileAccessor.isExistFile(f))
            {
                throw new Exception("店舗リスト定義ファイルが見つかりません。" + nr + "[" + f + "]");
            }

            try
            {
                XmlDocument x = new XmlDocument();
                x.Load(f);
                XmlNodeList nl = x.SelectNodes("/Shops/Shop");

                if ( nl.Count < 1)
                {
                    throw new Exception("店舗リストの設定がされていません。" + nr + "[" + f + "]");
                }

                int o = 1; // 定義ファイルにおける出現順
                foreach (XmlElement n in nl)
                {
                    if (n.GetAttribute("Code") == ""
                         || n.GetAttribute("Name") == "")
                    {
                        throw new Exception("店舗リストに不備があります(店舗の属性値が不足している可能性があります)");
                    }

                    shop s = new shop{
                        Order = o
                        , Code = int.Parse(n.GetAttribute("Code"))
                        , Name = n.GetAttribute("Name")
                    };
                    List.Add(s);
                    o++;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("店舗リスト定義ファイルの読み込みに失敗しました。" + nr + "[" + ex.Message + "]");
            }
        }
    }

}
