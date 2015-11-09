using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixVector
{
    /// <summary>
    /// 対称行列のクラス
    /// </summary>
    [Serializable]
    public class SymmetricMatrix : SquareMatrix
    {
        #region コンストラクタ
        /// <summary>
        /// 空の対称行列を作成します
        /// </summary>
        protected SymmetricMatrix() { }

        /// <summary>
        /// 与えられたベクトル配列から対称行列を作成します
        /// </summary>
        /// <param name="Vectors">ベクトル配列</param>
        public SymmetricMatrix(Vector[] Vectors)
            : base(Vectors)
        {
            if (!this.CheckElement())
                throw new ApplicationException("行列が対称行列ではありません");
        }

        /// <summary>
        /// 与えられた配列から対称行列を作成します。
        /// </summary>
        /// <param name="DoubleArray">行列の元となる配列</param>
        public SymmetricMatrix(double[][] DoubleArray)
            : base(DoubleArray)
        {
            if (!this.CheckElement())
                throw new ApplicationException("行列が対称行列ではありません");
        }

        /// <summary>
        /// 与えられた行列から対称行列を作成します。
        /// </summary>
        /// <param name="matrix">行列</param>
        public SymmetricMatrix(Matrix matrix)
            : base(matrix)
        {
            if (!this.CheckElement())
                throw new ApplicationException("行列が対称行列ではありません");
        }

        /// <summary>
        /// 与えられた対称行列と同一の対称行列を作成します。
        /// </summary>
        /// <param name="matrix">コピー元の対称行列</param>
        public SymmetricMatrix(SymmetricMatrix matrix)
            : base(matrix)
        {
        }

        /// <summary>
        /// 与えられた次元数からなる対称行列を作成します。
        /// </summary>
        /// <param name="Dimension"></param>
        public SymmetricMatrix(int Dimension)
            : base(Dimension)
        {
        }
        #endregion

        /// <summary>
        /// 現在の行列が対称行列かを調べます。
        /// </summary>
        /// <returns>Checkの結果</returns>
        private bool CheckElement()
        {
            for (int row = 0; row < this.RowSize; row++)
                for (int col = 0; col < this.ColSize; col++)
                    if (this.Vectors[row][col] != this.Vectors[col][row])
                        return  false;

            return true;
        }

        /// <summary>
        /// 固有値と固有ベクトルを取得します。
        /// </summary>
        /// <param name="EPS">許容相対誤差</param>
        /// <returns>固有値と固有ベクトル</returns>
        public override EigenSystem GetEigenVectorAndValue(double EPS)
        {
            //現在の行列を設定（後に固有ベクトルが入る）
            double[][] EigenValueMatrix = this.MatrixElements;
            //単位行列を設定（後に固有値が入る）
            double[][] EigenVectorMatrix = SquareMatrix.IdentityMatrix(this.Dimension).MatrixElements;

            double Max = double.MaxValue;

            do
            {
                #region 固有値、固有ベクトルの繰り返し計算
                //α、β、γ
                double Alpha, Beta, Gamma;

                double Sin, Cos, Work;

                //一時的にpp,pq,qqを格納
                double Temp_A, Temp_B, Temp_C;

                int MaxRow ;
                int MaxCol ;

                //現在の行列の中で最大のものを取得。
                //MaxRowとMaxColの中に最大値、Maxに最大値が入る
                Max = this.GetMaxValue(EigenValueMatrix, out MaxRow, out MaxCol);

                //ピボットを決めて回転
                Temp_A = EigenValueMatrix[MaxRow][MaxRow];
                Temp_B = EigenValueMatrix[MaxRow][MaxCol];
                Temp_C = EigenValueMatrix[MaxCol][MaxCol];

                Alpha = -Temp_B;
                Beta = (Temp_A - Temp_C) / 2;
                Gamma = Math.Abs(Beta) / Math.Sqrt(Alpha * Alpha + Beta * Beta);

                Sin = Math.Sqrt((1.0 - Gamma) / 2);
                if (Alpha * Beta < 0) Sin = -Sin;
                Cos = Math.Sqrt(1.0 - Sin * Sin);

                for (int j = 0; j < EigenValueMatrix.Length; j++)
                {
                    Work = EigenValueMatrix[MaxRow][j] * Cos - EigenValueMatrix[MaxCol][j] * Sin;
                    EigenValueMatrix[MaxCol][j] = EigenValueMatrix[MaxRow][j] * Sin + EigenValueMatrix[MaxCol][j] * Cos;
                    EigenValueMatrix[MaxRow][j] = Work;
                }

                for (int j = 0; j < EigenValueMatrix.Length; j++)
                {
                    EigenValueMatrix[j][MaxRow] = EigenValueMatrix[MaxRow][j];
                    EigenValueMatrix[j][MaxCol] = EigenValueMatrix[MaxCol][j];
                }

                Work = 2.0 * Temp_B * Sin * Cos;
                EigenValueMatrix[MaxRow][MaxRow] = Temp_A * Cos * Cos + Temp_C * Sin * Sin - Work;
                EigenValueMatrix[MaxCol][MaxCol] = Temp_A * Sin * Sin + Temp_C * Cos * Cos + Work;
                EigenValueMatrix[MaxRow][MaxCol] = EigenValueMatrix[MaxCol][MaxRow] = 0;


                // 固有ベクトルの導出 
                for (int i = 0; i < EigenValueMatrix.Length; i++)
                {
                    Work = EigenVectorMatrix[i][MaxRow] * Cos - EigenVectorMatrix[i][MaxCol] * Sin;
                    EigenVectorMatrix[i][MaxCol] = EigenVectorMatrix[i][MaxRow] * Sin + EigenVectorMatrix[i][MaxCol] * Cos;
                    EigenVectorMatrix[i][MaxRow] = Work;
                }
                #endregion
            } while (EPS < Max);
            //許容相対誤差以下になるまで繰り返して、抜けたら計算終了

            //固有ベクトル、固有値をセットする処理
            EigenSystem EigenData = new EigenSystem();
            Matrix ResultVectorMatrix = new Matrix(EigenVectorMatrix);
            for (int i = 0; i < this.Dimension; i++)
            {
                //固有ベクトルをセット
                Vector EigenVector = ResultVectorMatrix.GetRowVector(i);
                //固有値をセット
                double EigenValue = EigenValueMatrix[i][i];

                if (EigenValue > EPS)
                {
                    //保持するクラスにセット後、固有ベクトルの大きさを正規化
                    EigenData.Add(new EigenVectorAndValue(EigenVector, EigenValue));
                    EigenData[i].Normlize();
                }
                else
                {
                    //固有値が小さすぎたら固有値0、固有ベクトル0にしておく。
                    EigenData.Add(new EigenVectorAndValue(new Vector(EigenVector.Length), 0d));
                }
            }

            return EigenData;
        }


        /// <summary>
        /// 指定された配列の中で絶対値が最大のものを取得します。
        /// </summary>
        /// <param name="WorkMatrix">探索する配列</param>
        /// <param name="MaxRow">最大値の行</param>
        /// <param name="MaxCol">最大値の列</param>
        /// <returns>最大値</returns>
        private double GetMaxValue(double[][] WorkMatrix, out int MaxRow, out int MaxCol)
        {
            double Max = 0.0;
            MaxRow = -1;
            MaxCol = -1;

            for (int i = 0; i < WorkMatrix.Length - 1; i++)
                // 対角成分を検索するのでdim-1は無視 
                for (int j = i + 1; j < WorkMatrix.Length; j++)
                    if (Math.Abs(WorkMatrix[i][j]) > Max)
                    {
                        MaxRow = i;
                        MaxCol = j;
                        Max = Math.Abs(WorkMatrix[i][j]);
                    }

            return Max;
        }
    }
}
