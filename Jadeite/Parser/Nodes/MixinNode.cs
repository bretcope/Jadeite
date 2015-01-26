
namespace Jadeite.Parser.Nodes
{
    public class MixinNode : AttributesNode
    {
        public override string Type => "Mixin";

        public string Name { get; }
        public string Args { get; }
        public BlockNode Block { get; }
        public bool Call { get; }

        public MixinNode(string name, string args, BlockNode block, bool call)
        {
            Name = name;
            Args = args;
            Block = block;
            Call = call;
        }
    }
}
