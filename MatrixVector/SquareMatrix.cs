using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixVector
{

    /// <summary>
    /// 正方行列のクラス
    /// </summary>
    [Serializable]
    public class SquareMatrix : Matrix
    {
        #region コンストラクタ
        /// <summary>
        /// 空の正方行列を作成します。
        /// </summary>
        protected SquareMatrix() { }

        /// <summary>
        /// 与えられたベクトル配列から正方行列を作成します。
        /// </summary>
        /// <param name="Vectors">ベクトル配列</param>
        public SquareMatrix(Vector[] Vectors)
            : base(Vectors)
        {
            if (this.ColSize != this.RowSize)
                throw new ApplicationException("行と列の大きさが違います");
        }

        /// <summary>
        /// 与えられた配列から正方行列を作成します。
        /// </summary>
        /// <param name="DoubleArray">配列</param>
        public SquareMatrix(double[][] DoubleArray)
            : base(DoubleArray)
        {
            if (this.ColSize != this.RowSize)
                throw new ApplicationException("行と列の大きさが違います");
        }

        /// <summary>
        /// 与えられた行列から正方行列を作成します。
        /// </summary>
        /// <param name="matrix">行列</param>
        public SquareMatrix(Matrix matrix)
            : base(matrix)
        {
            if (this.ColSize != this.RowSize)
                throw new ApplicationException("行と列の大きさが違います");
        }

        /// <summary>
        /// 与えられた次元数からなる正方行列を作成します。
        /// </summary>
        /// <param name="Dimension">次元数</param>
        public SquareMatrix(int Dimension)
            : base(Dimension, Dimension)
        {
        }
        #endregion

        /// <summary>
        /// 許容相対誤差を指定して固有ベクトルと固有値を取得します。
        /// このメソッドはオーバーライド、もしくは個別に実装して下さい。
        /// </summary>
        /// <param name="EPS">許容相対誤差</param>
        /// <returns>固有ベクトル</returns>
        public virtual EigenSystem GetEigenVectorAndValue(double EPS)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// デフォルトの許容相対誤差で固有ベクトルと固有値を取得します。
        /// </summary>
        /// <returns>固有ベクトル</returns>
        public virtual EigenSystem GetEigenVectorAndValue()
        {
            double EPS = 0.00001;
            return GetEigenVectorAndValue(EPS);
        }

        /// <summary>
        /// 指定した階数の単位行列を取得します。
        /// </summary>
        /// <param name="Rank">階数</param>
        /// <returns>単位行列</returns>
        public static SquareMatrix IdentityMatrix(int Rank)
        {
            SquareMatrix ReturnMatrix = new SquareMatrix(Rank);
            for (int i = 0; i < Rank; i++)
                ReturnMatrix[i, i] = 1d;
            return ReturnMatrix;
        }

        /// <summary>
        /// 行列の次元数を取得します。
        /// </summary>
        public int Dimension
        {
            get { return this.RowSize; }
        }

        /// <summary>
        /// 逆行列を取得します。
        /// </summary>
        /// <param name="SquareMatrix">元になる正方行列</param>
        /// <returns>逆行列</returns>
        public SquareMatrix GetInverseMatrix()
        {
            int i, j, k, matrixLength = base.Vectors.Length;
            //元の行列
            SquareMatrix BaseMatrix = new SquareMatrix(base.Vectors);
            //逆行列が入るベクトル。今は単位行列。
            SquareMatrix InverseMatrix = SquareMatrix.IdentityMatrix(matrixLength);
            double temp;

            for (i = 0; i < matrixLength; i++)
            {
                temp = 1 / BaseMatrix[i,i];
                for (j = 0; j < matrixLength; j++)
                {
                    BaseMatrix[i, j] *= temp;
                    InverseMatrix[i, j] *= temp;
                }

                for (j = 0; j < matrixLength; j++)
                {
                    if (i != j)
                    {
                        temp = BaseMatrix[j, i];
                        for (k = 0; k < matrixLength; k++)
                        {
                            BaseMatrix[j, k] -= BaseMatrix[i, k] * temp;
                            InverseMatrix[j, k] -= InverseMatrix[i, k] * temp;
                        }
                    }
                }
            }

            return InverseMatrix;
        }

        /// <summary>
        /// 行列式の値を取得
        /// </summary>
        /// <returns>行列式の値</returns>
        public double GetDet()
        {
            double det = 1d;
            int dim = this.ColSize;
            SquareMatrix WorkMatrix = new SquareMatrix(this);

            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                    if (i < j)
                    {
                        double buf = WorkMatrix[j, i] / WorkMatrix[i, i];
                        for (int k = 0; k < dim; k++)
                            WorkMatrix[j, k] -= WorkMatrix[i, k] * buf;
                    }

            //対角部分の積
            for (int i = 0; i < dim; i++)
                det *= WorkMatrix[i, i];

            return det;
        }

        #region オペレーターオーバーロード
        public static SquareMatrix operator +(SquareMatrix LeftMatrix, SquareMatrix RightMatrix)
        {
            if (LeftMatrix.ColSize != RightMatrix.ColSize || LeftMatrix.RowSize != RightMatrix.RowSize)
                throw new ApplicationException("左右の行列の大きさが一致しません");
            SquareMatrix ReturnMatrix = new SquareMatrix(LeftMatrix.RowSize);

            for (int Row = 0; Row < ReturnMatrix.RowSize; Row++)
                for (int Col = 0; Col < ReturnMatrix.ColSize; Col++)
                {
                    double SetValue = LeftMatrix[Row, Col] + RightMatrix[Row, Col];
                    ReturnMatrix[Row, Col] = SetValue;
                }
            return ReturnMatrix;

        }

        public static SquareMatrix operator -(SquareMatrix LeftMatrix, SquareMatrix RightMatrix)
        {
            if (LeftMatrix.ColSize != RightMatrix.ColSize || LeftMatrix.RowSize != RightMatrix.RowSize)
                throw new ApplicationException("左右の行列の大きさが一致しません");

            SquareMatrix ReturnMatrix = new SquareMatrix(LeftMatrix.RowSize);

            for (int Row = 0; Row < ReturnMatrix.RowSize; Row++)
                for (int Col = 0; Col < ReturnMatrix.ColSize; Col++)
                {
                    double SetValue = LeftMatrix[Row, Col] - RightMatrix[Row, Col];
                    ReturnMatrix[Row, Col] = SetValue;
                }
            return ReturnMatrix;

        }

        public static SquareMatrix operator *(SquareMatrix Matrix, double Scalar)
        {
            SquareMatrix ReturnMatrix = new SquareMatrix(Matrix.RowSize);

            for (int Row = 0; Row < Matrix.RowSize; Row++)
                for (int Col = 0; Col < Matrix.ColSize; Col++)
                    ReturnMatrix[Row, Col] = Matrix[Row, Col] * Scalar;
            return ReturnMatrix;
        }

        public static SquareMatrix operator *(double Scalar, SquareMatrix Matrix)
        {
            SquareMatrix ReturnMatrix = new SquareMatrix(Matrix.RowSize);

            for (int Row = 0; Row < Matrix.RowSize; Row++)
                for (int Col = 0; Col < Matrix.ColSize; Col++)
                    ReturnMatrix[Row, Col] = Matrix[Row, Col] * Scalar;
            return ReturnMatrix;
        }

        public static SquareMatrix operator /(SquareMatrix LeftMatrix, double Scalar)
        {
            SquareMatrix ReturnMatrix = new SquareMatrix(LeftMatrix.RowSize);

            for (int Row = 0; Row < LeftMatrix.RowSize; Row++)
                for (int Col = 0; Col < LeftMatrix.ColSize; Col++)
                    ReturnMatrix[Row, Col] = LeftMatrix[Row, Col] / Scalar;
            return ReturnMatrix;
        }

        public static RowVector operator *(RowVector vector, SquareMatrix matrix)
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

        public static ColumnVector operator *(SquareMatrix matrix, ColumnVector vector)
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
        #endregion

    }
}
