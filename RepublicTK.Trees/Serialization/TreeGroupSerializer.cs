using RepublicTK.Core.Extensions;
using RepublicTK.Core.Models;
using RepublicTK.Core.Serialization;
using RepublicTK.Trees.Models;
using System.Numerics;
using System.Text;

namespace RepublicTK.Trees.Serialization
{
    public class TreeGroupSerializer : ISerializer<TreeGroup>
    {
        public TreeGroup Read(BinaryReader reader, SerializationContext context)
        {
            var group = new TreeGroup(
                treeId: reader.ReadCString(Encoding.ASCII, 64),
                capacity: reader.ReadInt32());

            for (int i = 0; i < group.Capacity; i++)
            {
                var position = context.Read<Vector3>(reader);
                var quadTreeIndex = reader.ReadInt32();
                var ticksToMaturity = reader.ReadByte();

                group.Add(new Tree(position, ticksToMaturity));
            }

            return group;
        }

        public void Write(BinaryWriter writer, SerializationContext context, TreeGroup value)
        {
            writer.WriteCString(value.TreeId, Encoding.ASCII, 64);
            writer.Write(value.Count);

            foreach (var treePosition in value)
            {
                context.Write<Vector3>(writer, treePosition.Position);
                context.Write<Region>(writer, treePosition.Position);
                writer.Write(treePosition.TicksToMaturity);
            }
        }
    }
}
