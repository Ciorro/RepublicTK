using System.Collections;

namespace RepublicTK.Trees.Models
{
    public class TreeGroupCollection : IEnumerable<TreeGroup>
    {
        private readonly Dictionary<string, TreeGroup> _groups = new();

        public TreeGroupCollection()
        {
        }

        public TreeGroupCollection(IEnumerable<TreeGroup> groups)
        {
            foreach (var group in groups)
            {
                Add(group);
            }
        }

        public int Count
        {
            get => _groups.Count;
        }

        public int TreeCount
        {
            get => _groups.Values.Sum(x => x.Count);
        }

        public TreeGroup this[string key]
        {
            get => _groups[key];
        }

        public void Add(TreeGroup item)
        {
            if (_groups.TryGetValue(item.TreeId, out var group))
            {
                group.AddRange(item);
            }
            else
            {
                _groups.Add(item.TreeId, item);
            }
        }

        public bool Remove(string treeId)
            => _groups.Remove(treeId);

        public void Clear()
            => _groups.Clear();

        public bool ContainsGroup(string treeId)
            => _groups.ContainsKey(treeId);

        public void Merge(TreeGroupCollection other)
        {
            foreach (var group in other)
            {
                Add(group);
            }
        }

        public IEnumerator<TreeGroup> GetEnumerator()
            => _groups.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _groups.Values.GetEnumerator();
    }
}
