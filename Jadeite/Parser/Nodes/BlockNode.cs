
using System.Collections.Generic;

namespace Jadeite.Parser.Nodes
{
    public class BlockNode : Node
    {
        private readonly List<Node> _nodes = new List<Node>();

        public override string Type => "Block";
        public override bool IsBlock => true;
        public IReadOnlyList<Node> Nodes { get; }

        public bool IsEmpty => _nodes.Count == 0;

        public BlockNode(Node node = null)
        {
            Nodes = _nodes.AsReadOnly();
            if (node != null)
                _nodes.Add(node);
        }

        public int PushNode(Node node)
        {
            _nodes.Add(node);
            return _nodes.Count;
        }

        public int UnshiftNode(Node node)
        {
            _nodes.Insert(0, node);
            return _nodes.Count;
        }

        // Return the "last" block, or the first `yield` node.
        public Node IncludeBlock()
        {
            var ret = this;

            foreach (var node in _nodes)
            {
                //if ()
            }

            return ret;
        }
    }
}
