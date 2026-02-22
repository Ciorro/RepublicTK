using System.Collections;

namespace RepublicTK.Serialization.Animations.Models
{
    public class Animation : IEnumerable<Bone>
    {
        private readonly List<Bone> _bones = [];

        public int Fps { get; set; }
        public int Spf { get; set; }
        public int FrameCount { get; }

        public IReadOnlyList<Bone> Bones => _bones;

        public Animation(int frameCount)
        {
            FrameCount = frameCount;
        }

        public void AddBone(Bone bone)
        {
            if (bone.FrameCount != FrameCount)
            {
                throw new InvalidOperationException($"Invalid frame count: {bone.FrameCount}, expected: {FrameCount}");
            }

            _bones.Add(bone);
        }

        public bool RemoveBone(Bone bone)
            => _bones.Remove(bone);

        public void ClearBones()
            => _bones.Clear();

        public Bone CreateBone(string name)
            => new Bone(name, FrameCount);

        public IEnumerator<Bone> GetEnumerator()
            => _bones.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _bones.GetEnumerator();
    }
}
