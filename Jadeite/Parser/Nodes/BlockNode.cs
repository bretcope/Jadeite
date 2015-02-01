
using System;
using System.Collections.Generic;

namespace Jadeite.Parser.Nodes
{
    public class BlockNode : Node
    {

        public override string Type => "Block";
        public override bool IsBlock => true;
        public bool IsSubBlock { get; set; }
        public string Name { get; set; }
        public List<Node> Nodes { get; set; } = new List<Node>();

        public bool IsEmpty => Nodes.Count == 0;

        internal Parser Parser { get; set; }
        public List<Node> Appended { get; set; } = new List<Node>();
        public List<Node> Prepended { get; set; } = new List<Node>();
        public BlockMode Mode { get; set; }

        public BlockNode(Node node = null)
        {
            if (node != null)
                Nodes.Add(node);
        }

        public int PushNode(Node node)
        {
            Nodes.Add(node);
            return Nodes.Count;
        }

        public int UnshiftNode(Node node)
        {
            Nodes.Insert(0, node);
            return Nodes.Count;
        }

        // Return the "last" block, or the first `yield` node.
        public Node IncludeBlock()
        {
            var ret = this;

            foreach (var node in Nodes)
            {
                //if ()
            }

            throw new NotImplementedException();
            return ret;
        }
    }
}
