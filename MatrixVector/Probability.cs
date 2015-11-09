using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace MatrixVector
{
    public class Probability
    {
        /// <summary>
        ///  Fisherの正確確率検定(片側検定)
        /// </summary>
        /// <param name="Matrix">データ表</param>
        /// <returns>p値</returns>
        public static double FishersExactOneSideTest(Matrix Matrix)
        {
            return Function.FishersExact(Matrix, false);
        }

        /// <summary>
        /// Fisherの正確確率検定(両側検定)
        /// </summary>
        /// <param name="Matrix">データ表</param>
        /// <returns>p値</returns>
        public static double FishersExactTwoSideTest(Matrix Matrix)
        {
            return Function.FishersExact(Matrix, true);
        }

        /// <summary>
        /// χ二乗検定
        /// </summary>
        /// <param name="Matrix">データ表</param>
        /// <returns>p値</returns>
        public static double ChiSqTest(Matrix Matrix)
        {
            ColumnVector DiffVector = Matrix.GetColVector(0) - Matrix.GetColVector(1);
            DiffVector = ColumnVector.Multiply(DiffVector, DiffVector);

            for (int i = 0; i < DiffVector.Length; i++)
                DiffVector[i] /= Matrix.GetColVector(1)[i];

            return 1d - Function.PChisq(DiffVector.GetSum(), DiffVector.Length - 1);
            //return Function.QChisq(DiffVector.GetSum(), DiffVector.Length - 1);
        }

    }
}
