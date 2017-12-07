using System;
using BraneCloud.Evolution.EC.MatrixLib.Data;
using BraneCloud.Evolution.EC.MatrixLib.Interfaces.Decomposition;

namespace BraneCloud.Evolution.EC.MatrixLib.Dense.Row.Decomposition.QR
{
    //package org.ejml.dense.row.decomposition.qr;

/**
 * <p>
 * Householder QR decomposition is rich in operations along the columns of the matrix.  This can be
 * taken advantage of by solving for the Q matrix in a column major format to reduce the number
 * of CPU cache misses and the number of copies that are performed.
 * </p>
 *
 * @see QRDecompositionHouseholder_DDRM
 *
 * @author Peter Abeles
 */
    public class QRDecompositionHouseholderColumn_DDRM : QRDecomposition<DMatrixRMaj>
    {

        /**
         * Where the Q and R matrices are stored.  R is stored in the
         * upper triangular portion and Q on the lower bit.  Lower columns
         * are where u is stored.  Q_k = (I - gamma_k*u_k*u_k^T).
         */
        protected double[][] dataQR; // [ column][ row ]

        // used internally to store temporary data
        protected double[] v;

        // dimension of the decomposed matrices
        protected int numCols; // this is 'n'

        protected int numRows; // this is 'm'
        protected int minLength;

        // the computed gamma for Q_k matrix
        protected double[] gammas;

        // local variables
        protected double gamma;

        protected double tau;

        // did it encounter an error?
        protected bool error;

        public virtual void setExpectedMaxSize(int numRows, int numCols)
        {
            this.numCols = numCols;
            this.numRows = numRows;
            minLength = Math.Min(numCols, numRows);
            int maxLength = Math.Max(numCols, numRows);

            if (dataQR == null || dataQR.Length < numCols || dataQR[0].Length < numRows)
            {
                //dataQR = new double[ numCols ][  numRows ];
                dataQR = TensorFactory.Create<double>(numCols, numRows);
                v = new double[maxLength];
                gammas = new double[minLength];
            }

            if (v.Length < maxLength)
            {
                v = new double[maxLength];
            }
            if (gammas.Length < minLength)
            {
                gammas = new double[minLength];
            }
        }

        /**
         * Returns the combined QR matrix in a 2D array format that is column major.
         *
         * @return The QR matrix in a 2D matrix column major format. [ column ][ row ]
         */
        public double[][] getQR()
        {
            return dataQR;
        }

        /**
         * Computes the Q matrix from the imformation stored in the QR matrix.  This
         * operation requires about 4(m<sup>2</sup>n-mn<sup>2</sup>+n<sup>3</sup>/3) flops.
         *
         * @param Q The orthogonal Q matrix.
         */
        public virtual DMatrixRMaj getQ(DMatrixRMaj Q, bool compact)
        {
            if (compact)
            {
                Q = UtilDecompositons_DDRM.checkIdentity(Q, numRows, minLength);
            }
            else
            {
                Q = UtilDecompositons_DDRM.checkIdentity(Q, numRows, numRows);
            }

            for (int j = minLength - 1; j >= 0; j--)
            {
                double[] u = dataQR[j];

                double vv = u[j];
                u[j] = 1;
                QrHelperFunctions_DDRM.rank1UpdateMultR(Q, u, gammas[j], j, j, numRows, v);
                u[j] = vv;
            }

            return Q;
        }

        /**
         * Returns an upper triangular matrix which is the R in the QR decomposition.  If compact then the input
         * expected to be size = [min(rows,cols) , numCols] otherwise size = [numRows,numCols].
         *
         * @param R Storage for upper triangular matrix.
         * @param compact If true then a compact matrix is expected.
         */
        public virtual DMatrixRMaj getR(DMatrixRMaj R, bool compact)
        {
            if (compact)
            {
                R = UtilDecompositons_DDRM.checkZerosLT(R, minLength, numCols);
            }
            else
            {
                R = UtilDecompositons_DDRM.checkZerosLT(R, numRows, numCols);
            }

            for (int j = 0; j < numCols; j++)
            {
                double[] colR = dataQR[j];
                int l = Math.Min(j, numRows - 1);
                for (int i = 0; i <= l; i++)
                {
                    double val = colR[i];
                    R.set(i, j, val);
                }
            }

            return R;
        }

        /**
         * <p>
         * To decompose the matrix 'A' it must have full rank.  'A' is a 'm' by 'n' matrix.
         * It requires about 2n*m<sup>2</sup>-2m<sup>2</sup>/3 flops.
         * </p>
         *
         * <p>
         * The matrix provided here can be of different
         * dimension than the one specified in the constructor.  It just has to be smaller than or equal
         * to it.
         * </p>
         */
        public virtual bool decompose(DMatrixRMaj A)
        {
            setExpectedMaxSize(A.numRows, A.numCols);

            convertToColumnMajor(A);

            error = false;

            for (int j = 0; j < minLength; j++)
            {
                householder(j);
                updateA(j);
            }

            return !error;
        }

        public virtual bool inputModified()
        {
            return false;
        }

        /**
         * Converts the standard row-major matrix into a column-major vector
         * that is advantageous for this problem.
         *
         * @param A original matrix that is to be decomposed.
         */
        protected void convertToColumnMajor(DMatrixRMaj A)
        {
            for (int x = 0; x < numCols; x++)
            {
                double[] colQ = dataQR[x];
                for (int y = 0; y < numRows; y++)
                {
                    colQ[y] = A.data[y * numCols + x];
                }
            }
        }

        /**
         * <p>
         * Computes the householder vector "u" for the first column of submatrix j.  Note this is
         * a specialized householder for this problem.  There is some protection against
         * overfloaw and underflow.
         * </p>
         * <p>
         * Q = I - &gamma;uu<sup>T</sup>
         * </p>
         * <p>
         * This function finds the values of 'u' and '&gamma;'.
         * </p>
         *
         * @param j Which submatrix to work off of.
         */
        protected void householder(int j)
        {
            double[] u = dataQR[j];

            // find the largest value in this column
            // this is used to normalize the column and mitigate overflow/underflow
            double max = QrHelperFunctions_DDRM.findMax(u, j, numRows - j);

            if (max == 0.0)
            {
                gamma = 0;
                error = true;
            }
            else
            {
                // computes tau and normalizes u by max
                tau = QrHelperFunctions_DDRM.computeTauAndDivide(j, numRows, u, max);

                // divide u by u_0
                double u_0 = u[j] + tau;
                QrHelperFunctions_DDRM.divideElements(j + 1, numRows, u, u_0);

                gamma = u_0 / tau;
                tau *= max;

                u[j] = -tau;
            }

            gammas[j] = gamma;
        }

        /**
         * <p>
         * Takes the results from the householder computation and updates the 'A' matrix.<br>
         * <br>
         * A = (I - &gamma;*u*u<sup>T</sup>)A
         * </p>
         *
         * @param w The submatrix.
         */
        protected void updateA(int w)
        {
            double[] u = dataQR[w];

            for (int j = w + 1; j < numCols; j++)
            {

                double[] colQ = dataQR[j];
                double val = colQ[w];

                for (int k = w + 1; k < numRows; k++)
                {
                    val += u[k] * colQ[k];
                }
                val *= gamma;

                colQ[w] -= val;
                for (int i = w + 1; i < numRows; i++)
                {
                    colQ[i] -= u[i] * val;
                }
            }
        }

        public double[] getGammas()
        {
            return gammas;
        }
    }
}