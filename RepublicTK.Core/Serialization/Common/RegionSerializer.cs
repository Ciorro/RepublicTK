using RepublicTK.Core.Models;

namespace RepublicTK.Core.Serialization.Common
{
    public class RegionSerializer : ISerializer<Region>
    {
        private readonly int _worldOrigin;
        private readonly int _quadtreeSize;
        private readonly float _unitSize;
        private readonly int[,]? _regions;

        public RegionSerializer()
            : this(20000, 6)
        { }

        public RegionSerializer(int worldSize, int quadtreeLevels)
        {
            _quadtreeSize = (int)Math.Pow(2, quadtreeLevels);
            _regions = new int[_quadtreeSize, _quadtreeSize];
            _worldOrigin = worldSize / 2;
            _unitSize = worldSize / _quadtreeSize;

            int index = 0;

            for (int i = 0; i < _quadtreeSize * _quadtreeSize; i++)
            {
                for (int j = 0; j < quadtreeLevels; j++)
                {
                    if (i % (int)Math.Pow(4, j + 1) == 0)
                    {
                        index++;
                    }
                }

                int x = 0;
                int z = 0;

                for (int j = 0; j < quadtreeLevels; j++)
                {
                    if ((i >> j * 2 & 0b01) == 0b01)
                    {
                        x += (int)Math.Pow(2, j);
                    }

                    if ((i >> j * 2 & 0b10) == 0b10)
                    {
                        z += (int)Math.Pow(2, j);
                    }
                }

                _regions[x, z] = index++;
            }
        }

        public Region Read(BinaryReader reader, SerializationContext context)
        {
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter writer, SerializationContext context, Region value)
        {
            writer.Write(GetRegion(value.RegionPosition.X, value.RegionPosition.Z));
        }

        private int GetRegion(double x, double y)
        {
            x += _worldOrigin;
            y += _worldOrigin;

            int idX = (int)Math.Clamp(x / _unitSize, 0, _quadtreeSize - 1);
            int idZ = (int)Math.Clamp(y / _unitSize, 0, _quadtreeSize - 1);

            return _regions![idX, idZ];
        }
    }
}
