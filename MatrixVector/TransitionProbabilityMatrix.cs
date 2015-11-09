using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MatrixVector;

namespace MatrixVector
{
    public class TransitionProbabilityMatrix : SquareMatrix
    {
        public TransitionProbabilityMatrix(SquareMatrix Matrix)
        {
            this.Vectors = (ColumnVector[])Matrix.ColumnVector.Clone();
        }

        public TransitionProbabilityMatrix(int ElemntSize)
            : base(ElemntSize)
        { }

        public void AddTransitionVector(Vector TransitionVector)
        {
            for (int i = 1; i < TransitionVector.Length; i++)
                this[(int)TransitionVector[i - 1], (int)TransitionVector[i]]++;
        }

        public Matrix GetTransitionProbabilityMatrix()
        {
            Matrix ResultMatrix = new Matrix(this.RowSize, this.ColSize);

            ColumnVector SumVector = new ColumnVector(this.RowSize);
            for (int i = 0; i < SumVector.Length; i++)
                SumVector[i] = this.GetRowVector(i).GetSum();

            for (int i = 0; i < ResultMatrix.RowSize; i++)
                if (SumVector[i] != 0) ResultMatrix = ResultMatrix.SetRowVectorAt(i, this.GetRowVector(i) / SumVector[i]);

            return ResultMatrix;
        }
    }
}
