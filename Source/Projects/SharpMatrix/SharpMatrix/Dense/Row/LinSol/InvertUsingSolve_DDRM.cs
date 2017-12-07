using System;
using BraneCloud.Evolution.EC.MatrixLib.Data;
using BraneCloud.Evolution.EC.MatrixLib.Interfaces.LinSol;

namespace BraneCloud.Evolution.EC.MatrixLib.Dense.Row.LinSol
{
    //package org.ejml.dense.row.linsol;

/**
 * A matrix can be easily inverted by solving a system with an identify matrix.  The only
 * disadvantage of this approach is that additional computations are required compared to
 * a specialized solution.
 *
 * @author Peter Abeles
 */
    public class InvertUsingSolve_DDRM
    {

        public static void invert(LinearSolverDense<DMatrixRMaj> solver, DMatrix1Row A, DMatrixRMaj A_inv,
            DMatrixRMaj storage)
        {

            if (A.numRows != A_inv.numRows || A.numCols != A_inv.numCols)
            {
                throw new ArgumentException("A and A_inv must have the same dimensions");
            }

            CommonOps_DDRM.setIdentity(storage);

            solver.solve(storage, A_inv);
        }

        public static void invert(LinearSolverDense<DMatrixRMaj> solver, DMatrix1Row A, DMatrixRMaj A_inv)
        {

            if (A.numRows != A_inv.numRows || A.numCols != A_inv.numCols)
            {
                throw new ArgumentException("A and A_inv must have the same dimensions");
            }

            CommonOps_DDRM.setIdentity(A_inv);

            solver.solve(A_inv, A_inv);
        }
    }
}