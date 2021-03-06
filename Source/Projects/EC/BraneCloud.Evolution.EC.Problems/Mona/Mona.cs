using System.IO;
using BraneCloud.Evolution.EC.Configuration;
using BraneCloud.Evolution.EC.Simple;
using BraneCloud.Evolution.EC.Vector;

namespace BraneCloud.Evolution.EC.Problems.Mona
{

    [ECConfiguration("ec.problems.mona.Mona")]
    public class Mona : Problem, ISimpleProblem
    {
        public const string P_IN = "in";
        public const string P_OUT = "out";
        public const string P_VERTICES = "num-vertices";

        public Picture pic = new Picture();
        public FileInfo In;
        public FileInfo Out;
        public int NumVertices;

        public override object Clone()
        {
            Mona m = (Mona) base.Clone();
            m.pic = (Picture) pic.Clone();
            return m;
        }

        public override void Setup(IEvolutionState state, IParameter paramBase)
        {
            base.Setup(state, paramBase);
            In = state.Parameters.GetFile(paramBase.Push(P_IN), null);
            Out = state.Parameters.GetFile(paramBase.Push(P_OUT), null);
            NumVertices = state.Parameters.GetInt(paramBase.Push(P_VERTICES), null, 3);
            if (NumVertices < 3) state.Output.Fatal("Number of vertices must be >= 3");
            pic.Load(In);
        }

        public void Evaluate(IEvolutionState state, Individual ind, int subpopulation, int threadnum)
        {
            if (ind.Evaluated) return;

            DoubleVectorIndividual _ind = (DoubleVectorIndividual) ind;
            int vertexSkip = NumVertices * 2 + 4; // for four colors

            pic.Clear();
            for (int i = 0; i < _ind.genome.Length; i += vertexSkip)
                pic.AddPolygon(_ind.genome, i, NumVertices);

            double error = pic.Error();
            ((SimpleFitness) _ind.Fitness).SetFitness(state, (float) (1.0 - error), error == 0);
            ind.Evaluated = true;
        }

        public override void FinishEvaluating(IEvolutionState state, int threadnum)
        {
            pic.DisposeGraphics(); // dutifully
        }

        public override void Describe(
            IEvolutionState state,
            Individual ind,
            int threadnum,
            int subpopulation,
            int log)
        {
            ind.Evaluated = false;
            Evaluate(state, ind, subpopulation, threadnum);

            var fi = new FileInfo(Path.Combine(Out.DirectoryName, state.Generation + "-" + Out.Name));
            pic.Save(fi); // not sure if "." is acceptable in Windows
            pic.Display("Best So Far, Generation " + state.Generation);
            // System.out.println("Filled Polygons: " + pic.GetLatestFilledPolygonCount() + " of " + pic.GetLatestTotalCount());
            ind.Evaluated = true;
        }
    }
}