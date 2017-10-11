using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 納品データ作成ツール
{
    public class ShopController
    {
        public shops ss { private set; get; }

        public ShopController(shops p)
        {
            if (p == null) throw new Exception("shopsオブジェクトへの参照が存在しません");
            ss = p;
        }

    }
}
