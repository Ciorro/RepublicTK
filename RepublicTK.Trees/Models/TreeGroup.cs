using System.Numerics;

namespace RepublicTK.Trees.Models
{
    public class TreeGroup : List<Tree>
    {
        public string TreeId { get; set; }

        public TreeGroup(string treeId)
        {
            TreeId = treeId;
        }

        public TreeGroup(string treeId, int capacity)
            : base(capacity)
        {
            TreeId = treeId;
        }

        public TreeGroup(string treeId, IEnumerable<Tree> collection)
            : base(collection)
        {
            TreeId = treeId;
        }

        public TreeGroup(string treeId, IEnumerable<Vector3> collection)
            : base(collection.Select(x => new Tree(x)))
        {
            TreeId = treeId;
        }
    }
}
