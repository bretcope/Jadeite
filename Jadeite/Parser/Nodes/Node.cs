
namespace Jadeite.Parser.Nodes
{
    public abstract class Node
    {
        public abstract string Type { get; }
        public int LineNumber { get; set; }
        public string FileName { get; set; }
        public Node Block { get; set; }
        public bool Debug { get; set; }

        public virtual bool IsBlock => false;
        public virtual bool IsText => false;
        public virtual bool IsTextOnly => false;
        public virtual bool IsYield { get; set; }
    }
}
