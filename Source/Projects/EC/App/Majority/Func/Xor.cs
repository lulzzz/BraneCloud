using BraneCloud.Evolution.EC.Configuration;
using BraneCloud.Evolution.EC.GP;

namespace BraneCloud.Evolution.EC.App.Majority.Func
{
    [ECConfiguration("ec.app.majority.func.Xor")]
    public class Xor : GPNode
    {
        public override string ToString()
        {
            return "xor";
        }

        public override int ExpectedChildren => 2;

        public override void Eval(IEvolutionState state,
            int thread,
            GPData input,
            ADFStack stack,
            GPIndividual individual,
            IProblem problem)
        {
            Children[0].Eval(state, thread, input, stack, individual, problem);

            var md = (MajorityData) input;
            long y0 = md.Data0;
            long y1 = md.Data1;

            Children[1].Eval(state, thread, input, stack, individual, problem);

            md.Data0 = md.Data0 ^ y0;
            md.Data1 = md.Data1 ^ y1;
        }
    }
}


