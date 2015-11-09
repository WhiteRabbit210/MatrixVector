using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace MatrixVector
{
    /// <summary>
    /// 行列を扱うクラス
    /// </summary>
    [Serializable]
    public class Matrix
    {
        /// <summary>
        /// Matrixが保持しているベクトル（縦ベクトル）
        /// </summary>
        protected ColumnVector[] Vectors;
        protected object _Tag;

        #region コンストラクタ
        /// <summary>
        /// 空の行列を作成します（継承用）
        /// </summary>
        protected Matrix() { }

        /// <summary>
        /// RowSize行×ColSize列の行列を作成します。値は全て0になります。
        /// </summary>
        /// <param name="RowSize">行数</param>
        /// <param name="ColSize">列数</param>
        public Matrix(int RowSize, int ColSize)
        {
            this.Vectors = new ColumnVector[ColSize];
            for (int i = 0; i < ColSize; i++)
                this.Vectors[i] = new ColumnVector(RowSize);
        }

        /// <summary>
        /// 与えられたベクトル配列から行列を作ります。
        /// 全てのベクトルは同じ大きさである必要があります。
        /// 与えられたベクトルは縦ベクトルとして扱われます。
        /// </summary>
        /// <param name="Vectors">ベクトル配列</param>
        public Matrix(Vector[] Vectors)
        {
            for (int i = 1; i < Vectors.Length; i++)
                if (Vectors[i - 1].Length != Vectors[i].Length)
                    throw new ApplicationException("配列の要素数が異なっています");

            this.Vectors = new ColumnVector[Vectors.Length];
            for (int i = 0; i < Vectors.Length; i++)
                this.Vectors[i] = new ColumnVector(Vectors[i]);
        }

        /// <summary>
        /// 与えられた横ベクトル配列から行列を作ります。
        /// 全てのベクトルは同じ大きさである必要があります。
        /// </summary>
        /// <param name="Vectors">横ベクトル配列</param>
        public Matrix(RowVector[] Vectors)
        {
            for (int i = 1; i < Vectors.Length; i++)
                if (Vectors[i - 1].Length != Vectors[i].Length)
                    throw new ApplicationException("配列の要素数が異なっています");

            this.Vectors = new ColumnVector[Vectors[0].Length];
            for (int i = 0; i < this.Vectors.Length; i++)
            {
                this.Vectors[i] = new ColumnVector(Vectors.Length);
                for (int j = 0; j < Vectors.Length; j++)
                    this[j, i] = Vectors[j][i];
            }
        }

        /// <summary>
        /// 与えられた縦ベクトル配列から行列を作ります。
        /// 全てのベクトルは同じ大きさである必要があります。
        /// </summary>
        /// <param name="Vectors">縦ベクトル配列</param>
        public Matrix(ColumnVector[] Vectors)
        {
            for (int i = 1; i < Vectors.Length; i++)
                if (Vectors[i - 1].Length != Vectors[i].Length)
                    throw new ApplicationException("配列の要素数が異なっています");

            this.Vectors = new ColumnVector[Vectors.Length];
            for (int i = 0; i < Vectors.Length; i++)
                this.Vectors[i] = new ColumnVector(Vectors[i]);
        }

        public Matrix(List<ColumnVector> Vectors)
            : this(Vectors.ToArray())
        { }

        public Matrix(List<Vector> Vectors)
            : this(Vectors.ToArray())
        { }

        public Matrix(List<RowVector> Vectors)
            : this(Vectors.ToArray())

        { }
        /// <summary>
        /// 与えられた配列から行列を作ります。
        /// 配列は同じ大きさである必要があります。
        /// </summary>
        /// <param name="DoubleArray">行列の元となる配列</param>
        public Matrix(double[][] DoubleArray)
        {
            for (int i = 1; i < DoubleArray.Length; i++)
                if (DoubleArray[i - 1].Length != DoubleArray[i].Length)
                    throw new ApplicationException("配列の要素数が異なっています");

            this.Vectors = new ColumnVector[DoubleArray.Length];
            for (int i = 0; i < DoubleArray.Length; i++)
                this.Vectors[i] = new ColumnVector(DoubleArray[i]);
        }

        /// <summary>
        /// 与えられた行列と同じ行列を作成します。
        /// </summary>
        /// <param name="matrix">コピー元となる行列</param>
        public Matrix(Matrix matrix)
        {
            Vectors = new ColumnVector[matrix.ColSize];
            for (int i = 0; i < matrix.ColSize; i++)
                Vectors[i] = matrix.GetColVector(i);

            this.Tag = matrix.Tag;
        }
        #endregion

        #region サイズ、ベクトル取得、設定系
        /// <summary>
        /// 列数を取得します
        /// </summary>
        public int ColSize { get { return Vectors.Length; } }

        /// <summary>
        /// 行数を取得します
        /// </summary>
        public int RowSize { get { return Vectors[0].Length; } }

        /// <summary>
        /// 指定した行・列の要素を返します。
        /// </summary>
        /// <param name="Row">指定する行</param>
        /// <param name="Col">指定する列</param>
        /// <returns>要素の値</returns>
        public virtual double GetElement(int Row, int Col) { return Vectors[Col][Row]; }

        /// <summary>
        /// 指定した行・列の要素を設定します。
        /// </summary>
        /// <param name="Row">指定する行</param>
        /// <param name="Col">指定する列</param>
        /// <param name="Value">指定する値</param>
        public virtual void SetElement(int Row, int Col, double Value) { this.Vectors[Col][Row] = Value; }

        /// <summary>
        /// 指定した0から始まるの列番号のベクトル要素を取得するメソッドです。
        /// </summary>
        /// <param name="Index">0から始まる列番号</param>
        /// <returns>指定した列のベクトル要素</returns>
        public virtual ColumnVector GetColVector(int Index) { return new ColumnVector(Vectors[Index]); }

        /// <summary>
        /// 指定した0から始まるの行番号のベクトル要素を取得するメソッドです。
        /// </summary>
        /// <param name="Index">0から始まる行番号</param>
        /// <returns>指定した行のベクトル要素</returns>
        public virtual RowVector GetRowVector(int Index)
        {
            double[] Res = new double[ColSize];
            for (int i = 0; i < ColSize; i++)
                Res[i] = Vectors[i][Index];

            return new RowVector(Res);
        }

        /// <summary>
        /// 行列の要素のコピーを取得します。
        /// </summary>
        public virtual double[][] MatrixElements
        {
            get
            {
                double[][] ReturnDoubleArray = new double[ColSize][];
                for (int i = 0; i < ColSize; i++)
                    ReturnDoubleArray[i] = new double[RowSize];

                for (int i = 0; i < ColSize; i++)
                    for (int j = 0; j < RowSize; j++)
                        ReturnDoubleArray[i][j] = this[j, i];

                return ReturnDoubleArray;
            }
        }

        /// <summary>
        /// 行列の要素を0から始まるインデックスを指定して取得、設定します。
        /// </summary>
        /// <param name="Row">0から始まる行番号</param>
        /// <param name="Col">0から始まる列番号</param>
        /// <returns>指定された行列の要素</returns>
        public double this[int Row, int Col]
        {
            get { return this.Vectors[Col][Row]; }
            set { this.Vectors[Col][Row] = value; }
        }

        public ColumnVector[] ColumnVector
        {
            get { return this.Vectors; }
            set { this.Vectors = value; }
        }
        #endregion

        #region 削除系
        #region 列削除系
        /// <summary>
        /// 指定した列インデックス要素を削除した行列を取得します
        /// </summary>
        /// <param name="ColumnIndex">削除する列インデックス</param>
        /// <returns>指定した列インデックス要素を削除した行列</returns>
        public virtual Matrix RemoveColumnElementAt(int ColumnIndex)
        {
            if (ColumnIndex > this.ColSize)
                throw new ApplicationException("インデックスが範囲外です");

            ColumnVector[] Vectors = new ColumnVector[this.ColSize - 1];

            for (int i = 0; i < ColumnIndex; i++)
                Vectors[i] = this.GetColVector(i);

            for (int i = ColumnIndex; i < Vectors.Length; i++)
                Vectors[i] = this.GetColVector(i + 1);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定した範囲の列要素を削除した行列を取得します
        /// </summary>
        /// <param name="StartIndex">削除開始列インデックス</param>
        /// <param name="EndIndex">削除終了列インデックス</param>
        /// <returns>指定した範囲の列要素を削除した行列</returns>
        public virtual Matrix RemoveColumnElementRange(int StartIndex, int EndIndex)
        {
            if (StartIndex > EndIndex || StartIndex >= this.ColSize || StartIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndIndex < 0 || EndIndex >= this.ColSize)
                throw new ApplicationException("指定位置が正しくありません");

            ColumnVector[] Vectors = new ColumnVector[this.ColSize - (EndIndex - StartIndex) - 1];

            for (int i = 0; i < StartIndex; i++)
                Vectors[i] = this.GetColVector(i);

            for (int i = 1; i < this.ColSize - EndIndex; i++)
                Vectors[i + StartIndex - 1] = this.GetColVector(i + EndIndex);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定した開始列から最後までの列要素を削除した行列を取得します
        /// </summary>
        /// <param name="StartIndex">削除開始列インデックス</param>
        /// <returns>指定した範囲の列要素を削除した行列</returns>
        public virtual Matrix RemoveColumnElementToEnd(int StartIndex)
        {
            return this.RemoveColumnElementRange(StartIndex, this.ColSize - 1);
        }

        /// <summary>
        /// 指定した終了列までの列要素を削除した行列を取得します
        /// </summary>
        /// <param name="EndIndex">削除終了列インデックス</param>
        /// <returns>指定した範囲の列要素を削除した行列</returns>
        public virtual Matrix RemoveColumnElementFromStart(int EndIndex)
        {
            return this.RemoveColumnElementRange(0, EndIndex);
        }
        #endregion

        #region 行削除系
        /// <summary>
        /// 指定した行インデックス要素を削除した行列を取得します
        /// </summary>
        /// <param name="RowIndex">削除する行インデックス</param>
        /// <returns>指定した行インデックス要素を削除した行列</returns>
        public virtual Matrix RemoveRowElemntAt(int RowIndex)
        {
            if (RowIndex > this.RowSize)
                throw new ApplicationException("インデックスが範囲外です");

            RowVector[] Vectors = new RowVector[this.RowSize - 1];

            for (int i = 0; i < RowIndex; i++)
                Vectors[i] = this.GetRowVector(i);

            for (int i = RowIndex; i < Vectors.Length; i++)
                Vectors[i] = this.GetRowVector(i + 1);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定した範囲の行要素を削除した行列を取得します
        /// </summary>
        /// <param name="StartIndex">削除開始行インデックス</param>
        /// <param name="EndIndex">削除終了行インデックス</param>
        /// <returns>指定した範囲の行要素を削除した行列</returns>
        public virtual Matrix RemoveRowElementRange(int StartIndex, int EndIndex)
        {
            if (StartIndex > EndIndex || StartIndex >= this.RowSize || StartIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndIndex < 0 || EndIndex >= this.RowSize)
                throw new ApplicationException("指定位置が正しくありません");

            RowVector[] Vectors = new RowVector[this.RowSize - (EndIndex - StartIndex) - 1];

            for (int i = 0; i < StartIndex; i++)
                Vectors[i] = this.GetRowVector(i);

            for (int i = 1; i < this.RowSize - EndIndex; i++)
                Vectors[i + StartIndex - 1] = this.GetRowVector(i + EndIndex);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定した開始行から最後までの行要素を削除した行列を取得します
        /// </summary>
        /// <param name="StartIndex">削除開始行インデックス</param>
        /// <returns>指定した範囲の行要素を削除した行列</returns>
        public virtual Matrix RemoveRowElementToEnd(int StartIndex)
        {
            return this.RemoveRowElementRange(StartIndex, this.RowSize - 1);
        }

        /// <summary>
        /// 指定した終了行までの行要素を削除した行列を取得します
        /// </summary>
        /// <param name="EndIndex">削除終了行インデックス</param>
        /// <returns>指定した範囲の行要素を削除した行列</returns>
        public virtual Matrix RemoveRowElementFromStart(int EndIndex)
        {
            return this.RemoveRowElementRange(0, EndIndex);
        }
        #endregion
        #endregion

        #region 挿入系
        #region ベクトル挿入系
        /// <summary>
        /// 指定した横ベクトルを指定した位置に挿入します
        /// </summary>
        /// <param name="Index">0から始まるインデックス</param>
        /// <param name="RowVector">挿入する縦ベクトル</param>
        /// <returns>指定したベクトルが挿入された行列</returns>
        public virtual Matrix InsertRowVectorAt(int Index, RowVector RowVector)
        {
            if (this.ColSize != RowVector.Length)
                throw new ApplicationException("要素の数が違います");

            if (Index < 0 || Index > RowSize)
                throw new ApplicationException("インデックスが範囲外です");

            RowVector[] Vectors = new RowVector[this.RowSize + 1];

            for (int i = 0; i < Index; i++)
                Vectors[i] = this.GetRowVector(i);

            Vectors[Index] = RowVector;

            for (int i = Index + 1; i < Vectors.Length; i++)
                Vectors[i] = this.GetRowVector(i - 1);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定した横ベクトルを最後に挿入します
        /// </summary>
        /// <param name="RowVector">挿入する縦ベクトル</param>
        /// <returns>指定したベクトルが挿入された行列</returns>
        public virtual Matrix InsertRowVectorAtEnd(RowVector RowVector)
        {
            return this.InsertRowVectorAt(this.RowSize, RowVector);
        }

        /// <summary>
        /// 指定した横ベクトルを最初に挿入します
        /// </summary>
        /// <param name="RowVector">挿入する縦ベクトル</param>
        /// <returns>指定したベクトルが挿入された行列</returns>
        public virtual Matrix InsertRowVectorAtStart(RowVector RowVector)
        {
            return this.InsertRowVectorAt(0, RowVector);
        }

        /// <summary>
        /// 指定した縦ベクトルを指定した位置に挿入します
        /// </summary>
        /// <param name="Index">0から始まるインデックス</param>
        /// <param name="ColumnVector">挿入する横ベクトル</param>
        /// <returns>指定したベクトルが挿入された行列</returns>
        public virtual Matrix InsertColumnVectorAt(int Index, ColumnVector ColumnVector)
        {
            if (this.RowSize != ColumnVector.Length)
                throw new ApplicationException("要素の数が違います");

            if (Index < 0 || Index > ColSize)
                throw new ApplicationException("インデックスが範囲外です");

            ColumnVector[] Vectors = new ColumnVector[this.ColSize + 1];

            for (int i = 0; i < Index; i++)
                Vectors[i] = this.GetColVector(i);

            Vectors[Index] = ColumnVector;

            for (int i = Index + 1; i < Vectors.Length; i++)
                Vectors[i] = this.GetColVector(i - 1);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定した縦ベクトルを最後に挿入します
        /// </summary>
        /// <param name="ColumnVector">挿入する横ベクトル</param>
        /// <returns>指定したベクトルが挿入された行列</returns>
        public virtual Matrix InsertColumnVectorAtEnd(ColumnVector ColumnVector)
        {
            return this.InsertColumnVectorAt(this.ColSize, ColumnVector);
        }

        /// <summary>
        /// 指定した縦ベクトルを最初に挿入します
        /// </summary>
        /// <param name="ColumnVector">挿入する横ベクトル</param>
        /// <returns>指定したベクトルが挿入された行列</returns>
        public virtual Matrix InsertColumnVectorAtStart(ColumnVector ColumnVector)
        {
            return this.InsertColumnVectorAt(0, ColumnVector);
        }

        public virtual Matrix SetColumnVectorAt(int Index, ColumnVector ColumnVector)
        {
            Matrix ResultMatrix = this.RemoveColumnElementAt(Index);
            return ResultMatrix.InsertColumnVectorAt(Index, ColumnVector);
        }

        public virtual Matrix SetRowVectorAt(int Index, RowVector RowVector)
        {
            Matrix ResultMatrix = this.RemoveRowElemntAt(Index);
            return ResultMatrix.InsertRowVectorAt(Index, RowVector);
        }
        #endregion

        #region 行列挿入系
        /// <summary>
        /// 指定した行の位置に行列を挿入します
        /// </summary>
        /// <param name="RowIndex">0から始まる行のインデックス</param>
        /// <param name="Matrix">挿入する行列</param>
        /// <returns>指定した行位置に行列を挿入された行列</returns>
        public virtual Matrix InsertMatrixAtRowIndex(int RowIndex, Matrix Matrix)
        {
            if (this.ColSize != Matrix.ColSize)
                throw new ApplicationException("列のサイズが異なっています");

            if (RowIndex < 0 | RowIndex > this.RowSize)
                throw new ApplicationException("インデックスが範囲外です");

            RowVector[] Vectors = new RowVector[this.RowSize + Matrix.RowSize];

            for (int i = 0; i < RowIndex; i++)
                Vectors[i] = this.GetRowVector(i);

            for (int i = RowIndex; i < RowIndex + Matrix.RowSize; i++)
                Vectors[i] = Matrix.GetRowVector(i - RowIndex);

            for (int i = RowIndex + Matrix.RowSize; i < Vectors.Length; i++)
                Vectors[i] = this.GetRowVector(i - Matrix.RowSize);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 最終行に指定した行列を挿入します
        /// </summary>
        /// <param name="Matrix">挿入する行列</param>
        /// <returns>最終行に指定した行列が挿入された行列</returns>
        public virtual Matrix InsertMatrixAtRowEnd(Matrix Matrix)
        {
            return this.InsertMatrixAtRowIndex(this.RowSize, Matrix);
        }

        /// <summary>
        /// 行列の最初の行に指定した行列を挿入します
        /// </summary>
        /// <param name="Matrix">挿入する行列</param>
        /// <returns>初行に指定した行列が挿入された行列</returns>
        public virtual Matrix InsertMatrixAtRowStart(Matrix Matrix)
        {
            return this.InsertMatrixAtRowIndex(0, Matrix);
        }

        /// <summary>
        /// 指定した列位置に行列を挿入します
        /// </summary>
        /// <param name="ColumnIndex">0から始まる列のインデックス</param>
        /// <param name="Matrix">挿入する行列</param>
        /// <returns>指定した列位置に行列を挿入した行列</returns>
        public virtual Matrix InsertMatrixAtColumnIndex(int ColumnIndex, Matrix Matrix)
        {
            if (this.RowSize != Matrix.RowSize)
                throw new ApplicationException("行のサイズが異なっています");

            if (ColumnIndex < 0 | ColumnIndex > this.ColSize)
                throw new ApplicationException("インデックスが範囲外です");

            ColumnVector[] Vectors = new ColumnVector[this.ColSize + Matrix.ColSize];

            for (int i = 0; i < ColumnIndex; i++)
                Vectors[i] = this.GetColVector(i);

            for (int i = ColumnIndex; i < ColumnIndex + Matrix.ColSize; i++)
                Vectors[i] = Matrix.GetColVector(i - ColumnIndex);

            for (int i = ColumnIndex + Matrix.ColSize; i < Vectors.Length; i++)
                Vectors[i] = this.GetColVector(i - Matrix.ColSize);

            return new Matrix(Vectors);
        }

        /// <summary>
        /// 最終列に指定した行列を挿入します
        /// </summary>
        /// <param name="Matrix">挿入する行列</param>
        /// <returns>最終列に指定した行列が挿入された行列</returns>
        public virtual Matrix InsertMatrixAtColumnEnd(Matrix Matrix)
        {
            return this.InsertMatrixAtColumnIndex(this.ColSize, Matrix);
        }

        /// <summary>
        /// 行列の最初の列に指定した行列を挿入します
        /// </summary>
        /// <param name="Matrix">挿入する行列</param>
        /// <returns>初列に指定した行列が挿入された行列</returns>
        public virtual Matrix InsertMatrixAtColumnStart(Matrix Matrix)
        {
            return this.InsertMatrixAtColumnIndex(0, Matrix);
        }
        #endregion
        #endregion

        #region 部分行列取得系
        /// <summary>
        /// 指定された列範囲の部分行列を取得します
        /// </summary>
        /// <param name="StartColumnIndex">開始列</param>
        /// <param name="EndColumnIndex">終了列</param>
        /// <returns>指定された列範囲の部分行列</returns>
        public virtual Matrix GetMatrixColumnBetween(int StartColumnIndex, int EndColumnIndex)
        {
            if (StartColumnIndex > EndColumnIndex || StartColumnIndex >= this.ColSize || StartColumnIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndColumnIndex < 0 || EndColumnIndex >= this.ColSize)
                throw new ApplicationException("指定位置が正しくありません");

            ColumnVector[] Vectors = new ColumnVector[EndColumnIndex - StartColumnIndex + 1];
            for (int i = StartColumnIndex; i <= EndColumnIndex; i++)
                Vectors[i - StartColumnIndex] = this.GetColVector(i);
            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定した開始列から最終列までの部分行列を取得します
        /// </summary>
        /// <param name="StartColumnIndex">開始列</param>
        /// <returns>指定した開始列から最終列までの部分行列</returns>
        public virtual Matrix GetMatrixColumnToEnd(int StartColumnIndex)
        {
            return this.GetMatrixColumnBetween(StartColumnIndex, this.ColSize - 1);
        }

        /// <summary>
        /// 指定した終了列までの部分行列を取得します
        /// </summary>
        /// <param name="EndColumnIndex">終了列</param>
        /// <returns>指定した終了列までの部分行列</returns>
        public virtual Matrix GetMatrixColumnFromStart(int EndColumnIndex)
        {
            return this.GetMatrixColumnBetween(0, EndColumnIndex);
        }

        /// <summary>
        /// 指定された行範囲の部分行列を取得します
        /// </summary>
        /// <param name="StartRowIndex">開始行</param>
        /// <param name="EndRowIndex">終了行</param>
        /// <returns>指定された行範囲の部分行列</returns>
        public virtual Matrix GetMatrixRowBetween(int StartRowIndex, int EndRowIndex)
        {
            if (StartRowIndex > EndRowIndex || StartRowIndex >= this.RowSize || StartRowIndex < 0)
                throw new ApplicationException("指定位置が正しくありません");
            if (EndRowIndex < 0 || EndRowIndex >= this.RowSize)
                throw new ApplicationException("指定位置が正しくありません");

            RowVector[] Vectors = new RowVector[EndRowIndex - StartRowIndex + 1];
            for (int i = StartRowIndex; i <= EndRowIndex; i++)
                Vectors[i - StartRowIndex] = this.GetRowVector(i);
            return new Matrix(Vectors);
        }

        /// <summary>
        /// 指定された開始行から最終行までの部分行列を取得します
        /// </summary>
        /// <param name="StartRowIndex">開始行</param>
        /// <returns>指定された開始行から最終行までの部分行列</returns>
        public virtual Matrix GetMatrixRowToEnd(int StartRowIndex)
        {
            return this.GetMatrixRowBetween(StartRowIndex, this.RowSize - 1);
        }

        /// <summary>
        /// 指定された終了行までの部分行列を取得します
        /// </summary>
        /// <param name="EndRowIndex">終了行</param>
        /// <returns>指定された終了行までの部分行列</returns>
        public virtual Matrix GetMatrixRowFromStart(int EndRowIndex)
        {
            return this.GetMatrixRowBetween(0, EndRowIndex);
        }

        public virtual Matrix GetIndexColumnVectorMatrix(List<int> Index)
        { return GetIndexColumnVectorMatrix(Index.ToArray()); }

        public virtual Matrix GetIndexColumnVectorMatrix(int[] Index)
        {
            ColumnVector[] ResultVector = new ColumnVector[Index.Length];
            for (int i = 0; i < ResultVector.Length; i++)
                ResultVector[i] = this.GetColVector(Index[i]);

            return new Matrix(ResultVector);
        }


        public virtual Matrix GetIndexRowVectorMatrix(List<int> Index)
        { return GetIndexRowVectorMatrix(Index.ToArray()); }

        public virtual Matrix GetIndexRowVectorMatrix(int[] Index)
        {
            RowVector[] ResultVector = new RowVector[Index.Length];
            for (int i = 0; i < ResultVector.Length; i++)
                ResultVector[i] = this.GetRowVector(Index[i]);

            return new Matrix(ResultVector);
        }
        #endregion

        #region 特殊取得系



        public static Matrix GetIMatrix(int Size)
        {
            Matrix ResultMatrix = new Matrix(Size, Size);
            for (int i = 0; i < Size; i++)
                ResultMatrix[i, i] = 1;
            return ResultMatrix;
        }

        /// <summary>
        /// 行列の縦ベクトル要素をすべて連結した縦ベクトルを取得します
        /// </summary>
        /// <returns>連結された縦ベクトル</returns>
        public virtual ColumnVector GetConbineColumnVector()
        {
            ColumnVector ResultVector = this.GetColVector(0);

            for (int i = 1; i < this.ColSize; i++)
                ResultVector = new ColumnVector(ResultVector.InsertAtEnd(this.GetColVector(i)));

            return ResultVector;

        }

        /// <summary>
        /// 行列の横ベクトル要素をすべて連結した横ベクトルを取得します（涼子作成）
        /// </summary>
        /// <returns>連結された横ベクトル</returns>
        public virtual RowVector GetConbineRowVector()
        {
            RowVector ResultVector = this.GetRowVector(0);

            for (int i = 1; i < this.RowSize; i++)
                ResultVector = new RowVector(ResultVector.InsertAtEnd(this.GetRowVector(i)));

            return ResultVector;

        }

        /// <summary>
        /// 転置行列を取得します。
        /// </summary>
        /// <returns>転置された行列</returns>
        public virtual Matrix GetTranspose()
        {
            Vector[] CulcVec = new Vector[this.RowSize];
            for (int i = 0; i < this.RowSize; i++)
                CulcVec[i] = this.GetRowVector(i);

            return new Matrix(CulcVec);
        }

        /// <summary>
        /// 列方向に平均を取った値をベクトルで取得します。
        /// 取得したベクトルは横ベクトルになります。
        /// </summary>
        /// <returns>列方向の平均ベクトル</returns>
        public virtual RowVector GetAverageCol()
        {
            double[] Average = new double[ColSize];
            for (int i = 0; i < ColSize; i++)
                Average[i] = Vectors[i].GetAverage();

            return new RowVector(Average);
        }

        /// <summary>
        /// 行方向に平均を取った値をベクトルで取得します。
        /// 取得したベクトルは縦ベクトルになります。
        /// </summary>
        /// <returns>行方向の平均ベクトル</returns>
        public virtual ColumnVector GetAverageRow()
        {
            return new ColumnVector(this.GetTranspose().GetAverageCol());
        }

        /// <summary>
        /// 列方向のノルムを1にした行列を返します。
        /// </summary>
        /// <returns>列方向のノルムが1の行列</returns>
        public virtual Matrix GetNormalizedMatrixCol()
        {
            ColumnVector[] ReturnVector = new ColumnVector[ColSize];

            //現在の行列の要素ベクトルの要素を1にしたベクトルを取得
            for (int i = 0; i < ColSize; i++)
                ReturnVector[i] = Vectors[i].GetNormlizeVector();

            return new Matrix(ReturnVector);
        }

        /// <summary>
        /// 列方向のノルムを1にした行列を返します。
        /// </summary>
        /// <returns>列方向のノルムが1の行列</returns>
        public virtual Matrix GetNormalizedMatrixRow()
        {
            // 転置→正規化→転置
            Matrix TransposedMatrix = this.GetTranspose();
            Matrix NormalizedMatrix = TransposedMatrix.GetNormalizedMatrixCol();

            return NormalizedMatrix.GetTranspose();
        }

        /// <summary>
        /// 指定した個数の列ベクトルが並んだ行列を取得します。
        /// 主に平均ベクトルに対して使ったりします。
        /// </summary>
        /// <param name="Vector">列ベクトル</param>
        /// <param name="Size">個数</param>
        /// <returns>指定した個数の列ベクトルが並んだ行列</returns>
        public static Matrix GetSameElementMatrix(ColumnVector Vector, int Size)
        {
            return Matrix.GetSameElementMatrix((Vector)Vector, Size);
        }

        /// <summary>
        /// 指定した個数の行ベクトルが並んだ行列を取得します。
        /// 主に平均ベクトルに対して使ったりします。
        /// </summary>
        /// <param name="Vector">行ベクトル</param>
        /// <param name="Size">個数</param>
        /// <returns>指定した個数の行ベクトルが並んだ行列</returns>
        public static Matrix GetSameElementMatrix(RowVector Vector, int Size)
        {
            return Matrix.GetSameElementMatrix((Vector)Vector, Size).GetTranspose();
        }

        /// <summary>
        /// 指定した個数の行ベクトルが並んだ行列を取得します。
        /// </summary>
        /// <param name="Vector">ベクトル</param>
        /// <param name="Size">個数</param>
        /// <returns>指定した個数のベクトルが並んだ行列</returns>
        private static Matrix GetSameElementMatrix(Vector Vector, int Size)
        {
            ColumnVector[] MatrixElement = new ColumnVector[Size];
            for (int i = 0; i < Size; i++)
                MatrixElement[i] = new ColumnVector(Vector);
            return new Matrix(MatrixElement);
        }

        public List<List<double>> GetElementList()
        {
            List<List<double>> ResultList = new List<List<double>>();
            for (int i = 0; i < this.ColSize; i++)
                ResultList.Add(this.GetColVector(i).GetElementList());

            return ResultList;
        }
        #endregion

        #region オペレーターオーバーロード
        public static Matrix operator *(Matrix Matrix, double Scalar)
        {
            Matrix ReturnMatrix = new Matrix(Matrix.RowSize, Matrix.ColSize);

            for (int Row = 0; Row < Matrix.RowSize; Row++)
                for (int Col = 0; Col < Matrix.ColSize; Col++)
                    ReturnMatrix[Row, Col] = Matrix[Row, Col] * Scalar;
            return ReturnMatrix;
        }

        public static Matrix operator *(double Scalar, Matrix Matrix)
        {
            Matrix ReturnMatrix = new Matrix(Matrix.RowSize, Matrix.ColSize);

            for (int Row = 0; Row < Matrix.RowSize; Row++)
                for (int Col = 0; Col < Matrix.ColSize; Col++)
                    ReturnMatrix[Row, Col] = Matrix[Row, Col] * Scalar;
            return ReturnMatrix;
        }

        public static Matrix operator /(Matrix LeftMatrix, double Scalar)
        {
            Matrix ReturnMatrix = new Matrix(LeftMatrix.RowSize, LeftMatrix.ColSize);

            for (int Row = 0; Row < LeftMatrix.RowSize; Row++)
                for (int Col = 0; Col < LeftMatrix.ColSize; Col++)
                    ReturnMatrix[Row, Col] = LeftMatrix[Row, Col] / Scalar;
            return ReturnMatrix;
        }

        public static RowVector operator *(RowVector vector, Matrix matrix)
        {
            //行のサイズとベクトルのサイズが一致してないと駄目
            if (vector.Length != matrix.RowSize)
                throw new ApplicationException("要素の数が一致しません。");

            double[] Culc = new double[matrix.ColSize];
            for (int i = 0; i < matrix.ColSize; i++)
                for (int j = 0; j < matrix.RowSize; j++)
                    Culc[i] += vector[j] * matrix[j, i];

            return new RowVector(Culc);
        }

        public static ColumnVector operator *(Matrix matrix, ColumnVector vector)
        {
            //列のサイズとベクトルのサイズが一致してないと駄目
            if (vector.Length != matrix.ColSize)
                throw new ApplicationException("要素の数が一致しません。");

            double[] Culc = new double[matrix.RowSize];
            for (int i = 0; i < matrix.RowSize; i++)
                for (int j = 0; j < matrix.ColSize; j++)
                    Culc[i] += vector[j] * matrix[i, j];

            return new ColumnVector(Culc);
        }


        /// <summary>
        /// 行列と行列とのかけ算
        /// </summary>
        /// <param name="LeftMatrix">左からかける行列</param>
        /// <param name="RightMatrix">右からかける行列</param>
        /// <returns>答えの行列</returns>
        public static Matrix operator *(Matrix LeftMatrix, Matrix RightMatrix)
        {
            if (LeftMatrix.ColSize != RightMatrix.RowSize)
                throw new ApplicationException("かけ算を行うための要素の数が一致しません");

            Matrix ReturnMatrix = new Matrix(LeftMatrix.RowSize, RightMatrix.ColSize);

            //かけ算処理
            for (int Row = 0; Row < ReturnMatrix.RowSize; Row++)
                for (int Col = 0; Col < ReturnMatrix.ColSize; Col++)
                {
                    double SetValue = LeftMatrix.GetRowVector(Row) * RightMatrix.GetColVector(Col);
                    ReturnMatrix[Row, Col] = SetValue;
                }

            return ReturnMatrix;
        }

        public static Matrix operator +(Matrix LeftMatrix, Matrix RightMatrix)
        {
            if (LeftMatrix.ColSize != RightMatrix.ColSize || LeftMatrix.RowSize != RightMatrix.RowSize)
                throw new ApplicationException("左右の行列の大きさが一致しません");
            Matrix ReturnMatrix = new Matrix(LeftMatrix.RowSize, LeftMatrix.ColSize);

            for (int Row = 0; Row < ReturnMatrix.RowSize; Row++)
                for (int Col = 0; Col < ReturnMatrix.ColSize; Col++)
                {
                    double SetValue = LeftMatrix[Row, Col] + RightMatrix[Row, Col];
                    ReturnMatrix[Row, Col] = SetValue;
                }
            return ReturnMatrix;

        }

        public static Matrix operator -(Matrix LeftMatrix, Matrix RightMatrix)
        {
            if (LeftMatrix.ColSize != RightMatrix.ColSize || LeftMatrix.RowSize != RightMatrix.RowSize)
                throw new ApplicationException("左右の行列の大きさが一致しません");

            Matrix ReturnMatrix = new Matrix(LeftMatrix.RowSize, LeftMatrix.ColSize);

            for (int Row = 0; Row < ReturnMatrix.RowSize; Row++)
                for (int Col = 0; Col < ReturnMatrix.ColSize; Col++)
                {
                    double SetValue = LeftMatrix[Row, Col] - RightMatrix[Row, Col];
                    ReturnMatrix[Row, Col] = SetValue;
                }
            return ReturnMatrix;

        }
        #endregion

        public virtual Matrix GetMultiply(Matrix TargetMatrix)
        {
            return Matrix.GetMultiply(this, TargetMatrix);
        }

        public static Matrix GetMultiply(Matrix LeftMatrix, Matrix RightMatrix)
        {
            if (LeftMatrix.ColSize != RightMatrix.ColSize || LeftMatrix.RowSize != RightMatrix.RowSize)
                throw new ApplicationException("行列のサイズが合っていません");

            Matrix ResultMatrix = new Matrix(LeftMatrix.RowSize, LeftMatrix.ColSize);

            for (int Col = 0; Col < LeftMatrix.ColSize; Col++)
                for (int Row = 0; Row < LeftMatrix.RowSize; Row++)
                    ResultMatrix[Row, Col] = LeftMatrix[Row, Col] * RightMatrix[Row, Col];

            return ResultMatrix;
        }

        /// <summary>
        /// 現在の行数と列数を出力します
        /// </summary>
        /// <returns>「this.RowSize行 this.ColSize列」</returns>
        public override string ToString()
        {
            return this.RowSize + "行 " + this.ColSize + "列";
        }

        public string WriteLog()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this.ColSize; i++)
            {
                for (int j = 0; j < this.RowSize; j++)
                    sb.Append(this[j, i].ToString() + " ");
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// このクラスをCSVでセーブするメソッド
        /// </summary>
        /// <param name="SaveFileName">セーブファイル名</param>
        /// <returns>成功ならTrue</returns>
        public virtual bool SaveCSV(string SaveFileName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int col = 0; col < this.ColSize; col++)
                {
                    for (int row = 0; row < this.RowSize; row++)
                        sb.Append(this[row, col] + ",");

                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(Environment.NewLine);
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SaveFileName))
                {
                    sw.Write(sb.ToString());
                }
                return true;
            }
            catch (Exception error)
            {
                return false;
            }
        }

        /// <summary>
        /// このクラスをCSVでセーブするメソッド
        /// </summary>
        /// <param name="SaveFileName">セーブファイル名</param>
        /// <returns>成功ならTrue</returns>
        public virtual bool SaveCSVWithLabel(string SaveFileName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                for (int col = 0; col < this.ColSize; col++)
                {
                    for (int row = 0; row < this.RowSize; row++)
                        sb.Append(this[row, col] + ",");

                    sb.Remove(sb.Length - 2, 1);
                    sb.Append(Environment.NewLine);
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SaveFileName))
                {
                    sw.Write(sb.ToString());
                }
                return true;
            }
            catch (Exception error)
            {
                return false;
            }
        }

        /// <summary>
        /// csvファイルのデータを読み込んで行列(Matrix)として返す関数
        /// </summary>
        /// <param name="FileName">読み込むファイルの名前</param>
        /// <returns>読み込んだデータをMatrix型に入れたもの</returns>
        public static Matrix LoadCSV(string FileName)
        {
            List<ColumnVector> Vectors = new List<ColumnVector>();
            string[] Datas;

            using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName))
            {
                string Data = sr.ReadToEnd();//与えられたFileNameの中身を全部読み込む
                Data = Data.Replace("\r", "");//\rがあれば消す(空白に置き換える)
                Data = Data.TrimEnd('\n');//末尾の\nを消す
                Datas = Data.Split('\n');//改行ごとに分割して配列に入れる
            }

            foreach (string Data in Datas)
            {
                string[] sValues = Data.Split(',');//Dateを,で分割して配列に入れていく
                ColumnVector Vector = new ColumnVector(sValues.Length);//縦ベクトル

                for (int i = 0; i < sValues.Length; i++)
                    Vector[i] = double.Parse(sValues[i]);

                Vectors.Add(Vector);//リストに加えていく。縦ベクトルが繋がってく感じ?
            }
            return new Matrix(Vectors.ToArray());
        }

        /// <summary>
        /// csvファイルのデータを読み込んで行列(Matrix)として返す関数
        /// </summary>
        /// <param name="FileName">読み込むファイルの名前</param>
        /// <returns>読み込んだデータをMatrix型に入れたもの</returns>
        public static Matrix LoadCSVWithLabel(string FileName)
        {
            List<ColumnVector> Vectors = new List<ColumnVector>();
            string[] Datas;

            using (System.IO.StreamReader sr = new System.IO.StreamReader(FileName))
            {
                string Data = sr.ReadToEnd();//与えられたFileNameの中身を全部読み込む
                Data = Data.Replace("\r", "");//\rがあれば消す(空白に置き換える)
                Data = Data.TrimEnd('\n');//末尾の\nを消す
                Datas = Data.Split('\n');//改行ごとに分割して配列に入れる
            }

            foreach (string Data in Datas)
            {
                string[] sValues = Data.Split(',');//Dateを,で分割して配列に入れていく
                ColumnVector Vector = new ColumnVector(sValues.Length);//縦ベクトル

                for (int i = 0; i < sValues.Length; i++)
                    Vector[i] = double.Parse(sValues[i]);

                Vectors.Add(Vector);//リストに加えていく。縦ベクトルが繋がってく感じ?
            }
            return new Matrix(Vectors.ToArray());
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
        public static Matrix LoadBinary(string strLoadFileName)
        {
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(strLoadFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (Matrix)bf.Deserialize(fs);
                }
            }
            catch (Exception error) { return null; }
        }

        public object Tag
        {
            get { return this._Tag; }
            set { this._Tag = value; }
        }


    }
}
