using System.Collections;
using System.Numerics;

namespace RepublicTK.Serialization.Animations.Models
{
    public class Bone : IEnumerable<Matrix4x4>
    {
        private Matrix4x4[] _frames;

        public string Name { get; set; }
        
        public Matrix4x4 this[int index]
        {
            get => _frames[index];
            set => _frames[index] = value;
        }
        public int FrameCount => _frames.Length;

        public Bone(string name, int frameCount)
        {
            Name = name;
            _frames = new Matrix4x4[frameCount];
        }

        public Bone(string name, IEnumerable<Matrix4x4> frames)
        {
            Name = name;
            _frames = [..frames];
        }

        public IEnumerator<Matrix4x4> GetEnumerator()
            => _frames.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _frames.GetEnumerator();
    }
}
