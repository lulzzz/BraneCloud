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
using BraneCloud.Evolution.EC.CoEvolve;
using BraneCloud.Evolution.EC.Configuration;
using BraneCloud.Evolution.EC.Runtime;

namespace BraneCloud.Evolution.EC.App.Coevolve2
{
    class Program
    {
        static void Main(string[] args)
        {
            // This application is missing ICCEAStatistics, which no longer seems to be included.
            // It was set in the local ecsuite.params (renamed ecsuite2.params),
            // so that we can have the library param file in the same directory.

            // This primes the activator so it knows where to look for types that will be created from parameters.
            ECActivator.AddSourceAssemblies(new[]
             {
               Assembly.GetAssembly(typeof(IEvolutionState)),
               Assembly.GetAssembly(typeof(CoevolutionaryECSuite))
             });

            Evolve.Run(new[] { "-file", @"Params/App/Coevolve2/coevolve2.params" });
            Console.WriteLine("\nDone!");
            Console.ReadLine();
        }
    }
}

