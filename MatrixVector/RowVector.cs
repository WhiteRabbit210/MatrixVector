using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace MatrixVector
{
    /// <summary>
    /// 横（行方向）ベクトルのクラス
    /// </summary>
    [Serializable]
    public class RowVector:Vector
    {
        /// <summary>
        /// 与えられたベクトルと同一の横ベクトルを作成します。
        /// </summary>
        /// <param name="Vector">コピー元のベクトル</param>
        public RowVector(Vector Vector) : base(Vector) { }

        /// <summary>
        /// 与えられたベクトルと同一の横ベクトルを作成します。
        /// </summary>
        /// <param name="Vector">コピー元のベクトル</param>
        public RowVector(RowVector Vector) : base((Vector)Vector) { }

        /// <summary>
        /// 与えられたベクトルと同一の横ベクトルを作成します。
        /// </summary>
        /// <param name="Vector">コピー元のベクトル</param>
        public RowVector(ColumnVector Vector) : base((Vector)Vector) { }

        /// <summary>
        /// 与えられた要素を持つ横ベクトルを作成します。
        /// </summary>
        /// <param name="Element">要素の配列</param>
        public RowVector(double[] Element) : base(Element) { }

        /// <summary>
        /// 指定された次元数の横ベクトルを作成します。値は全て0になります。
        /// </summary>
        /// <param name="ElementSize">ベクトルの次元数</param>
        public RowVector(int ElementSize) : base(ElementSize) { }

        /// <summary>
        /// 与えられた次元数の横ベクトルを作成します。値は全てValueになります。
        /// </summary>
        /// <param name="Value">値</param>
        /// <param name="ElementSize">次元数</param>
        public RowVector(double Value, int ElementSize) : base(Value, ElementSize) { }

        /// <summary>
        /// 空の横ベクトルのインスタンスを作成します。
        /// </summary>
        protected RowVector() { }

        /// <summary>
        /// 正規化したベクトルを取得するメソッドです。
        /// </summary>
        /// <returns>正規化されたベクトル</returns>
        public virtual RowVector GetNormlizeVector()
        {
            double Norm = this.GetNorm();

            if (Norm == 0)
            {
                //エラーを投げても良い
                //throw new ApplicationException("ベクトルの大きさが0です。");

                //一応、このバージョンは自分と同じベクトル（要素が全部0）を戻す事にする
                return new RowVector(this);
            }

            RowVector ReturnVector = new RowVector(this);
            ReturnVector /= Norm;
            return ReturnVector;
        }

        /// <summary>
        /// ベクトルを指定した個数に分割するメソッドです。
        /// </summary>
        /// <param name="Length">分割後のベクトル数</param>
        /// <returns>指定した要素数に分割されたベクトル</returns>
        public virtual RowVector[] GetSeparatedVector(int Length)
        {
            if (this.Length % Length != 0)
                throw new ApplicationException("要素が割り切れません");

            //TODO 新規追加場所
            RowVector[] ResultVector = new RowVector[Length];
            for (int i = 0; i < ResultVector.Length; i++)
            {
                ResultVector[i] = new RowVector(this.Length / Length);
                for (int j = 0; j < ResultVector[i].Length; j++)
                    ResultVector[i][j] = this[i * ResultVector[i].Length + j];
            }
            return ResultVector;
        }


        /// <summary>
        /// ベクトルの微分を取得するメソッドです。
        /// </summary>
        /// <returns>微分値ベクトル</returns>
        public virtual RowVector GetDifferential()
        {
            //TODO 新規追加場所
            RowVector ResultVector = new RowVector(this.Length - 1);
            for (int i = 0; i < ResultVector.Length; i++)
                ResultVector[i] = this[i] - this[i + 1];

            return ResultVector;
        }

        #region 部分ベクトル取得系
        /// <summary>
        /// 指定された範囲の部分ベクトルを取得します
        /// </summary>
        /// <param name="StartIndex">開始位置</param>
        /// <param name="EndIndex">終了位置</param>
        /// <returns>部分ベクトル</returns>
        public virtual RowVector GetVectorBetween(int StartIndex, int EndIndex)
        {
            if (StartIndex > EndIndex || StartIndex >= this.Length || StartIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndIndex < 0 || EndIndex >= this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            RowVector ReturnVector = new RowVector(EndIndex - StartIndex + 1);
            for (int i = StartIndex; i <= EndIndex; i++)
                ReturnVector[i - StartIndex] = this[i];
            return ReturnVector;
        }

        /// <summary>
        /// 指定した開始位置からベクトルの最後までの部分ベクトルを取得します
        /// </summary>
        /// <param name="Start">開始位置</param>
        /// <returns>部分ベクトル</returns>
        public virtual RowVector GetVectorToEnd(int Start)
        {
            return this.GetVectorBetween(Start, this.Length - 1);
        }

        /// <summary>
        /// ベクトルの最初から指定した終了位置までの部分ベクトルを取得します
        /// </summary>
        /// <param name="End">終了位置</param>
        /// <returns>部分ベクトル</returns>
        public virtual RowVector GetVectorFromStart(int End)
        {
            return this.GetVectorBetween(0, End);
        }
        #endregion

        #region オペレータオーバーロード
        #region かけ算の定義
        public static RowVector operator *(RowVector vector, double scalar)
        {
            return new RowVector((Vector)vector * scalar);
        }

        public static RowVector operator *(double scalar, RowVector vector)
        {
            return new RowVector( (Vector)vector * scalar);
        }

        public static double operator *(RowVector RowVector, ColumnVector ColumnVector)
        {
            if (RowVector.Length != ColumnVector.Length)
                throw new ApplicationException("要素の数が一致しません。");

            double Sum = 0;
            for (int i = 0; i < RowVector.Length; i++)
                Sum += RowVector[i] * ColumnVector[i];
            return Sum;
        }
        #endregion

        #region 足し算の定義
        public static RowVector operator +(RowVector LeftVecotr, RowVector RightVector)
        {
            if (LeftVecotr.Length != RightVector.Length)
                throw new ApplicationException("要素の数が一致しません。");

            RowVector ReturnVector = new RowVector(LeftVecotr);

            for (int i = 0; i < ReturnVector.Length; i++)
                ReturnVector[i] += RightVector[i];

            return ReturnVector;
        }
        #endregion

        #region 引き算の定義
        public static RowVector operator -(RowVector LeftVector, RowVector RightVector)
        {
            if (LeftVector.Length != RightVector.Length)
                throw new ApplicationException("要素の数が一致しません。");

            RowVector ReturnVector = new RowVector(LeftVector);

            for (int i = 0; i < ReturnVector.Length; i++)
                ReturnVector[i] -= RightVector[i];

            return ReturnVector;
        }
        #endregion

        #region 割り算の定義
        public static RowVector operator /(RowVector vector, double scalar)
        {
            return new RowVector((Vector)vector / scalar);
        }
        #endregion


        public static bool operator ==(RowVector vector1, RowVector vector2)
        {
            return (Vector)vector1 == (Vector)vector2;
        }

        public static bool operator !=(RowVector vector1, RowVector vector2)
        {
            return (Vector)vector1 != (Vector)vector2;
        }

        #endregion

        #region 挿入系
        /// <summary>
        /// 指定した場所にスカラー値を挿入します
        /// </summary>
        /// <param name="Index">0から始まる挿入位置</param>
        /// <param name="Scalar">スカラー値</param>
        /// <returns>値が挿入されたベクトル</returns>
        public virtual RowVector InsertAt(int Index, double Scalar)
        {
            if (Index < 0 | Index > this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            RowVector ReturnVector = new RowVector(this.Length + 1);
            int Count = 0;

            while (Count < Index)
                ReturnVector[Count] = this[Count++];

            ReturnVector[Count++] = Scalar;

            while (Count < ReturnVector.Length)
            {
                ReturnVector[Count] = this[Count - 1];
                Count++;
            }
            return ReturnVector;
        }

        /// <summary>
        /// 指定した場所にベクトルを挿入します
        /// </summary>
        /// <param name="Index">0から始まる挿入位置</param>
        /// <param name="Vector">ベクトル</param>
        /// <returns>ベクトルが挿入されたベクトル</returns>
        public virtual RowVector InsertAt(int Index, RowVector Vector)
        {
            if (Index < 0 | Index > this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            RowVector ReturnVector = new RowVector(this.Length + Vector.Length);
            int Count = 0;

            while (Count < Index)
                ReturnVector[Count] = this[Count++];

            while (Count - Index < Vector.Length)
            {
                ReturnVector[Count] = Vector[Count - Index];
                Count++;
            }

            while (Count < ReturnVector.Length)
            {
                ReturnVector[Count] = this[Count - Vector.Length];
                Count++;
            }
            return ReturnVector;
        }

        /// <summary>
        /// ベクトルの最後にスカラー値を追加します
        /// </summary>
        /// <param name="Scalar">スカラー値</param>
        /// <returns>値が挿入されたベクトル</returns>
        public virtual RowVector InsertAtEnd(double Scalar)
        {
            return this.InsertAt(this.Length, Scalar);
        }

        /// <summary>
        /// ベクトルの最後にベクトルを追加します
        /// </summary>
        /// <param name="Vector">ベクトル</param>
        /// <returns>ベクトルが挿入されたベクトル</returns>
        public virtual RowVector InsertAtEnd(RowVector Vector)
        {
            return this.InsertAt(this.Length, Vector);
        }

        /// <summary>
        /// ベクトルの最初にスカラー値を追加します
        /// </summary>
        /// <param name="Scalar">スカラー値</param>
        /// <returns>値が挿入されたベクトル</returns>
        public virtual RowVector InsertAtStart(double Scalar)
        {
            return this.InsertAt(0, Scalar);
        }

        /// <summary>
        /// ベクトルの最初にベクトルを追加します
        /// </summary>
        /// <param name="Vector">ベクトル</param>
        /// <returns>ベクトルが挿入されたベクトル</returns>
        public virtual RowVector InsertAtStart(RowVector Vector)
        {
            return this.InsertAt(0, Vector);
        }

        #endregion

        #region 削除系
        /// <summary>
        /// 指定されたインデックス配列の要素を削除したベクトルを取得します。
        /// インデックスは配列でなく1個(ただのint型)でも大丈夫です。
        /// </summary>
        /// <param name="Index">削除する要素のインデックス配列</param>
        /// <returns>指定されたインデックス配列の要素を削除したベクトル</returns>
        public virtual RowVector RemoveElementAt(params int[] Index)
        {
            //↓このコードでも実行可能。でも速度の面で難ありかも。
            /*
            Vector Vector = new Vector(this);
            for (int i = IndexArray.Length - 1 ; i >= 0; i--)
                Vector = Vector.RemoveElementAt(IndexArray[i]);
            */

            //あんまり綺麗じゃないけどこっちの方が速度は速いはず
            RowVector ReturnVector = new RowVector(this.Length - Index.Length);
            int Count = 0;
            for (int i = 0; i < this.Length; i++)
            {
                bool flag = true;
                for (int j = 0; j < Index.Length; j++)
                    if (i == Index[j])
                    {
                        flag = false;
                        break;
                    }
                if (flag) ReturnVector[Count++] = this[i];
            }

            return ReturnVector;
        }

        /// <summary>
        /// 指定した範囲の要素を削除したベクトルを取得します
        /// </summary>
        /// <param name="StartIndex">開始位置</param>
        /// <param name="EndIndex">終了位置</param>
        /// <returns>指定した範囲の要素を削除したベクトル</returns>
        public virtual RowVector RemoveRange(int StartIndex, int EndIndex)
        {
            if (StartIndex > EndIndex || StartIndex >= this.Length || StartIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndIndex < 0 || EndIndex >= this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            RowVector ReturnVector = new RowVector(this.Length - (EndIndex - StartIndex) - 1);

            for (int i = 0; i < StartIndex; i++)
                ReturnVector[i] = this[i];

            for (int i = 1; i < this.Length - EndIndex; i++)
                ReturnVector[i + StartIndex - 1] = this[i + EndIndex];

            return ReturnVector;
        }

        /// <summary>
        /// 指定した位置から最後までの要素を削除したベクトルを取得します
        /// </summary>
        /// <param name="StartIndex">削除を開始する要素のインデックス</param>
        /// <returns>指定した位置から最後までの要素を削除したベクトル</returns>
        public virtual RowVector RemoveToEnd(int StartIndex)
        {
            return this.RemoveRange(StartIndex, this.Length - 1);
        }

        /// <summary>
        /// 指定した位置までの要素を削除したベクトルを取得します
        /// </summary>
        /// <param name="EndIndex">削除を終了する要素のインデックス</param>
        /// <returns>指定した位置までの要素を削除したベクトル</returns>
        public virtual RowVector RemoveFromStart(int EndIndex)
        {
            return this.RemoveRange(0, EndIndex);
        }
        #endregion

        /// <summary>
        /// バイナリデータをロードするメソッド
        /// </summary>
        /// <param name="strLoadFileName">ロードファイル名</param>
        /// <returns>ロードしたデータ</returns>
        public new static RowVector LoadBinary(string strLoadFileName)
        {
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(strLoadFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (RowVector)bf.Deserialize(fs);
                }
            }
            catch (Exception error) { return null; }
        }

        /// <summary>
        /// ベクトルの『要素毎』のかけ算です（内積ではありません）内積は『InnerProduct』で求めて下さい。
        /// </summary>
        /// <param name="LeftVector">最初のベクトル</param>
        /// <param name="RightVector">二個目のベクトル</param>
        /// <returns>『要素毎』にかけ算されたベクトル</returns>
        public static RowVector Multiply(RowVector LeftVector, RowVector RightVector)
        {
            if (LeftVector.Length != RightVector.Length)
                throw new ApplicationException("要素の数が等しくありません");
            double[] Element = new double[LeftVector.Length];

            for (int i = 0; i < LeftVector.Length; i++)
                Element[i] = LeftVector[i] * RightVector[i];

            return new RowVector(Element);
        }
    }
}
