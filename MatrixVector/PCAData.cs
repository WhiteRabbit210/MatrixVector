using System;
using MatrixVector;
using System.Runtime.Serialization.Formatters.Binary;

namespace MatrixVector
{
    /// <summary>
    /// 主成分分析のデータを保持・保存・読み込みをするクラス
    /// </summary>
    [Serializable]
    public class PCAData
    {
        #region プライベート変数
        /// <summary>
        /// 固有値・固有ベクトルを保持
        /// </summary>
        protected EigenSystem EigenSystemData;

        /// <summary>
        /// 展開係数を保持
        /// </summary>
        protected Matrix CoefficientMatrix;

        /// <summary>
        /// 平均ベクトルを保持（Vector型から変更したので注意してね！！）
        /// </summary>
//        protected ColumnVector AverageVector;
        protected Vector AverageVector;

        /// <summary>
        /// その他のデータを保持
        /// </summary>
        protected object Tag;
        #endregion

        /// <summary>
        /// 与えられたデータからインスタンスを作成します
        /// </summary>
        /// <param name="EigenSystem">固有値・固有ベクトル</param>
        /// <param name="CoefficientMatrix">展開係数</param>
        /// <param name="AverageVector">平均ベクトル（Vector型から変更したので注意してね！！）</param>
        /// <param name="Tag">その他データ</param>
        public PCAData(EigenSystem EigenSystem, Matrix CoefficientMatrix, ColumnVector AverageVector, object Tag)
        {
            this.EigenSystemData = new EigenSystem(EigenSystem);
            this.CoefficientMatrix = new Matrix(CoefficientMatrix);
            this.AverageVector = new ColumnVector(AverageVector);
            this.Tag = (object)Tag;
        }


        /// <summary>
        /// このクラスをセーブするメソッド
        /// </summary>
        /// <param name="strSaveFileName">セーブファイル名</param>
        /// <returns>成功ならTrue</returns>
        public virtual bool DataSave(string strSaveFileName)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(strSaveFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, this);
            }
            return true;
        }

        /// <summary>
        /// データをロードするメソッド
        /// </summary>
        /// <param name="strLoadFileName">ロードファイル名</param>
        /// <returns>ロードしたデータ</returns>
        public static PCAData DataLoad(string strLoadFileName)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(strLoadFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (PCAData)bf.Deserialize(fs);
            }
        }

        #region プロパティ

        /// <summary>
        /// 固有値・固有ベクトルを取得します
        /// </summary>
        public EigenSystem EigenSystem
        {
            get { return new EigenSystem(this.EigenSystemData); }
        }

        /// <summary>
        /// 平均ベクトルを取得します
        /// </summary>
        public ColumnVector Average
        {
            get { return new ColumnVector(this.AverageVector); }
        }

        /// <summary>
        /// 展開係数を取得します
        /// </summary>
        public Matrix Coefficient
        {
            get { return new Matrix(this.CoefficientMatrix); }
        }

        /// <summary>
        /// データの個数を取得します
        /// </summary>
        public int DataCount
        {
            get { return this.CoefficientMatrix.ColSize; }
        }

        /// <summary>
        /// パラメータの個数を取得します
        /// </summary>
        public int ParamCount
        {
            get { return this.CoefficientMatrix.RowSize; }
        }

        /// <summary>
        /// データ固有の値を取得します
        /// </summary>
        public object DataTag
        {
            get { return this.Tag; }
            set { Tag = value; }
        }
        #endregion
    }
}
