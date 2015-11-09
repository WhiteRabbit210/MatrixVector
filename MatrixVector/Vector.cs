using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace MatrixVector
{
    /// <summary>
    /// ベクトルのクラス（縦横を明確にしたい場合はRowVector,ColumnVectorを利用して下さい）
    /// </summary>
    [Serializable]
    public class Vector
    {
        /// <summary>
        /// ベクトルの要素
        /// </summary>
        protected double[] element;

        /// <summary>
        /// 汎用的なデータ
        /// </summary>
        protected object Tag;

        #region コンストラクタ
        /// <summary>
        /// 空のベクトルのインスタンスを作成します。
        /// </summary>
        public Vector()
        {
            this.element = new double[0];
        }

        /// <summary>
        /// 指定された次元数のベクトルを作成します。値は全て0になります。
        /// </summary>
        /// <param name="ElementSize">ベクトルの次元数</param>
        public Vector(int ElementSize)
        {
            this.element = new double[ElementSize];
        }

        /// <summary>
        /// 与えられた要素を持つベクトルを作成します。
        /// </summary>
        /// <param name="Element">要素の配列</param>
        public Vector(double[] Element)
        {
            this.element = (double[])Element.Clone();
        }

        /// <summary>
        /// 与えられた要素を持つベクトルを作成します。
        /// </summary>
        /// <param name="Element">要素の配列</param>
        public Vector(float[] Element)
        {
            this.element = (double[])Element.Clone();
        }


        /// <summary>
        /// 与えられたベクトルと同一のベクトルを作成します。
        /// </summary>
        /// <param name="vector">コピー元のベクトル</param>
        public Vector(Vector vector)
        {
            this.element = (double[])vector.element.Clone();
            this.Tag = vector.Tag;
        }

        /// <summary>
        /// 与えられた次元数のベクトルを作成します。値は全てValueになります。
        /// </summary>
        /// <param name="Value">値</param>
        /// <param name="Size">次元数</param>
        public Vector(double Value, int Size)
        {
            element = new double[Size];
            for (int i = 0; i < Size; i++)
                element[i] = Value;
        }

        /// <summary>
        /// 複数のベクトルを連結したベクトルを作成します。
        /// </summary>
        /// <param name="Vectors">ベクトル</param>
        public Vector(params Vector[] Vectors)
        {
            {
                int CountSum = 0;
                for (int i = 0; i < Vectors.Length; i++)
                    CountSum += Vectors[i].Length;
                this.element = new double[CountSum];
            }

            int TotalCount = 0;
            for (int Count = 0; Count < Vectors.Length; Count++)
                for (int i = 0; i < Vectors[Count].Length; i++)
                    this.element[TotalCount++] = Vectors[Count][i];
        }

        /// <summary>
        /// ベクトルの後ろにスカラーを追加します
        /// </summary>
        /// <param name="Vector">ベクトル</param>
        /// <param name="Scalar">スカラー</param>
        public Vector(Vector Vector, params double[] Scalar)
        {
            this.element = new double[Vector.Length + Scalar.Length];
            for (int i = 0; i < Vector.Length; i++)
                this.element[i] = Vector[i];
            for (int i = Vector.Length; i < element.Length; i++)
                this.element[i] = Scalar[i - Vector.Length];
        }

        /// <summary>
        /// スカラー配列をベクトルの前に追加します
        /// </summary>
        /// <param name="Scalar">スカラー配列</param>
        /// <param name="Vector">ベクトル</param>
        public Vector(double[] Scalar, Vector Vector)
        {
            this.element = new double[Scalar.Length + Vector.Length];
            for (int i = 0; i < Scalar.Length; i++)
                this.element[i] = Scalar[i];
            for (int i = Scalar.Length; i < this.Length; i++)
                this.element[i] = Vector[i - Scalar.Length];
        }
        #endregion

        #region 取得系
        /// <summary>
        /// ベクトルの要素の平均を取得します。
        /// </summary>
        /// <returns></returns>
        public virtual double GetAverage()
        {
            double Sum = 0;
            for (int i = 0; i < Length; i++)
                Sum += element[i];
            return Sum / Length;
        }

        /// <summary>
        /// ベクトルのノルムを取得するメソッドです。
        /// </summary>
        /// <returns>ノルム</returns>
        public virtual double GetNorm()
        {
            double Sum = 0;
            for (int i = 0; i < this.Length; i++)
                Sum += this[i] * this[i];

            double Norm = Math.Sqrt(Sum);

            return Norm;
        }

        /// <summary>
        /// 標本分散を取得するメソッドです。
        /// </summary>
        /// <returns>標本分散</returns>
        public virtual double GetSampleVariance()
        {
            double Sum = 0;
            double Average = this.GetAverage();
            for (int i = 0; i < this.Length; i++)
                Sum += (this[i] - Average) * (this[i] - Average);

            return Sum / this.Length;
        }

        /// <summary>
        /// 不偏分散を取得するメソッドです。
        /// </summary>
        /// <returns>不偏分散</returns>
        public virtual double GetUnbiasedVariance()
        {
            double Sum = 0;
            double Average = this.GetAverage();
            for (int i = 0; i < this.Length; i++)
                Sum += (this[i] - Average) * (this[i] - Average);

            return Sum / (this.Length - 1);
        }

        /// <summary>
        /// 標準偏差を取得するメソッドです。
        /// </summary>
        /// <returns>標準偏差</returns>
        public virtual double GetStandardDeviation()
        {
            return Math.Sqrt(this.GetUnbiasedVariance());
        }

        /// <summary>
        /// 正規化したベクトルを取得するメソッドです。
        /// </summary>
        /// <returns>正規化されたベクトル</returns>
        public virtual Vector GetNormlizeVector()
        {
            double Norm = this.GetNorm();

            if (Norm == 0)
            {
                //エラーを投げても良い
                //throw new ApplicationException("ベクトルの大きさが0です。");

                //一応、このバージョンは自分と同じベクトル（要素が全部0）を戻す事にする
                return new Vector(this);
            }

            Vector ReturnVector = new Vector(this);
            ReturnVector /= Norm;
            return ReturnVector;
        }

        /// <summary>
        /// 中央値を取得するメソッドです。偶数個の場合は中央要素の平均を返します。
        /// </summary>
        /// <returns>中央値</returns>
        public double GetMedian()
        {
            List<double> ListElement = new List<double>();
            for (int i = 0; i < this.Length; i++)
                ListElement.Add(this[i]);

            ListElement.Sort();

            //偶数個なら中央要素の平均を返す
            return this.Length % 2 == 1 ? ListElement[this.Length / 2] : (ListElement[this.Length / 2] + ListElement[this.Length / 2 - 1]) / 2;
        }

        /// <summary>
        /// 最大値を取得するメソッドです。
        /// </summary>
        /// <returns>最大値</returns>
        public double GetMax()
        {
            /*
            List<double> ListElement = new List<double>();
            for (int i = 0; i < this.Length; i++)
                ListElement.Add(this[i]);

            ListElement.Sort();
            return ListElement[ListElement.Count-1];
            */
            return this.element.Max();
        }

        /// <summary>
        /// 最大値とそのインデックスを取得するメソッドです。
        /// </summary>
        /// <returns>最大値とインデックス</returns>
        public virtual ValueAndIndex GetMaxWithIndex()
        {
            List<ValueAndIndex> ListElement = new List<ValueAndIndex>();
            for (int i = 0; i < this.Length; i++)
            {
                ValueAndIndex Data = new ValueAndIndex(this[i], i);
                ListElement.Add(Data);
            }
            ListElement.Sort();
            return ListElement[ListElement.Count -1];
        }

        /// <summary>
        /// 最小値を取得するメソッドです。
        /// </summary>
        /// <returns>最小値</returns>
        public double GetMin()
        {
            /*
            List<double> ListElement = new List<double>();
            for (int i = 0; i < this.Length; i++)
                ListElement.Add(this[i]);

            ListElement.Sort();
            return ListElement[0];
            */
            return this.element.Min();
        }

        /// <summary>
        /// 最小値とそのインデックスを取得するメソッドです。
        /// </summary>
        /// <returns>最小値とインデックス</returns>
        public virtual ValueAndIndex GetMinWithIndex()
        {
            List<ValueAndIndex> ListElement = new List<ValueAndIndex>();
            for (int i = 0; i < this.Length; i++)
            {
                ValueAndIndex Data = new ValueAndIndex(this[i], i);
                ListElement.Add(Data);
            }
            ListElement.Sort();
            
            return ListElement[0];
        }

        /// <summary>
        /// ベクトルの要素の合計を取得します。
        /// </summary>
        /// <returns>ベクトルの要素の合計</returns>
        public virtual double GetSum()
        {
            return element.Sum();
        }

        /// <summary>
        /// Valueよりも大きい値を持つ要素のインデックスを取得するメソッドです。
        /// </summary>
        /// <param name="Value">閾値</param>
        /// <returns>Valueよりも大きい値を持つ要素のインデックス</returns>
        public virtual int[] GetUpperIndex(double Value)
        {
            List<int> IndexList = new List<int>();
            for (int i = 0; i < this.Length; i++)
                if (this[i] > Value)
                    IndexList.Add(i);
            return IndexList.ToArray();
        }

        /// <summary>
        /// Valueよりも小さい値を持つ要素のインデックスを取得するメソッドです。
        /// </summary>
        /// <param name="Value">閾値</param>
        /// <returns>Valueよりも小さい値を持つ要素のインデックス</returns>
        public virtual int[] GetLowerIndex(double Value)
        {
            List<int> IndexList = new List<int>();
            for (int i = 0; i < this.Length; i++)
                if (this[i] < Value)
                    IndexList.Add(i);
            return IndexList.ToArray();
        }

        /// <summary>
        /// Valueと等しい値を持つ要素のインデックスを取得するメソッドです。
        /// </summary>
        /// <param name="Value">閾値</param>
        /// <returns>Valueと等しい値を持つ要素のインデックス</returns>
        public virtual int[] GetEqualIndex(double Value)
        {
            List<int> IndexList = new List<int>();
            for (int i = 0; i < this.Length; i++)
                if (this[i] == Value)
                    IndexList.Add(i);
            return IndexList.ToArray();
        }

        /// <summary>
        /// ベクトルの微分を取得するメソッドです。
        /// </summary>
        /// <returns>微分値ベクトル</returns>
        public virtual Vector GetDifferential()
        {
            //TODO 新規追加場所
            Vector ResultVector = new Vector(this.Length - 1);
            for (int i = 0; i < ResultVector.Length; i++)
                ResultVector[i] = this[i] - this[i + 1];

            return ResultVector;
        }
        
        /// <summary>
        /// ベクトルを指定した個数に分割するメソッドです。
        /// </summary>
        /// <param name="Length">分割後のベクトル数</param>
        /// <returns>指定した要素数に分割されたベクトル</returns>
        public virtual Vector[] GetSeparatedVector(int Length)
        {
            if (this.Length % Length != 0)
                throw new ApplicationException("要素が割り切れません");

            //TODO 新規追加場所
            Vector[] ResultVector = new Vector[Length];
            for (int i = 0; i < ResultVector.Length; i++)
            {
                ResultVector[i] = new Vector(this.Length / Length);
                for (int j = 0; j < ResultVector[i].Length; j++)
                    ResultVector[i][j] = this[i * ResultVector[i].Length + j];
            }
            return ResultVector;
        }

        /// <summary>
        /// ベクトルを対象行列にします。
        /// </summary>
        /// <returns>対象行列</returns>
        public virtual SymmetricMatrix GetSymmetricMatrix()
        {
            int i, j, length = element.Length;
            SymmetricMatrix returnMatrix = new SymmetricMatrix(length);
            for (i = 0; i < length; i++)
            {
                for (j = 0; j < length; j++)
                {
                    returnMatrix[i, j] = element[i] * element[j];
                }
            }
            return returnMatrix;
        }

        /// <summary>
        /// ベクトルの要素をListのstring型として取得します
        /// </summary>
        /// <returns>要素のリスト</returns>
        public List<string> ToStringList()
        {
            List<string> AnswerList = new List<string>();

            for(int i = 0; i < this.Length; i++)
                AnswerList.Add(this[i].ToString());

            return AnswerList;
        }


        public List<double> GetElementList()
        {
            return new List<double>(this.element);
        }
        #endregion

        #region 計算系
        /// <summary>
        /// ベクトルの『要素毎』のかけ算です（内積ではありません）内積は『InnerProduct』で求めて下さい。
        /// </summary>
        /// <param name="LeftVector">最初のベクトル</param>
        /// <param name="RightVector">二個目のベクトル</param>
        /// <returns>『要素毎』にかけ算されたベクトル</returns>
        public static Vector Multiply(Vector LeftVector, Vector RightVector)
        {
            if (LeftVector.Length != RightVector.Length)
                throw new ApplicationException("要素の数が等しくありません");
            double[] Element = new double[LeftVector.Length];

            for (int i = 0; i < LeftVector.Length; i++)
                Element[i] = LeftVector[i] * RightVector[i];

            return new Vector(Element);
        }

        /// <summary>
        /// 指定したベクトルとの内積を求めます。ベクトルの次元数は等しくないとエラーが発生します。
        /// </summary>
        /// <param name="Vector">内積を求めるための対象となるベクトル</param>
        /// <returns>内積</returns>
        public virtual double InnerProduct(Vector Vector)
        {
            if (this.Length != Vector.Length)
                throw new ApplicationException("要素の数が一致しません。");

            double Sum = 0;
            for (int i = 0; i < this.Length; i++)
                Sum += this[i] * Vector[i];
            return Sum;
        }
        #endregion 

        #region 部分ベクトル取得系
        /// <summary>
        /// 指定された範囲の部分ベクトルを取得します
        /// </summary>
        /// <param name="StartIndex">開始位置</param>
        /// <param name="EndIndex">終了位置</param>
        /// <returns>部分ベクトル</returns>
        public virtual Vector GetVectorBetween(int StartIndex, int EndIndex)
        {
            if (StartIndex > EndIndex || StartIndex >= this.Length || StartIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndIndex < 0 || EndIndex >= this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            Vector ReturnVector = new Vector(EndIndex - StartIndex + 1);
            for (int i = StartIndex; i <= EndIndex; i++)
                ReturnVector[i - StartIndex] = this[i];
            return ReturnVector;
        }

        /// <summary>
        /// 指定した開始位置からベクトルの最後までの部分ベクトルを取得します
        /// </summary>
        /// <param name="Start">開始位置</param>
        /// <returns>部分ベクトル</returns>
        public virtual Vector GetVectorToEnd(int Start)
        {
            return this.GetVectorBetween(Start, this.Length-1);
        }

        /// <summary>
        /// ベクトルの最初から指定した終了位置までの部分ベクトルを取得します
        /// </summary>
        /// <param name="End">終了位置</param>
        /// <returns>部分ベクトル</returns>
        public virtual Vector GetVectorFromStart(int End)
        {
            return this.GetVectorBetween(0, End);
        }
        #endregion

        #region 挿入系
        /// <summary>
        /// 指定した場所にスカラー値を挿入します
        /// </summary>
        /// <param name="Index">0から始まる挿入位置</param>
        /// <param name="Scalar">スカラー値</param>
        /// <returns>値が挿入されたベクトル</returns>
        public virtual Vector InsertAt(int Index, double Scalar)
        {
            if (Index < 0 | Index > this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            Vector ReturnVector = new Vector(this.Length + 1);
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
        public virtual Vector InsertAt(int Index, Vector Vector)
        {
            if (Index < 0 | Index > this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            Vector ReturnVector = new Vector(this.Length + Vector.Length);
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
        public virtual Vector InsertAtEnd(double Scalar)
        {
            return this.InsertAt(this.Length, Scalar);
        }

        /// <summary>
        /// ベクトルの最後にベクトルを追加します
        /// </summary>
        /// <param name="Vector">ベクトル</param>
        /// <returns>ベクトルが挿入されたベクトル</returns>
        public virtual Vector InsertAtEnd(Vector Vector)
        {
            return this.InsertAt(this.Length, Vector);
        }

        /// <summary>
        /// ベクトルの最初にスカラー値を追加します
        /// </summary>
        /// <param name="Scalar">スカラー値</param>
        /// <returns>値が挿入されたベクトル</returns>
        public virtual Vector InsertAtStart(double Scalar)
        {
            return this.InsertAt(0, Scalar);
        }

        /// <summary>
        /// ベクトルの最初にベクトルを追加します
        /// </summary>
        /// <param name="Vector">ベクトル</param>
        /// <returns>ベクトルが挿入されたベクトル</returns>
        public virtual Vector InsertAtStart(Vector Vector)
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
        public virtual Vector RemoveElementAt(params int[] Index)
        {
            //↓このコードでも実行可能。でも速度の面で難ありかも。
            /*
            Vector Vector = new Vector(this);
            for (int i = IndexArray.Length - 1 ; i >= 0; i--)
                Vector = Vector.RemoveElementAt(IndexArray[i]);
            */

            //あんまり綺麗じゃないけどこっちの方が速度は速いはず
            Vector ReturnVector = new Vector(this.Length - Index.Length);
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
        public virtual Vector RemoveRange(int StartIndex, int EndIndex)
        {
            if (StartIndex > EndIndex || StartIndex >= this.Length || StartIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndIndex < 0 || EndIndex >= this.Length)
                throw new ApplicationException("指定位置が正しくありません");

            Vector ReturnVector = new Vector(this.Length - (EndIndex - StartIndex) - 1);

            for (int i = 0; i < StartIndex; i++)
                ReturnVector[i] = this[i];

            for (int i = 1; i < this.Length - EndIndex; i++)
                ReturnVector[i + StartIndex -1] = this[i + EndIndex];

            return ReturnVector;
        }

        /// <summary>
        /// 指定した位置から最後までの要素を削除したベクトルを取得します
        /// </summary>
        /// <param name="StartIndex">削除を開始する要素のインデックス</param>
        /// <returns>指定した位置から最後までの要素を削除したベクトル</returns>
        public virtual Vector RemoveToEnd(int StartIndex)
        {
            return this.RemoveRange(StartIndex, this.Length-1);
        }

        /// <summary>
        /// 指定した位置までの要素を削除したベクトルを取得します
        /// </summary>
        /// <param name="EndIndex">削除を終了する要素のインデックス</param>
        /// <returns>指定した位置までの要素を削除したベクトル</returns>
        public virtual Vector RemoveFromStart(int EndIndex)
        {
            return this.RemoveRange(0, EndIndex);
        }
        #endregion

        #region オペレータオーバーロード
        #region かけ算の定義
        public static Vector operator *(Vector vector, double scalar)
        {
            Vector ReturnVector = new Vector(vector);
            for (int i = 0; i < vector.Length; i++)
                ReturnVector.element[i] *= scalar;
            return ReturnVector;
        }

        public static Vector operator *(double scalar, Vector vector)
        {
            return vector * scalar;
        }

        #endregion

        #region 足し算の定義
        public static Vector operator +(Vector LeftVector, Vector RightVector)
        {
            if (LeftVector.Length != RightVector.Length)
                throw new ApplicationException("要素の数が一致しません。");

            Vector ReturnVector = new Vector(LeftVector);

            for (int i = 0; i < ReturnVector.Length; i++)
                ReturnVector[i] += RightVector[i];

            return ReturnVector;
        }
        #endregion

        #region 引き算の定義
        public static Vector operator -(Vector LeftVector, Vector RightVector)
        {
            if (LeftVector.Length != RightVector.Length)
                throw new ApplicationException("要素の数が一致しません。");

            Vector ReturnVector = new Vector(LeftVector);

            for (int i = 0; i < ReturnVector.Length; i++)
                ReturnVector[i] -= RightVector[i];

            return ReturnVector;
        }
        #endregion

        #region 割り算の定義
        public static Vector operator /(Vector vector, double scalar)
        {
            Vector ReturnVector = new Vector(vector);
            for (int i = 0; i < vector.Length; i++)
                ReturnVector.element[i] /= scalar;
            return ReturnVector;
        }
        #endregion

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }

        public static bool operator ==(Vector vector1, Vector vector2)
        {
            if ((object)vector1 == null && (object)vector2 == null)
                return true;
            else if ((object)vector1 == null || (object)vector2 == null)
                return false;

            if (vector1.Length != vector2.Length)
                return false;

            for (int i = 0; i < vector1.Length; i++)
                if (vector1[i] != vector2[i])
                    return false;

            return true;
        }


        #endregion

        #region プロパティ
        /// <summary>
        /// ベクトルの要素数を取得します。
        /// </summary>
        public int Length
        {
            get { return element.Length; }
        }

        /// <summary>
        /// ベクトルの要素を設定、取得します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>ベクトル要素</returns>
        public double this[int index]
        {
            get { return this.element[index]; }
            set { this.element[index] = value; }
        }

        /// <summary>
        /// ベクトルの要素を設定、取得します。
        /// </summary>
        public double[] VectorElement
        {
            get { return this.element; }
            set { this.element = value; }
        }

        /// <summary>
        /// 汎用的なデータを取得、設定します。
        /// </summary>
        public object TagData
        {
            get { return this.Tag; }
            set { this.Tag = value; }
        }
        #endregion

        #region 入出力系
        /// <summary>
        /// ベクトルの要素を10個まで出力します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(10);
        }

        /// <summary>
        /// 指定された個数までのベクトル要素を出力するメソッドです。
        /// </summary>
        /// <param name="Size">出力する要素数</param>
        /// <returns> ベクトル要素</returns>
        public virtual string ToString(int Size)
        {
            if (Length == 0)
                return "要素は空です。";
            else
            {
                int Count = Math.Min(Size, this.Length);

                System.Text.StringBuilder sb = new StringBuilder();

                sb.Append(this.Length + "個の要素\n");
                sb.Append(string.Format("{0,6:F3}", element[0]));
                for (int i = 1; i < Count; i++)
                    sb.Append(string.Format("," + "{0,6:F3}", element[i]));

                if (Count < this.Length)
                    sb.Append(" ... ");

                return sb.ToString();
            }
        }

        /// <summary>
        /// ベクトルを指定したファイル名に保存します。
        /// </summary>
        /// <param name="SaveFileName">保存ファイル名</param>
        public bool Save(string SaveFileName)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.Length; i++)
                sb.Append(this.element[i] + "\n");

            string SaveData = sb.ToString();
            SaveData = SaveData.TrimEnd('\n');

            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(SaveFileName));
                if (!di.Exists)
                    di.Create();

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SaveFileName))
                {
                    sw.Write(SaveData);
                }
            }
            catch (Exception error) { throw new ApplicationException(error.Message); }
            return true;
        }

        /// <summary>
        /// ベクトルファイルを指定したファイルから読み込みます
        /// </summary>
        /// <param name="LoadFileName">読み込みファイル名</param>
        public static Vector Load(string LoadFileName)
        {
            string LoadData;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(LoadFileName))
            {
                LoadData = sr.ReadToEnd();
            }
            LoadData = LoadData.Replace("\r", "");
            LoadData = LoadData.Trim('\n');

            string[] StringElement = LoadData.Split('\n');
            Vector ReturnVector = new Vector(StringElement.Length);
            for (int i = 0; i < ReturnVector.Length; i++)
                ReturnVector[i] = double.Parse(StringElement[i]);
            return ReturnVector;
        }

        /// <summary>
        /// このクラスをバイナリでセーブするメソッド
        /// </summary>
        /// <param name="strSaveFileName">セーブファイル名</param>
        /// <returns>成功ならTrue</returns>
        public virtual bool SaveBinary(string strSaveFileName)
        {
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(strSaveFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, this);
                }
            }
            catch (Exception error) { return false; }
            return true;
        }

        /// <summary>
        /// バイナリデータをロードするメソッド
        /// </summary>
        /// <param name="strLoadFileName">ロードファイル名</param>
        /// <returns>ロードしたデータ</returns>
        public static Vector LoadBinary(string strLoadFileName)
        {
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(strLoadFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (Vector)bf.Deserialize(fs);
                }
            }
            catch (Exception error) { return null; }
        }
        #endregion        
    }
}