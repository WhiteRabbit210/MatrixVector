using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace MatrixVector
{
    public class Function
    {
        static double v, w;
        static double PI = Math.PI;
        static double LOG_2PI = Math.Log(2 * PI);
        static double LOG_PI = Math.Log(Math.PI);
        static double N = 8;
        static double B0 = 1;
        static double B1 = (-1.0 / 2.0);
        static double B2 = (1.0 / 6.0);
        static double B4 = (-1.0 / 30.0);
        static double B6 = (1.0 / 42.0);
        static double B8 = (-1.0 / 30.0);
        static double B10 = (5.0 / 66.0);
        static double B12 = (-691.0 / 2730.0);
        static double B14 = (7.0 / 6.0);
        static double B16 = (-3617.0 / 510.0);

        internal static double GetFishersExactProbability(int a, int b, int c, int d)
        {
            BigInteger BIab = 1;
            BigInteger BIcd = 1;
            BigInteger BIac = 1;
            BigInteger BIbd = 1;
            BigInteger BInabcd = 1;

            for (int i = 2; i <= a + b; i++)
                BIab *= i;

            for (int i = 2; i <= c + d; i++)
                BIcd *= i;

            for (int i = 2; i <= a + c; i++)
                BIac *= i;

            for (int i = 2; i <= b + d; i++)
                BIbd *= i;

            {
                BigInteger BIa = 1;
                BigInteger BIb = 1;
                BigInteger BIc = 1;
                BigInteger BId = 1;
                BigInteger BIn = 1;

                for (int i = 2; i <= (a + b + c + d); i++)
                    BIn *= i;

                for (int i = 2; i <= a; i++)
                    BIa *= i;

                for (int i = 2; i <= b; i++)
                    BIb *= i;

                for (int i = 2; i <= c; i++)
                    BIc *= i;

                for (int i = 2; i <= d; i++)
                    BId *= i;

                BInabcd = BIn * BIa * BIb * BIc * BId;
            }

            BigInteger Prob = (BIab * BIcd * BIac * BIbd) * new BigInteger(1000000000000000000000000000d) / BInabcd;

            return double.Parse(Prob.ToString()) / 1000000000000000000000000000d;
        }


        /// <summary>
        /// Fisherの正確確率検定
        /// </summary>
        /// <param name="Matrix">データ表</param>
        /// <param name="test">両側検定か片側検定か</param>
        /// <returns>p値</returns>
        internal static double FishersExact(Matrix Matrix, bool test)
        {
            int sum = 0;
            if ((int)Matrix.GetRowVector(0).GetSum() > (int)Matrix.GetColVector(0).GetSum())
                sum = (int)Matrix.GetColVector(0).GetSum() + 1;
            else
                sum = (int)Matrix.GetRowVector(0).GetSum() + 1;

            ColumnVector AVector = new ColumnVector(sum);
            ColumnVector BVector = new ColumnVector(sum);
            ColumnVector CVector = new ColumnVector(sum);
            ColumnVector DVector = new ColumnVector(sum);
            ColumnVector ADBC = new ColumnVector(sum);
            ColumnVector OccurrenceProbabilityVector = new ColumnVector(sum);
            int DataNumber = (int)Matrix[0, 0];

            for (int i = 0; i < AVector.Length; i++)
            {
                AVector[i] = i;
                BVector[i] = Matrix.GetRowVector(0).GetSum() - AVector[i];
                CVector[i] = Matrix.GetColVector(0).GetSum() - AVector[i];
                DVector[i] = Matrix.GetColVector(1).GetSum() - BVector[i];
                ADBC[i] = AVector[i] * DVector[i] - BVector[i] * CVector[i];
                OccurrenceProbabilityVector[i] = GetFishersExactProbability((int)AVector[i], (int)BVector[i], (int)CVector[i], (int)DVector[i]);
            }

            double P = 0;

            //両側検定(true)
            if (test)
            {
                for (int i = 0; i < AVector.Length; i++)
                    if (ADBC[DataNumber] > System.Math.Abs(ADBC[i])) P += ADBC[i];
            }
            //片側検定(false)
            else
            {
                for (int i = 0; i < AVector.Length; i++)
                    if (ADBC[DataNumber] <= ADBC[i]) P += OccurrenceProbabilityVector[i];
            }

            return P;
        }

        /// <summary>
        /// ガンマ関数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Gamma(double x)  /* ガンマ関数 */
        {
            if (x < 0)
                return PI / (Math.Sin(PI * x) * Math.Exp(LogGamma(1 - x)));
            return Math.Exp(LogGamma(x));
        }

        /// <summary>
        /// ベータ関数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Beta(double x, double y)  /* ベータ関数 */
        {
            return Math.Exp(LogGamma(x) + LogGamma(y) - LogGamma(x + y));
        }

        /// <summary>
        /// ガンマ関数の対数
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double LogGamma(double x)  /* ガンマ関数の対数 */
        {
            v = 1;
            while (x < N) { v *= x; x++; }
            w = 1 / (x * x);
            return ((((((((B16 / (16 * 15)) * w + (B14 / (14 * 13))) * w
                        + (B12 / (12 * 11))) * w + (B10 / (10 * 9))) * w
                        + (B8 / (8 * 7))) * w + (B6 / (6 * 5))) * w
                        + (B4 / (4 * 3))) * w + (B2 / (2 * 1))) / x
                        + 0.5 * LOG_2PI - Math.Log(v) - x + (x - 0.5) * Math.Log(x);
        }

        public static double PGamma(double a, double x, double loggamma_a)  /* 本文参照 */
        {
            double Result, Term, Previous;

            if (x >= 1 + a)
                return 1 - QGamma(a, x, loggamma_a);
            if (x == 0)
                return 0;
            Result = Term = Math.Exp(a * Math.Log(x) - x - loggamma_a) / a;
            for (int k = 1; k < 1000; k++)
            {
                Term *= x / (a + k);
                Previous = Result; Result += Term;
                if (Result == Previous) return Result;
            }
            return Result;
        }

        public static double QGamma(double a, double x, double loggamma_a)  /* 本文参照 */
        {
            double Result, w, temp, Previous;
            double la = 1, lb = 1 + x - a;  /* Laguerreの多項式 */

            if (x < 1 + a)
                return 1 - PGamma(a, x, loggamma_a);

            w = Math.Exp(a * Math.Log(x) - x - loggamma_a);
            Result = w / lb;

            for (int k = 2; k < 1000; k++)
            {
                temp = ((k - 1 - a) * (lb - la) + (k + x) * lb) / k;
                la = lb;
                lb = temp;
                w *= (k - 1 - a) / k;
                temp = w / (la * lb);

                Previous = Result;
                Result += temp;
                if (Result == Previous)
                    return Result;
            }
            return Result;
        }

        /// <summary>
        /// カイ2乗分布の下側確率
        /// </summary>
        /// <param name="Chisq">χ二乗値</param>
        /// <param name="DF">自由度</param>
        /// <returns></returns>
        public static double PChisq(double Chisq, int DF)  /*  */
        {
            return PGamma(0.5 * DF, 0.5 * Chisq, LogGamma(0.5 * DF));
        }

        /// <summary>
        /// カイ2乗分布の上側確率
        /// </summary>
        /// <param name="Chisq">χ二乗値</param>
        /// <param name="DF">自由度</param>
        /// <returns></returns>
        public static double QChisq(double Chisq, int DF)  /*  */
        {
            return QGamma(0.5 * DF, 0.5 * Chisq, LogGamma(0.5 * DF));
        }

        public static double erf(double x)  /* Gaussの誤差関数 ${\rm erf}(x)$ */
        {
            if (x >= 0)
                return PGamma(0.5, x * x, LOG_PI / 2);
            else
                return -PGamma(0.5, x * x, LOG_PI / 2);
        }

        public static double erfc(double x)  /* $1 - {\rm erf}(x)$ */
        {
            if (x >= 0)
                return QGamma(0.5, x * x, LOG_PI / 2);
            else
                return 1 + PGamma(0.5, x * x, LOG_PI / 2);
        }

        public static double PNormal(double x)  /* 標準正規分布の下側確率 */
        {
            if (x >= 0)
                return 0.5 * (1 + PGamma(0.5, 0.5 * x * x, LOG_PI / 2));
            else
                return 0.5 * QGamma(0.5, 0.5 * x * x, LOG_PI / 2);
        }

        

        public static double QNormal(double x)  /* 標準正規分布の上側確率 */
        {
            if (x >= 0)
                return 0.5 * QGamma(0.5, 0.5 * x * x, LOG_PI / 2);
            else
                return 0.5 * (1 + PGamma(0.5, 0.5 * x * x, LOG_PI / 2));
        }
    }
}
