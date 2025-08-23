using System.Collections;
using System.Numerics;

namespace RepublicTK.Trees.Models
{
    public class TreeGroup : ICollection<Tree>
    {
        private readonly List<Tree> _trees = [];
        public string TreeId { get; set; }

        public TreeGroup(string treeId)
        {
            TreeId = treeId;
        }

        public Tree this[int index]
        {
            get => _trees[index];
            set => _trees[index] = value;
        }

        public TreeGroup(string treeId, int capacity)
            : this(treeId)
        {
            _trees = new List<Tree>(capacity);
        }

        public TreeGroup(string treeId, IEnumerable<Tree> trees)
            : this(treeId)
        {
            _trees = new List<Tree>(trees);
        }

        public TreeGroup(string treeId, IEnumerable<Vector3> positions)
            : this(treeId)
        {
            _trees = new List<Tree>(positions.Select(x => new Tree(x)));
        }

        public int Count
        {
            get => _trees.Count;
        }

        public int Capacity
        {
            get => _trees.Capacity;
        }

        public void Add(Tree tree)
            => _trees.Add(tree);

        public bool Remove(Tree tree)
            => _trees.Remove(tree);

        public void Clear()
            => _trees.Clear();

        public bool Contains(Tree item)
            => _trees.Contains(item);

        public void CopyTo(Tree[] array, int arrayIndex)
            => _trees.CopyTo(array, arrayIndex);

        public IEnumerator<Tree> GetEnumerator()
            => _trees.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _trees.GetEnumerator();

        bool ICollection<Tree>.IsReadOnly => false;
    }
}
