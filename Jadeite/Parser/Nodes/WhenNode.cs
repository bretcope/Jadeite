namespace Jadeite.Parser.Nodes
{
    public class WhenNode : Node
    {
        public override string Type => "When";
        public string Expression { get; }

        public WhenNode(string expression, BlockNode block = null)
        {
            Expression = expression;
            Block = block;
            Debug = false;
        }
    }
}