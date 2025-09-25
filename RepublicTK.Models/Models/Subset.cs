namespace RepublicTK.Models.Models
{
    public class Subset
    {
        public int StartIndex { get; set; }
        public int IndexCount { get; set; }
        public short MaterialId { get; set; }
        public List<short> BoneIds { get; set; } = [];

        internal Subset()
        {
        }

        public Subset(int startIndex, int indexCount)
        {
            StartIndex = startIndex;
            IndexCount = indexCount;
        }

        public Subset(int startIndex, int indexCount, short materialId)
            : this(startIndex, indexCount)
        {
            MaterialId = materialId;
        }

        internal int GetSize()
        {
            return 12 + (BoneIds.Count * sizeof(short));
        }
    }
}
