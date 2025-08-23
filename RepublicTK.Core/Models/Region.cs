using System.Numerics;

namespace RepublicTK.Core.Models
{
    public struct Region
    {
        public Vector3 RegionPosition { get; set; }

        public Region() { }

        public Region(Vector3 regionPosition)
        {
            RegionPosition = regionPosition;
        }

        public static implicit operator Region(Vector3 region)
            => new Region(region);

        public static implicit operator Vector3(Region region)
            => region.RegionPosition;
    }
}
