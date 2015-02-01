
namespace Jadeite.Parser.Nodes
{
    public abstract class Node
    {
        public abstract string Type { get; }
        public int LineNumber { get; set; }
        public string FileName { get; set; }
        public BlockNode Block { get; set; }
        public CodeNode Code { get; set; }
        public bool Debug { get; set; }

        public virtual bool IsText => false;
        public bool IsTextOnly { get; set; }
    }
}
