using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Template;
using System.Diagnostics;

/*
▪ Add a model matrix to the Mesh class. DONE

▪ Add the SceneGraph class. DONE

▪ Add a data structure for storing a hierarchy of meshes in the scene graph. DONE

▪ Add a Render method to the scene graph class that recursively processes the nodes in
the tree, while combining matrices so that each mesh is drawn using the correct
combined matrix. DONE

▪ Call the Render method of the SceneGraph from the Game class, using a camera matrix
that is updated based on user input. DONE
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

        Stopwatch timer = new Stopwatch();

        float xAddTemp = 0.5f;

        MeshNode firstPot;
        MeshNode secondPot;
        MeshNode plane;
        MeshNode ape;
        public SceneGraph(int width, int height)
        {
            perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            world = new MeshNode(null);
            renderShader = new Shader("../../../shaders/vs.glsl", "../../../shaders/fs.glsl");
            postProc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            wood = new Texture("../../../assets/wood.jpg");
            if (useRenderTarget) target = new RenderTarget(width, height);
            quad = new ScreenQuad();

            firstPot = new MeshNode(new Mesh("../../../assets/teapot.obj"));
            secondPot = new MeshNode(new Mesh("../../../assets/teapot.obj"));
            plane = new MeshNode(new Mesh("../../../assets/floor.obj"));
            ape = new MeshNode(new Mesh("../../../assets/monkey.obj"));

            world.AddChild(firstPot);
            world.AddChild(plane);
            world.AddChild(ape);
            if (ape.thisMesh != null)
            {
                Console.WriteLine("Mesh created");
            }
            
            firstPot.thisMesh?.TransformObject(Matrix4.CreateScale(1f));
            plane.thisMesh?.TransformObject(Matrix4.CreateScale(1f));

            firstPot.AddChild(secondPot);
            secondPot.thisMesh?.TransformObject(Matrix4.CreateScale(0.5f) * Matrix4.CreateTranslation(new Vector3(10, 0, 0)));
        }

        public void Render(Matrix4 cameraM)
        {
            float frameDuration = (float)timer.Elapsed.TotalSeconds;
            float calc = 1 / frameDuration;
            //Console.WriteLine("FPS: " + calc);
            timer.Reset();
            timer.Start();

            world.childNodes[0].thisMesh?.TransformObject(Matrix4.CreateTranslation((-xAddTemp * frameDuration, 0, 0)));
            if (useRenderTarget) target?.Bind();
            RenderFromChildNode(world, cameraM, Matrix4.Identity);
            if (useRenderTarget) target?.Unbind();
            if (useRenderTarget && postProc != null && target != null && quad != null)
            {
                quad.Render(postProc, target.GetTextureID());
            }
        }

        public void RenderFromChildNode(MeshNode node, Matrix4 cameraM, Matrix4 parentMatrix) {
            for (int i = 0; i < node.childNodes.Count; i++)
            {
                MeshNode childNode = node.childNodes[i];
                if (renderShader != null && wood != null && childNode.thisMesh != null)
                {
                    childNode.thisMesh.Render(renderShader,childNode.thisMesh.localTransform * parentMatrix * cameraM * perspectiveMatrix, wood);
                    RenderFromChildNode(childNode, cameraM, childNode.thisMesh.localTransform);
                }
            }
        }
    }
}
