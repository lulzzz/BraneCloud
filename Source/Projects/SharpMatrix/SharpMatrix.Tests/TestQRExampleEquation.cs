using System;
using SharpMatrix.Data;
using SharpMatrix.Dense.Row;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randomization;

namespace SharpMatrix.Examples
{
    //package org.ejml.example;

/**
 * @author Peter Abeles
 */
    [TestClass]
    public class TestQRExampleEquation
    {

        IMersenneTwister rand = new MersenneTwisterFast(23423);

        //@Test
        [TestMethod]
        public void basic()
        {
            checkMatrix(7, 5);
            checkMatrix(5, 5);
            checkMatrix(7, 7);
        }

        private void checkMatrix(int numRows, int numCols)
        {
            DMatrixRMaj A = RandomMatrices_DDRM.rectangle(numRows, numCols, -1, 1, rand);

            QRExampleEquation alg = new QRExampleEquation();

            alg.decompose(A);

            DMatrixRMaj Q = alg.getQ();
            DMatrixRMaj R = alg.getR();

            DMatrixRMaj A_found = new DMatrixRMaj(numRows, numCols);
            CommonOps_DDRM.mult(Q, R, A_found);

            Assert.IsTrue(MatrixFeatures_DDRM.isIdentical(A, A_found, UtilEjml.TEST_F64));
        }

    }
}