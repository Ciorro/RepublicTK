using RepublicTK.Serialization;
using RepublicTK.Serialization.Extensions;
using RepublicTK.Serialization.Trees.Models;
using System.Numerics;

namespace RepublicTK.Trees.Serialization
{
    public sealed partial class TreesSerializer : ISerializer<TreeGroupCollection>
    {
        public TreeGroupCollection Read(BinaryReader reader)
        {
            int numGroups = reader.ReadInt32();
            int fileSize = reader.ReadInt32();

            var treeGroups = new TreeGroupCollection();

            for (int i = 0; i < numGroups; i++)
            {
                treeGroups.Add(ReadTreeGroup(reader));
            }

            return treeGroups;
        }

        private TreeGroup ReadTreeGroup(BinaryReader reader)
        {
            string treeId = reader.ReadCString(64);
            int treeCount = reader.ReadInt32();

            var group = new TreeGroup(treeId, treeCount);

            for (int i = 0; i < treeCount; i++)
            {
                group.Add(ReadTree(reader));
            }

            return group;
        }

        private Tree ReadTree(BinaryReader reader)
        {
            Vector3 position = reader.ReadVector3();
            int quadTreeIndex = reader.ReadInt32();
            byte ticksToMaturity = reader.ReadByte();

            return new Tree(position, ticksToMaturity);
        }
    }
}
