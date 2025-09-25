namespace RepublicTK.Models.Models
{
    public class Model
    {
        public List<string> Materials { get; set; } = [];
        public List<ModelNode> Nodes { get; set; } = [];

        public IEnumerable<Bone> Bones
        {
            get => Nodes.OfType<Bone>();
        }

        public IEnumerable<Mesh> Meshes
        {
            get => Nodes.OfType<Mesh>();
        }

        public IEnumerable<Helper> Helpers
        {
            get => Nodes.OfType<Helper>();
        }
    }
}
