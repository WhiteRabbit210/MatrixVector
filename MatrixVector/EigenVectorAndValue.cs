using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixVector
{
    /// <summary>
    /// 固有値と固有ベクトルをセットで保持するクラス
    /// </summary>
    [Serializable]
    public class EigenVectorAndValue : IComparable
    {
        /// <summary>
        /// 固有ベクトル
        /// </summary>
        protected Vector Vector;

        /// <summary>
        /// 固有値
        /// </summary>
        protected double Value;

        #region コンストラクタ
        /// <summary>
        /// 与えられた固有ベクトルと固有値からクラスを作成します
        /// </summary>
        /// <param name="EigenVector">固有ベクトル</param>
        /// <param name="EigenValue">固有値</param>
        public EigenVectorAndValue(Vector EigenVector, double EigenValue)
        {
            this.Vector = new Vector(EigenVector);
            this.Value = EigenValue;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="BaseData">コピー元データ</param>
        public EigenVectorAndValue(EigenVectorAndValue BaseData)
        {
            this.Vector = BaseData.EigenVector;
            this.Value = BaseData.EigenValue;
        }
        #endregion

        /// <summary>
        /// 固有ベクトルの大きさを正規化するメソッドです。
        /// </summary>
        public void Normlize()
        {
            Vector = Vector.GetNormlizeVector();
        }

        /// <summary>
        /// 固有ベクトルを取得します。
        /// </summary>
        public Vector EigenVector
        {
            get { return new Vector(Vector); }
        }

        /// <summary>
        /// 固有値を取得します。
        /// </summary>
        public double EigenValue
        {
            get { return Value; }
        }

        /// <summary>
        /// 固有値と固有ベクトルを出力します。
        /// </summary>
        /// <returns>固有値と固有ベクトル</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder();

            sb.Append("固有値\n\t" + Value.ToString() + "\n");
            sb.Append("固有ベクトル\n");
            sb.Append(Vector.ToString());

            return sb.ToString();
        }


        #region IComparable メンバ

        /// <summary>
        /// 固有値の大きい順番に並べるための値を返します。
        /// </summary>
        /// <param name="obj">EigenVectorAndValue</param>
        /// <returns>固有値が大きければ正の値</returns>
        public int CompareTo(object obj)
        {
            if (obj is EigenVectorAndValue)
            {
                double ThisEigenValue = Value;
                double TargetEigenValue = ((EigenVectorAndValue)obj).Value;

                if (ThisEigenValue == TargetEigenValue)
                    return 0;

                while (Math.Abs(ThisEigenValue - TargetEigenValue) < 1)
                {
                    ThisEigenValue *= 100;
                    TargetEigenValue *= 100;
                }

                return (int)(TargetEigenValue - ThisEigenValue);
            }
            else
                return 0;            
        }

        #endregion
    }
}
