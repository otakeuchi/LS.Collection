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

        private string FileName;

        public shops(string f)
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

                List = new List<shop>();

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

                FileName = f;

            }
            catch (Exception ex)
            {
                throw new Exception("店舗リスト定義ファイルの読み込みに失敗しました。" + nr + "[" + ex.Message + "]");
            }
        }

        /// <summary>
        /// Orderをリセットする
        /// 1から始まる数字をセット
        /// </summary>
        private void resetOrder()
        {
            try
            {
                foreach (var item in List.Select((v, i) => new { v, i }))
                {
                    item.v.Order = item.i + 1;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        public void Add(shop s)
        {
            try
            {
                List.Add(s);
                //s.Order = List.Count();
                resetOrder();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemoveAt(int i)
        {
            try
            {
                List.RemoveAt(i);
                resetOrder();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 要素の移動
        /// 引数には、いずれも移動前の値を指定する
        /// </summary>
        /// <param name="from">0から始まる数字。最大数 = List.Count - 1</param>
        /// <param name="to">0から始まる数字。最大数 = List.Count - 1</param>
        public void Move(int from , int to)
        {
            if (from == to) return;
            if (from < 0 || from > List.Count - 1) throw new IndexOutOfRangeException();
            if (to < 0 || to > List.Count - 1) throw new IndexOutOfRangeException();

            try
            {
                shop s = List[from];
                List.RemoveAt(from);
                List.Insert(to, s);
                resetOrder();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "SJIS", null));
            XmlElement elShops = doc.CreateElement("Shops");
            doc.AppendChild(elShops);


            List.ForEach( s =>{
                XmlElement elShop = doc.CreateElement("Shop");
                elShop.SetAttribute("Code", s.Code.ToString("000"));
                elShop.SetAttribute("Name", s.Name);
                elShops.AppendChild(elShop);
            });

            doc.Save(FileName);
        }

    }

}
