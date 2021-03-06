﻿/*
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
using System.Reflection;
using BraneCloud.Evolution.EC.Configuration;
using BraneCloud.Evolution.EC.Runtime;

namespace BraneCloud.Evolution.EC.App.BBob
{
    class Program
    {
        static void Main(string[] args)
        {
            // NOTE: This requires some attention, since it is a fairly complex benchmark suite, and I'm not sure if it working properly yet.
            //       One thing I changed was setting each individual to evalutated in BBOBenchmarks.Evalutate(...)
            //       It was not previously doing this (intentional?).

            // This primes the activator so it knows where to look for types that will be created from parameters.
            ECActivator.AddSourceAssemblies(new[]
                                                {
                                                    Assembly.GetAssembly(typeof(IEvolutionState)),
                                                    Assembly.GetAssembly(typeof(BBOBenchmarks))
                                                });

            // Here we are starting up with bbob.params
            // But this can also be started with de.params (which does differential evolution)
            Evolve.Run(new[] { "-file", @"Params\App\BBob\bbob.params" });
            Console.WriteLine("\nDone!");
            Console.ReadLine();
        }
    }
}