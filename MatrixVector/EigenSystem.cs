using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatrixVector
{
    /// <summary>
    /// 固有値と固有ベクトルのリストを保持するクラス
    /// </summary>
    [Serializable]
    public class EigenSystem
    {
        /// <summary>
        /// 固有値と固有ベクトルのリスト
        /// </summary>
        protected List<EigenVectorAndValue> EigenListData = new List<EigenVectorAndValue>();

        #region コンストラクタ
        /// <summary>
        /// 固有値と固有ベクトルのリストを保持するクラスを作成します。
        /// </summary>
        public EigenSystem() { }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="EigenSystemData">コピー元</param>
        public EigenSystem(EigenSystem EigenSystemData)
        {
            foreach (EigenVectorAndValue EigenData in EigenSystemData.EigenListData)
                EigenListData.Add(EigenData);
        }
        #endregion

        /// <summary>
        /// 固有値、固有ベクトルのリストにデータを加えます
        /// 追加したデータは固有値が大きい順にソートされます。
        /// </summary>
        /// <param name="EigenData">固有値、固有ベクトルデータ</param>
        public void Add(EigenVectorAndValue EigenData)
        {
            EigenListData.Add(EigenData);
            EigenListData = EigenListData.OrderByDescending(n => n.EigenValue).ToList();
        }

        /// <summary>
        /// 固有値、固有ベクトルのデータを取得、設定します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <returns>固有値、固有ベクトルデータ</returns>
        public EigenVectorAndValue this[int index]
        {
            get { return new EigenVectorAndValue(EigenListData[index]); }
            set { EigenListData[index]= value; }
        }

        /// <summary>
        /// 固有値、固有ベクトルのリストからデータを削除します
        /// </summary>
        /// <param name="index">削除するデータのインデックス</param>
        public void RemoveAt(int index)
        {
            EigenListData.RemoveAt(index);
            EigenListData.Sort();
        }


        /// <summary>
        /// 全ての固有ベクトルを行列形式で取得します。
        /// </summary>
        /// <returns>全ての固有ベクトル</returns>
        public Matrix GetEigenVectors()
        {
            Vector[] ReturnVector = new Vector[EigenListData.Count];
            for (int i = 0; i < EigenListData.Count; i++)
                ReturnVector[i] = EigenListData[i].EigenVector;

            return new Matrix(ReturnVector);
        }

        public ColumnVector GetEigenValues()
        {
            ColumnVector ReturnVector = new ColumnVector(EigenListData.Count);
            for (int i = 0; i < EigenListData.Count; i++)
                ReturnVector[i] = EigenListData[i].EigenValue;

            return ReturnVector;
        }

        /// <summary>
        /// 累積寄与率を求めます。
        /// </summary>
        /// <returns>累積寄与率が順に入っているColumnVector</returns>
        public ColumnVector GetCumulativeContribution()
        {
            ColumnVector EigenValues = GetEigenValues();
            EigenValues = EigenValues / EigenValues.GetSum();
            for (int i = 1; i < EigenValues.Length; i++)
                EigenValues[i] = EigenValues[i - 1] + EigenValues[i];

            return EigenValues;
        }

        /// <summary>
        /// 固有値、固有ベクトルのリストの大きさを取得します。
        /// </summary>
        public int Count
        {
            get { return EigenListData.Count; }
        }
    }
}
