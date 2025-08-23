using System.Numerics;

namespace RepublicTK.Trees.Models
{
    public struct Tree
    {
        public Vector3 Position { get; set; }
        public byte TicksToMaturity { get; set; }

        public Tree()
        { 
        }

        public Tree(Vector3 position)
        {
            Position = position;
        }

        public Tree(Vector3 position, byte ticksToMaturity)
            : this(position)
        {
            TicksToMaturity = ticksToMaturity;
        }

        public override string ToString()
        {
            return $"[Tree]: Position({Position}), TicksToMaturity({TicksToMaturity})";
        }
    }
}
