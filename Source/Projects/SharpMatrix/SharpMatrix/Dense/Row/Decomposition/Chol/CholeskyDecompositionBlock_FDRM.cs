using System;
using SharpMatrix.Data;

namespace SharpMatrix.Dense.Row.Decomposition.Chol
{
    //package org.ejml.dense.row.decomposition.chol;

/**
 * This is an implementation of Cholesky that processes internal submatrices as blocks.  This is
 * done to reduce the number of cache issues.
 *
 * @author Peter Abeles
 */
    public class CholeskyDecompositionBlock_FDRM : CholeskyDecompositionCommon_FDRM
    {

        private int blockWidth; // how wide the blocks should be
        private FMatrixRMaj B; // row rectangular matrix

        private CholeskyBlockHelper_FDRM chol;

        /**
         * Creates a CholeksyDecomposition capable of decomposing a matrix that is
         * n by n, where n is the width.
         *
         * @param blockWidth The width of a block.
         */
        public CholeskyDecompositionBlock_FDRM(int blockWidth)
            : base(true)
        {

            this.blockWidth = blockWidth;

        }

        /**
         * Declares additional internal data structures.
         */
        public override void setExpectedMaxSize(int numRows, int numCols)
        {
            base.setExpectedMaxSize(numRows, numCols);

            // if the matrix that is being decomposed is smaller than the block we really don't
            // see the B matrix.
            if (numRows < blockWidth)
                B = new FMatrixRMaj(0, 0);
            else
                B = new FMatrixRMaj(blockWidth, maxWidth);

            chol = new CholeskyBlockHelper_FDRM(blockWidth);
        }

        /**
         * <p>
         * Performs Choleksy decomposition on the provided matrix.
         * </p>
         *
         * <p>
         * If the matrix is not positive definite then this function will return
         * false since it can't complete its computations.  Not all errors will be
         * found.
         * </p>
         * @return True if it was able to finish the decomposition.
         */
        protected override bool decomposeLower()
        {

            if (n < blockWidth)
                B.reshape(0, 0, false);
            else
                B.reshape(blockWidth, n - blockWidth, false);

            int numBlocks = n / blockWidth;
            int remainder = n % blockWidth;

            if (remainder > 0)
            {
                numBlocks++;
            }

            B.numCols = n;

            for (int i = 0; i < numBlocks; i++)
            {
                B.numCols -= blockWidth;

                if (B.numCols > 0)
                {
                    // apply cholesky to the current block
                    if (!chol.decompose(T, (i * blockWidth) * T.numCols + i * blockWidth, blockWidth)) return false;

                    int indexSrc = i * blockWidth * T.numCols + (i + 1) * blockWidth;
                    int indexDst = (i + 1) * blockWidth * T.numCols + i * blockWidth;

                    // B = L^(-1) * B
                    solveL_special(chol.getL().data, T, indexSrc, indexDst, B);

                    int indexL = (i + 1) * blockWidth * n + (i + 1) * blockWidth;

                    // c = c - a^T*a
                    symmRankTranA_sub(B, T, indexL);
                }
                else
                {
                    int width = remainder > 0 ? remainder : blockWidth;
                    if (!chol.decompose(T, (i * blockWidth) * T.numCols + i * blockWidth, width)) return false;
                }
            }


            // zero the top right corner.
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    t[i * n + j] = 0.0f;
                }
            }

            return true;
        }

        protected override bool decomposeUpper()
        {
            throw new InvalidOperationException("Not implemented.  Do a lower decomposition and transpose it...");
        }

        /**
         * This is a variation on the {@link TriangularSolver_FDRM#solveL} function.
         * It grabs the input from the top right row rectangle of the source matrix then writes the results
         * to the lower bottom column rectangle.  The rectangle matrices just matrices are submatrices
         * of the matrix that is being decomposed.  The results are also written to B.
         *
         * @param L A lower triangular matrix.
         * @param b_src matrix with the vectors that are to be solved for
         * @param indexSrc First index of the submatrix where the inputs are coming from.
         * @param indexDst First index of the submatrix where the results are going to.
         * @param B
         */
        public static void solveL_special(float[] L,
            FMatrixRMaj b_src,
            int indexSrc, int indexDst,
            FMatrixRMaj B)
        {
            float[] dataSrc = b_src.data;

            float[] b = B.data;
            int m = B.numRows;
            int n = B.numCols;
            int widthL = m;

//        for( int j = 0; j < n; j++ ) {
//            for( int i = 0; i < widthL; i++ ) {
//                float sum = dataSrc[indexSrc+i*b_src.numCols+j];
//                for( int k=0; k<i; k++ ) {
//                    sum -= L[i*widthL+k]* b[k*n+j];
//                }
//                float val = sum / L[i*widthL+i];
//                dataSrc[indexDst+j*b_src.numCols+i] = val;
//                b[i*n+j] = val;
//            }
//        }

            for (int j = 0; j < n; j++)
            {
                int indexb = j;
                int rowL = 0;

                //for( int i = 0; i < widthL; i++
                for (int i = 0; i < widthL; i++, indexb += n, rowL += widthL)
                {
                    float sum = dataSrc[indexSrc + i * b_src.numCols + j];

                    int indexL = rowL;
                    int endL = indexL + i;
                    int indexB = j;
                    //for( int k=0; k<i; k++ ) {
                    for (; indexL != endL; indexB += n)
                    {
                        sum -= L[indexL++] * b[indexB];
                    }
                    float val = sum / L[i * widthL + i];
                    dataSrc[indexDst + j * b_src.numCols + i] = val;
                    b[indexb] = val;
                }
            }
        }

        /**
         * <p>
         * Performs this operation:<br>
         * <br>
         * c = c - a<sup>T</sup>a <br>
         * where c is a submatrix.
         * </p>
         *
         * Only the upper triangle is updated.
         *
         * @param a A matrix.
         * @param c A matrix.
         * @param startIndexC start of the submatrix in c.
         */
        public static void symmRankTranA_sub(FMatrixRMaj a, FMatrixRMaj c,
            int startIndexC)
        {
            // TODO update so that it doesn't modify/read parts that it shouldn't
            float[] dataA = a.data;
            float[] dataC = c.data;

//        for( int i = 0; i < a.numCols; i++ ) {
//            for( int k = 0; k < a.numRows; k++ ) {
//                float valA = dataA[k*a.numCols+i];
//
//                for( int j = i; j < a.numCols; j++ ) {
//                    dataC[startIndexC+i*c.numCols+j] -= valA * dataA[k*a.numCols+j];
//                }
//            }
//        }

            int strideC = c.numCols + 1;
            for (int i = 0; i < a.numCols; i++)
            {
                int indexA = i;
                int endR = a.numCols;

                for (int k = 0; k < a.numRows; k++, indexA += a.numCols, endR += a.numCols)
                {
                    int indexC = startIndexC;
                    float valA = dataA[indexA];
                    int indexR = indexA;

                    while (indexR < endR)
                    {
                        dataC[indexC++] -= valA * dataA[indexR++];
                    }
                }
                startIndexC += strideC;
            }

        }
    }
}