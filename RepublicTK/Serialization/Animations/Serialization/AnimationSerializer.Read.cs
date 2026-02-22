using RepublicTK.Serialization.Animations.Models;
using RepublicTK.Serialization.Extensions;

namespace RepublicTK.Serialization.Animations.Serialization
{
    public partial class AnimationSerializer : ISerializer<Animation>
    {
        public Animation Read(BinaryReader reader)
        {
            string header = reader.ReadCString(8);
            if (!IsValidHeader(header))
            {
                throw new InvalidDataException($"Invalid file header \"{header}\". Expected \"B3DAM\"");
            }

            int format = reader.ReadInt32();
            int boneCount = reader.ReadInt32();
            int fps = reader.ReadInt32();
            int spf = reader.ReadInt32();
            int frameCount = reader.ReadInt32();
            int size = reader.ReadInt32();

            var animation = new Animation(frameCount)
            {
                Fps = fps,
                Spf = spf
            };

            for (int i = 0; i < boneCount; i++)
            {
                animation.AddBone(ReadBone(reader, frameCount));
            }

            return animation;
        }

        private Bone ReadBone(BinaryReader reader, int frameCount)
        {
            var bone = new Bone(reader.ReadCString(64), frameCount);

            for (int i = 0; i < frameCount; i++)
            {
                bone[i] = reader.ReadMatrix4x4();
            }

            return bone;
        }

        private bool IsValidHeader(string header)
        {
            return header == "B3DAM";
        }
    }
}
