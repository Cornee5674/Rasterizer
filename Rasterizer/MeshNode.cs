using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template;

namespace Rasterizer
{
    public class MeshNode
    {
        // A mesh node stores a list of children meshnodes, and the mesh itself.
        public List<MeshNode> childNodes;
        public Mesh? thisMesh;

        public MeshNode(Mesh mesh)
        {
            if (mesh != null) this.thisMesh = mesh;
            else thisMesh = null;
            childNodes = new List<MeshNode>();
        }

        public void AddChild(MeshNode child)
        {
            childNodes.Add(child);
        }
    }
}
