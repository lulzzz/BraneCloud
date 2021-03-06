using BraneCloud.Evolution.EC.Configuration;

namespace BraneCloud.Evolution.EC.Problems.GPSemantics.Func
{

    [ECConfiguration("ec.problems.gpsemantics.func.SemanticX6")]
    public class SemanticX6 : SemanticNode
    {
        public override char Value => 'X';

        public override int Index => 6;
    }
}