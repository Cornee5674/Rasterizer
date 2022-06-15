using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Template;

/*
▪ Add a model matrix to the Mesh class. DONE

▪ Add the SceneGraph class. DONE

▪ Add a data structure for storing a hierarchy of meshes in the scene graph. DONE

▪ Add a Render method to the scene graph class that recursively processes the nodes in
the tree, while combining matrices so that each mesh is drawn using the correct
combined matrix.

▪ Call the Render method of the SceneGraph from the Game class, using a camera matrix
that is updated based on user input.
*/

namespace Rasterizer
{
    public class SceneGraph
    {
        MeshNode world;

        Shader? renderShader;
        Shader? postProc;
        Texture? wood;
        RenderTarget? target;
        ScreenQuad? quad;
        readonly bool useRenderTarget = false;

        Matrix4 perspectiveMatrix;

        public SceneGraph(int width, int height)
        {
            perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            world = new MeshNode(null);
            renderShader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postProc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            wood = new Texture("../../../assets/wood.jpg");
            if (useRenderTarget) target = new RenderTarget(width, height);
            quad = new ScreenQuad();
            world.AddChild(new MeshNode(new Mesh("../../../assets/teapot.obj")));
            world.AddChild(new MeshNode(new Mesh("../../../assets/floor.obj")));
            float angle90degrees = MathF.PI / 2f;
            world.childNodes[0].thisMesh?.TransformObject(Matrix4.CreateScale(0.5f) * Matrix4.CreateFromAxisAngle(new Vector3(-1, 0, 0), angle90degrees));
            world.childNodes[1].thisMesh?.TransformObject(Matrix4.CreateScale(4.0f) * Matrix4.CreateFromAxisAngle(new Vector3(-1, 0, 0), angle90degrees));
        }

        public void Render(Matrix4 cameraM)
        {
            target?.Bind();
            for (int i = 0; i < world.childNodes.Count; i++)
            {             
                MeshNode childNode = world.childNodes[i];
                if (useRenderTarget && target != null && quad != null)
                {
                    if (renderShader != null && wood != null)
                    {
                        childNode.thisMesh?.Render(renderShader, childNode.thisMesh.localTransform * cameraM * perspectiveMatrix, wood);
                    }
                    if (postProc != null)
                    {
                        quad.Render(postProc, target.GetTextureID());
                    }

                }else
                {
                    if (renderShader != null && wood != null)
                    {
                        childNode.thisMesh?.Render(renderShader, childNode.thisMesh.localTransform * cameraM * perspectiveMatrix, wood);
                    }
                }               
            }
            target?.Unbind();
        }
    }
}
