using BraneCloud.Evolution.EC.Configuration;

namespace BraneCloud.Evolution.EC.App.GPSemantics.Func
{

    [ECConfiguration("ec.app.gpsemantics.func.SemanticN3")]
    public class SemanticN3 : SemanticNode
    {
        public override char Value => 'N';

        public override int Index => 3;

    }
}