
namespace Jadeite.Parser.Nodes
{
    public class CaseNode : Node
    {
        public override string Type => "Case";

        public string Expression { get; }

        public CaseNode(string expression, BlockNode block = null)
        {
            Expression = expression;
            Block = block;
        }
    }
}
