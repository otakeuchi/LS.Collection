using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using 納品データ作成ツール;
using EOSTools.Common;
using System.Data;

namespace 納品データ作成ツール.Tests
{
    static class Common
    {
        static public string nr = Environment.NewLine;

        // 見出しのリスト。Settings.Default.Properties.CSVHeadersと同じ値とする
        static public string Headers = "メンバーID,パスワード,お名前,お名前カナ,郵便番号,都道府県,市区町村,地名地番,建物名,メールアドレスＰＣ,メールアドレス携帯,電話番号,生年月日,性別,職業,メルマガ配信可否,DM郵送,契約店舗コード,ポイント有効期限,ポイント数"; 
        
        // コピー元画像フォルダのルート(テスト用)
        // 納品データ作成ツール.settings.ImageDirRootに合わせる
        static public string RootImageDir = @"C:\EntryPrj_DEV\WMEM\image\IMPORTED";

        // テストファイル出力フォルダ
        static public string TestDir = @"C:\開発関連\案件\03_レイジースーザン\201709_納品データ作成ツール\テストデータ\単体テスト";

        static public void ClearTestDir()
        {
            FileAccessor.clearDir(TestDir);
        }


        // バッチの画像フォルダを作成
        static public List<string> CreateImageDir(int num)
        {
            List<string> L = new List<string>();
            string DirTail = "REN";

            if (num < 1)
            {
                throw new Exception("引数numは1以上の値を指定してください");
            }

            // DBからバッチの情報を取得
            DBAccessor wmem = new DBAccessor();
            try
            {
                wmem.open(@"localhost\SQLEXPRESS", "wmem", "wmem", "wmem");
            }
            catch (Exception ex)
            {
                throw new Exception("DBへの接続に失敗しました" + nr + ex.Message);
            }

            DataTable dt;
            try
            {
                string q = "select top " + num.ToString() + " batchno from wmem.Batch order by batchno desc";
                dt = wmem.getDataTable(q);
                if (dt.Rows.Count < 1)
                {
                    throw new Exception("クエリの実行に失敗しました" + nr + q);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("データベースからバッチの情報を取得できません" + nr + ex.Message);
            }

            // フォルダの存在チェック。存在する場合は、何もしない
            foreach (DataRow r in dt.Rows)
            {
                string d = RootImageDir + "\\" + r.ItemArray[0].ToString() + DirTail; // バッチ番号 + REN
                if (FileAccessor.findDir(RootImageDir, r.ItemArray[0].ToString() + DirTail).Length < 1) // フォルダがない場合、フォルダを作成
                {
                    try
                    {
                        FileAccessor.createDir(d);
                        // 適当にTIFファイルを作成
                        for (int i = 0; i < 10; i++)
                        {
                            FileAccessor.saveFile(d + "\\" + i.ToString() + ".tif", "");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("フォルダの作成に失敗しました" + nr + ex.Message);
                    }
                }

                L.Add(r.ItemArray[0].ToString());
            }
            return L;
        }


        // 店舗マスタファイル
        static public string ShopsFile = TestDir + @"\shoplist.xml";
        //　店舗マスタを作成
        static public void CreateShopList()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"SJIS\"?>");
            sb.AppendLine("<Shops>");
            sb.AppendLine("<Shop Code=\"203\" Name=\"TDCラクーア\" />");
            sb.AppendLine("<Shop Code=\"204\" Name=\"新宿ミロード\"/>");
            sb.AppendLine("<Shop Code=\"205\" Name=\"イクスピアリ\" />");
            sb.AppendLine("<Shop Code=\"206\" Name=\"アトレ上野\" />");
            sb.AppendLine("<Shop Code=\"210\" Name=\"札幌パセオ\" />");
            sb.AppendLine("<Shop Code=\"211\" Name=\"仙台アエル\" />");
            sb.AppendLine("<Shop Code=\"215\" Name=\"ルミネ池袋\" />");
            sb.AppendLine("<Shop Code=\"424\" Name=\"近鉄あべのハルカス\" />");
            sb.AppendLine("<Shop Code=\"351\" Name=\"インタッチ二子玉川\" />");
            sb.AppendLine("<Shop Code=\"822\" Name=\"インタッチ名古屋\" />");
            sb.AppendLine("</Shops>");

            FileAccessor.saveFile(ShopsFile, sb.ToString(), false);
            shops ss = new shops(ShopsFile);
        }

        // ダミーのCSVデータを作る。引数にはバッチ番号を指定する
        static public string CreateDummyCSVData(int no, int count)
        {
            var sb = new StringBuilder();
            int i;
            for (i = 0; i < count - 1; i++)
            {
                sb.AppendLine(no.ToString("00000000") + "," + no.ToString("00000000") + "," + i.ToString("00000000") + ",古池達子,コイケサトコ,1570067,東京都,世田谷区,喜多見８－４－８,,annie-k@nifty.com,,0334150838,00000924,0,,■,,204,,");
            }
            // 最終行だけ少し変える
            sb.AppendLine(no.ToString("00000000") + "," + no.ToString("00000000") + "," + i.ToString("00000000") + ",古池達子,コイケサトコ,1570067,東京都,世田谷区,喜多見８－４－８,,annie-k@nifty.com,,0334150838,00000924,0,,■,,204,,最終セル");

            return sb.ToString();
        }


    }
}
