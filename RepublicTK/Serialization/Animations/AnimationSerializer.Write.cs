using RepublicTK.Serialization.Animations.Models;
using RepublicTK.Serialization.Extensions;

namespace RepublicTK.Serialization.Animations
{
    public partial class AnimationSerializer : ISerializer<Animation>
    {
        public void Write(BinaryWriter writer, Animation value)
        {
            writer.WriteCString("B3DAM\010", 8);
            writer.Write(0);
            writer.Write(value.Bones.Count);
            writer.Write(value.Fps);
            writer.Write(value.Spf);
            writer.Write(value.FrameCount);
            writer.Write(0);

            foreach (var bone in value)
            {
                WriteBone(writer, bone);
            }
        }

        private void WriteBone(BinaryWriter writer, Bone bone)
        {
            writer.WriteCString(bone.Name, 64);

            foreach (var frame in bone)
            {
                writer.Write(frame);
            }
        }
    }
}
