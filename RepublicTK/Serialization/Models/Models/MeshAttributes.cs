namespace RepublicTK.Serialization.Models.Models
{
    public struct MeshAttributes
    {
        private int _value;

        public MeshAttributes()
            : this(1)
        {
        }

        public MeshAttributes(int value)
        {
            _value = value;
        }

        public bool HasColors
        {
            get => (_value & 1 << 2) != 0;
            set => _value |= 1 << 2;
        }

        public bool HasNormals
        {
            get => (_value & 1 << 3) != 0;
            set => _value |= 1 << 3;
        }

        public bool HasTangents
        {
            get => (_value & 1 << 4) != 0;
            set => _value |= 1 << 4;
        }

        public bool HasBitangents
        {
            get => (_value & 1 << 5) != 0;
            set => _value |= 1 << 5;
        }

        public bool HasUVs
        {
            get => (_value & 1 << 8) != 0;
            set => _value |= 1 << 8;
        }

        public bool HasBones
        {
            get => (_value & 1 << 16) != 0;
            set => _value |= 1 << 16;
        }

        public bool HasFaces
        {
            get => (_value & 1 << 18) != 0;
            set => _value |= 1 << 18;
        }

        public int GetVertexSize()
        {
            int vertexSize = 3 * 4;

            if (HasUVs == true)
                vertexSize += 2 * 4;
            if (HasNormals == true)
                vertexSize += 3 * 4;
            if (HasTangents == true)
                vertexSize += 3 * 4;
            if (HasBitangents == true)
                vertexSize += 3 * 4;
            if (HasBones == true)
                vertexSize += 3 * 4;

            return vertexSize;
        }

        public static implicit operator MeshAttributes(int value)
        {
            return new MeshAttributes(value);
        }

        public static implicit operator int(MeshAttributes value)
        {
            return value._value;
        }
    }
}
