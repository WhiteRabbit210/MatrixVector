using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MatrixVector
{
    public static class PCA
    {
        public static PCAData GetPCAData(Matrix LoadMatrix,  object Tag = null)
        {
            ColumnVector AverageVector = LoadMatrix.GetAverageRow();
            Matrix AverageMatrix = Matrix.GetSameElementMatrix(AverageVector, LoadMatrix.ColSize);
            Matrix DiffMatrix = LoadMatrix - AverageMatrix;
            SymmetricMatrix LMatrix = new SymmetricMatrix(DiffMatrix.GetTranspose() * DiffMatrix);
            EigenSystem EigenSystemData = LMatrix.GetEigenVectorAndValue(0.00001);
            Matrix LEigenVector = EigenSystemData.GetEigenVectors();
            Matrix FinalEigenVector = (LoadMatrix * LEigenVector).GetNormalizedMatrixCol();
            EigenSystem FinalEigenSystem = new EigenSystem();
            for (int i = 0; i < EigenSystemData.Count; i++)
            {
                if (EigenSystemData[i].EigenValue > 0.0001)
                    FinalEigenSystem.Add(new EigenVectorAndValue(FinalEigenVector.GetColVector(i), EigenSystemData[i].EigenValue));
            }
            Matrix CoefficientMatrix = FinalEigenSystem.GetEigenVectors().GetTranspose() * DiffMatrix;

            return new PCAData( FinalEigenSystem, CoefficientMatrix, AverageVector, Tag);
        }
    }
}
