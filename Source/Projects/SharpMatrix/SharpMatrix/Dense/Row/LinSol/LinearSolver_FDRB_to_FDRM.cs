using SharpMatrix.Data;
using SharpMatrix.Dense.Block;
using SharpMatrix.Dense.Block.LinSol.Chol;
using SharpMatrix.Interfaces.Decomposition;
using SharpMatrix.Interfaces.LinSol;

namespace SharpMatrix.Dense.Row.LinSol
{
    //package org.ejml.dense.row.linsol;

/**
 * Wrapper that allows {@link FMatrixRBlock} to implements {@link LinearSolverDense}.  It works
 * by converting {@link FMatrixRMaj} into {@link FMatrixRBlock} and calling the equivalent
 * functions.  Since a local copy is made all input matrices are never modified.
 *
 * @author Peter Abeles
 */
    public class LinearSolver_FDRB_to_FDRM : LinearSolverDense<FMatrixRMaj>
    {
        protected LinearSolverDense<FMatrixRBlock> alg;

        // block matrix copy of the system A matrix.
        protected FMatrixRBlock blockA = new FMatrixRBlock(1, 1);

        // block matrix copy of B matrix passed into solve
        protected FMatrixRBlock blockB = new FMatrixRBlock(1, 1);

        // block matrix copy of X matrix passed into solve
        protected FMatrixRBlock blockX = new FMatrixRBlock(1, 1);

        public LinearSolver_FDRB_to_FDRM(LinearSolverDense<FMatrixRBlock> alg)
        {
            this.alg = alg;
        }

        /**
         * Converts 'A' into a block matrix and call setA() on the block matrix solver.
         *
         * @param A The A matrix in the linear equation. Not modified. Reference saved.
         * @return true if it can solve the system.
         */
        public virtual bool setA(FMatrixRMaj A)
        {
            blockA.reshape(A.numRows, A.numCols, false);
            MatrixOps_FDRB.convert(A, blockA);

            return alg.setA(blockA);
        }

        public virtual /**/ double quality()
        {
            return alg.quality();
        }

        /**
         * Converts B and X into block matrices and calls the block matrix solve routine.
         *
         * @param B A matrix &real; <sup>m &times; p</sup>.  Not modified.
         * @param X A matrix &real; <sup>n &times; p</sup>, where the solution is written to.  Modified.
         */
        public virtual void solve(FMatrixRMaj B, FMatrixRMaj X)
        {
            blockB.reshape(B.numRows, B.numCols, false);
            blockX.reshape(X.numRows, X.numCols, false);
            MatrixOps_FDRB.convert(B, blockB);

            alg.solve(blockB, blockX);

            MatrixOps_FDRB.convert(blockX, X);
        }

        /**
         * Creates a block matrix the same size as A_inv, inverts the matrix and copies the results back
         * onto A_inv.
         * 
         * @param A_inv Where the inverted matrix saved. Modified.
         */
        public virtual void invert(FMatrixRMaj A_inv)
        {
            blockB.reshape(A_inv.numRows, A_inv.numCols, false);

            alg.invert(blockB);

            MatrixOps_FDRB.convert(blockB, A_inv);
        }

        public virtual bool modifiesA()
        {
            return false;
        }

        public virtual bool modifiesB()
        {
            return false;
        }

        public virtual DecompositionInterface<FMatrixRMaj> getDecomposition()
        {
            return (DecompositionInterface<FMatrixRMaj>) alg.getDecomposition();
        }
    }
}