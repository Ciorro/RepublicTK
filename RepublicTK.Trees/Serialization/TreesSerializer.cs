using RepublicTK.Core.Extensions;
using RepublicTK.Core.Serialization;
using RepublicTK.Trees.Models;

namespace RepublicTK.Trees.Serialization
{
    public class TreesSerializer : ISerializer<ICollection<TreeGroup>>
    {
        public ICollection<TreeGroup> Read(BinaryReader reader, SerializationContext context)
        {
            int numGroups = reader.ReadInt32();
            int fileLength = reader.ReadInt32();

            var groups = new List<TreeGroup>(numGroups);

            for (int i = 0; i < numGroups; i++)
            {
                groups.Add(context.Read<TreeGroup>(reader));
            }

            return groups;
        }

        public void Write(BinaryWriter writer, SerializationContext context, ICollection<TreeGroup> value)
        {
            int fileSize = CalcFileSize(value);

            writer.Write(value.Count());
            writer.Write(fileSize);

            foreach (var group in value)
            {
                context.Write(writer, group);
            }

            writer.Write(fileSize);
        }

        private int CalcFileSize(IEnumerable<TreeGroup> value)
        {
            return value.Sum(group => group.Count * 17 + 68) + 12;
        }
    }
}
