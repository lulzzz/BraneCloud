/*
 * BraneCloud.Evolution.EC (Evolutionary Computation)
 * Copyright 2011 Bennett R. Stabile (BraneCloud.Evolution.net|com)
 * Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0.html)
 *
 * This is an independent conversion from Java to .NET of ...
 *
 * Sean Luke's ECJ project at GMU 
 * (Academic Free License v3.0): 
 * http://www.cs.gmu.edu/~eclab/projects/ecj
 *
 * Radical alteration was required throughout (including structural).
 * The author of ECJ cannot and MUST not be expected to support this fork.
 *
 * If you wish to create yet another fork, please use a different root namespace.
 * BraneCloud is a registered domain that will be used for name/schema resolution.
 */

using System;
using BraneCloud.Evolution.EC.Configuration;
using BraneCloud.Evolution.EC.GP;

namespace BraneCloud.Evolution.EC.Problems.Multiplexer.Func
{
    [ECConfiguration("ec.problems.multiplexer.func.And")]
    public class And : GPNode
    {
        public override string ToString() { return "and"; }

        //public override void CheckConstraints(IEvolutionState state,
        //    int tree,
        //    GPIndividual typicalIndividual,
        //    IParameter individualBase)
        //{
        //    base.CheckConstraints(state, tree, typicalIndividual, individualBase);
        //    if (Children.Length != 2)
        //        state.Output.Error("Incorrect number of children for node " +
        //            ToStringForError() + " at " +
        //            individualBase);
        //}

        public override int ExpectedChildren => 2;

        public override void Eval(IEvolutionState state,
            int thread,
            GPData input,
            ADFStack stack,
            GPIndividual individual,
            IProblem problem)
        {
            var md = (MultiplexerData)input;
            long[] dat_11 = null;  // quiets compiler complaints
            long dat_6 = 0L;
            byte dat_3 = 0;

            // No shortcuts for now
            Children[0].Eval(state, thread, input, stack, individual, problem);

            if (md.Status == MultiplexerData.STATUS_3)
                dat_3 = md.Dat3;
            else if (md.Status == MultiplexerData.STATUS_6)
                dat_6 = md.Dat6;
            else // md.status == MultiplexerData.STATUS_11
            {
                dat_11 = md.PopDat11();
                Array.Copy(md.Dat11, 0,
                    dat_11, 0,
                    MultiplexerData.MULTI_11_NUM_BITSTRINGS);
            }

            Children[1].Eval(state, thread, input, stack, individual, problem);

            // modify

            if (md.Status == MultiplexerData.STATUS_3)
                md.Dat3 &= dat_3;
            else if (md.Status == MultiplexerData.STATUS_6)
                md.Dat6 &= dat_6;
            else // md.status == MultiplexerData.STATUS_11
            {
                for (int x = 0; x < MultiplexerData.MULTI_11_NUM_BITSTRINGS; x++)
                    md.Dat11[x] &= dat_11[x];
                md.PushDat11(dat_11);
            }
        }
    }
}