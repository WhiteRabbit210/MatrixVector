using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixVector
{
    public struct ValueAndIndex:IComparable
    {
        /// <summary>
        /// データソート順番の指定列挙型
        /// </summary>
        public enum SortDirection
        {
            Descending,
            Ascending,
        }

        /// <summary>
        /// 値
        /// </summary>
        private double _Value;

        /// <summary>
        /// インデックス
        /// </summary>
        private int _Index;

        static private SortDirection _SortDirection = SortDirection.Descending;

        /// <summary>
        /// 指定された値とインデックスを保持する構造体を作成します。
        /// </summary>
        /// <param name="Value">値</param>
        /// <param name="Index">インデックス</param>
        public ValueAndIndex(double Value, int Index)
        {
            _Value = Value;
            _Index = Index;
        }

        /// <summary>
        /// 値を取得、設定します。
        /// </summary>
        public double Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        /// <summary>
        /// インデックスを取得、設定します。
        /// </summary>
        public int Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        /// <summary>
        /// 現在の値とインデックスを出力します。
        /// </summary>
        /// <returns>現在の値とインデックス</returns>
        public override string ToString()
        {
            return "Value = " + _Value + " : Index = " + _Index;
        }

        /// <summary>
        /// ソートの順番を取得、設定します。
        /// </summary>
        static public SortDirection SortOder
        {
            get { return _SortDirection; }
            set { _SortDirection = value;}
        }

        #region IComparable メンバ

        public int CompareTo(object obj)
        {
            if (_SortDirection == SortDirection.Descending)
                return (int)(this._Value - ((ValueAndIndex)obj)._Value);
            else
                return (int)(((ValueAndIndex)obj)._Value - this._Value);

        }

        #endregion
    }
}
