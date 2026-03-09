using RepublicTK.Serialization;
using RepublicTK.Serialization.Extensions;
using RepublicTK.Serialization.Trees.Models;
using RepublicTK.Utils;

namespace RepublicTK.Serialization.Trees
{
    public sealed partial class TreesSerializer : ISerializer<TreeGroupCollection>
    {
        private readonly QuadTreeHelper _quadTree = new();

        public void Write(BinaryWriter writer, TreeGroupCollection value)
        {
            int fileSize = CalcFileSize(value);

            writer.Write(value.Count);
            writer.Write(fileSize);

            foreach (var group in value)
            {
                WriteTreeGroup(writer, group);
            }

            writer.Write(fileSize);
        }

        private void WriteTreeGroup(BinaryWriter writer, TreeGroup group)
        {
            writer.WriteCString(group.TreeId, 64);
            writer.Write(group.Count);

            foreach (var tree in group)
            {
                WriteTree(writer, tree);
            }
        }

        private void WriteTree(BinaryWriter writer, Tree tree)
        {
            int quadTreeIndex = _quadTree.GetPositionIndex(tree.Position);

            writer.Write(tree.Position);
            writer.Write(quadTreeIndex);
            writer.Write(tree.TicksToMaturity);
        }

        private int CalcFileSize(IEnumerable<TreeGroup> value)
        {
            return value.Sum(group => group.Count * 17 + 68) + 12;
        }
    }
}
