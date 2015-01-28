namespace Jadeite.Parser.Nodes
{
    public class EachNode : Node
    {
         public override string Type => "Each";

        public string Object { get; }
        public string Value { get; }
        public string Key { get; }
        public BlockNode Alternative { get; set; }

        public EachNode(string obj, string value, string key, BlockNode block = null)
        {
            Object = obj;
            Value = value;
            Key = key;
            Block = block;
        }
    }
}