using System.Numerics;

namespace RepublicTK.Utils
{
    public class QuadTreeHelper
    {
        private readonly int _worldOrigin;
        private readonly int _quadtreeSize;
        private readonly float _unitSize;
        private readonly int[,] _regions;

        public QuadTreeHelper()
            : this(20000, 6)
        { }

        public QuadTreeHelper(int worldSize, int quadtreeLevels)
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

        public int GetPositionIndex(Vector3 position)
        {
            return GetPositionIndex(position.X, position.Z);
        }

        public int GetPositionIndex(Vector2 position)
        {
            return GetPositionIndex(position.X, position.Y);
        }

        public int GetPositionIndex(float x, float y)
        {
            int idX = (int)Math.Clamp((x + _worldOrigin) / _unitSize, 0, _quadtreeSize - 1);
            int idZ = (int)Math.Clamp((y + _worldOrigin) / _unitSize, 0, _quadtreeSize - 1);
            return _regions[idX, idZ];
        }
    }
}
