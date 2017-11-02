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

namespace BraneCloud.Evolution.EC.Problems.Edge.Func
{
    [ECConfiguration("ec.problems.edge.func.Split")]
    public class Split : GPNode
    {
        public override string ToString() { return "split"; }

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
            var edge = ((EdgeData)(input)).edge;
            var prob = (Edge)problem;

            if (prob.From.Length == prob.NumEdges)  // we're full, need to expand
            {
                var from_ = new int[prob.NumEdges * 2];
                var to_ = new int[prob.NumEdges * 2];
                var reading_ = new int[prob.NumEdges * 2];
                Array.Copy(prob.From, 0, from_, 0, prob.From.Length);
                Array.Copy(prob.To, 0, to_, 0, prob.To.Length);
                Array.Copy(prob.Reading, 0, reading_, 0, prob.Reading.Length);
                prob.From = from_;
                prob.To = to_;
                prob.Reading = reading_;
            }

            if (prob.Start.Length == prob.NumNodes)  // we're full, need to expand
            {
                var start_ = new bool[prob.NumNodes * 2];
                var accept_ = new bool[prob.NumNodes * 2];
                Array.Copy(prob.Start, 0, start_, 0, prob.Start.Length);
                Array.Copy(prob.Accept, 0, accept_, 0, prob.Accept.Length);
                prob.Start = start_;
                prob.Accept = accept_;
            }

            var newedge = prob.NumEdges;
            prob.NumEdges++;
            var newnode = prob.NumNodes;
            prob.NumNodes++;

            // set up new node
            prob.Accept[newnode] = false;
            prob.Start[newnode] = false;

            // set up new edge
            prob.From[newedge] = newnode;
            prob.To[newedge] = prob.To[edge];
            prob.Reading[newedge] = prob.Reading[edge];
            // modify old edge
            prob.To[edge] = newnode;

            // pass the original edge down the left child

            Children[0].Eval(state, thread, input, stack, individual, problem);

            // reset input for right child
            ((EdgeData)(input)).edge = newedge;

            // pass the new edge down the right child

            Children[1].Eval(state, thread, input, stack, individual, problem);
        }
    }
}