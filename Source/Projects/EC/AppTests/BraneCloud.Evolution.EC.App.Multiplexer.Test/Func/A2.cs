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

namespace BraneCloud.Evolution.EC.App.Multiplexer.Test.Func
{
    [ECConfiguration("ec.app.multiplexer.func.A2")]
    public class A2 : GPNode
    {
        const int bitpos = 2;  /* A2 */

        public override string ToString() { return "a2"; }

        public override void CheckConstraints(IEvolutionState state,
            int tree,
            GPIndividual typicalIndividual,
            IParameter individualBase)
        {
            base.CheckConstraints(state, tree, typicalIndividual, individualBase);
            if (Children.Length != 0)
                state.Output.Error("Incorrect number of children for node " +
                    ToStringForError() + " at " +
                    individualBase);
        }

        public override void Eval(IEvolutionState state,
            int thread,
            GPData input,
            ADFStack stack,
            GPIndividual individual,
            IProblem problem)
        {
            var md = (MultiplexerData)input;

            if (md.Status == MultiplexerData.STATUS_3)
                md.Dat3 = Fast.M_3[bitpos];
            else if (md.Status == MultiplexerData.STATUS_6)
                md.Dat6 = Fast.M_6[bitpos];
            else // md.status == MultiplexerData.STATUS_11
                Array.Copy(Fast.M_11[bitpos], 0,
                    md.Dat11, 0,
                    MultiplexerData.MULTI_11_NUM_BITSTRINGS);
        }
    }
}