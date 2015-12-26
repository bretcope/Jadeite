namespace Jadeite.Parsing
{
    public struct Position
    {
        public int Index { get; internal set; }
        public int Line { get; internal set; }
        public int Column { get; internal set; }
        public int Length { get; internal set; }
    }
}