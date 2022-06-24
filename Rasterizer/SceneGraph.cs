using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Template;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

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

        Shader? diffuseColorShader;
        Shader? staticColorShader;
        Shader? glossyDiffuseShader;
        Shader? postProc;
        Texture? wood;
        Texture? yellow;
        Texture? grey;
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

        List<MeshNode> lights;

        List<Vector3> lightColors;
        List<Vector3> lightPositions;
        public SceneGraph(int width, int height)
        {
            perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
            world = new MeshNode(null);
            diffuseColorShader = new Shader("../../../shaders/vs.glsl", "../../../shaders/diffuse-fs.glsl");
            staticColorShader = new Shader("../../../shaders/vs.glsl", "../../../shaders/static-color-fs.glsl");
            glossyDiffuseShader = new Shader("../../../shaders/vs.glsl", "../../../shaders/diffuse-glossy-fs.glsl");

            postProc = new Shader("../../../shaders/vs_post.glsl", "../../../shaders/fs_post.glsl");
            wood = new Texture("../../../assets/wood.jpg");
            yellow = new Texture("../../../assets/yellow.jpg");
            grey = new Texture("../../../assets/grey.jpg");
            if (useRenderTarget) target = new RenderTarget(width, height);
            quad = new ScreenQuad();
            lightColors = new List<Vector3>();
            lightPositions = new List<Vector3>();
            lights = new List<MeshNode>();

            lightPositions.Add((0, 10, 20));
            lightColors.Add((400, 400, 400));
            lightPositions.Add((0, 10, -20));
            lightColors.Add((400, 400, 400));
            lightPositions.Add((20, 10, 0));
            lightColors.Add((400, 400, 400));

            firstPot = new MeshNode(new Mesh("../../../assets/teapot.obj", yellow, diffuseColorShader, (0, 0, 0), 1));
            secondPot = new MeshNode(new Mesh("../../../assets/teapot.obj", grey, glossyDiffuseShader, (1f, 1f, 1f), 15));
            plane = new MeshNode(new Mesh("../../../assets/floor.obj", wood, diffuseColorShader, (0.4f, 0.4f, 0.4f), 15));
            ape = new MeshNode(new Mesh("../../../assets/monkey.obj", wood, diffuseColorShader, (0, 0, 0), 1));

            lights.Add(new MeshNode(new Mesh("../../../assets/lamp.obj", yellow, staticColorShader, (0, 0, 0), 1)));
            lights.Add(new MeshNode(new Mesh("../../../assets/lamp.obj", yellow, staticColorShader, (0, 0, 0), 1)));
            lights.Add(new MeshNode(new Mesh("../../../assets/lamp.obj", yellow, staticColorShader, (0, 0, 0), 1)));

            world.AddChild(firstPot);
            world.AddChild(plane);
            world.AddChild(ape);
          
            firstPot.thisMesh?.TransformObject(Matrix4.CreateScale(1f));
            plane.thisMesh?.TransformObject(Matrix4.CreateScale(1f));

            firstPot.AddChild(secondPot);
            secondPot.thisMesh?.TransformObject(Matrix4.CreateScale(0.5f) * Matrix4.CreateTranslation(new Vector3(10, 0, 0)));

            InitLights();
            Console.WriteLine(GL.GetError());
        }

        public void InitLights()
        {
            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].thisMesh?.TransformObject(Matrix4.CreateTranslation(lightPositions[i]));
                world.AddChild(lights[i]);
            }
        }

        int sign = 1;
        int x = 2;
        float moved = 0;
        public void LampAnimation(float frameDuration)
        {
            if (moved >= 10)
            {
                moved = 0;
                sign *= -1;
            }
            Vector3 move = new Vector3(x * frameDuration * sign, 0, 0);
            Console.WriteLine(moved);
            moved += Math.Abs(x * frameDuration);
            for (int i = 0; i < lights.Count; i++)
            {
                lightPositions[i] += move;
                lights[i].thisMesh?.TransformObject(Matrix4.CreateTranslation(move));
            }
        }

        public void Render(Matrix4 cameraM)
        {
            float frameDuration = (float)timer.Elapsed.TotalSeconds;
            float calc = 1 / frameDuration;
            //Console.WriteLine("FPS: " + calc);
            timer.Reset();
            timer.Start();

            LampAnimation(frameDuration);

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
                if (childNode.thisMesh.localShader != null && wood != null && childNode.thisMesh != null)
                {
                    Matrix4 toWorld = childNode.thisMesh.localTransform * parentMatrix;
                    Mesh thisMesh = childNode.thisMesh;
                    childNode.thisMesh.Render(thisMesh.localShader, lightPositions, lightColors, toWorld * cameraM * perspectiveMatrix,toWorld, thisMesh.localTexture, cameraM.ExtractTranslation(), thisMesh.specularCoefficient, thisMesh.n);
                    RenderFromChildNode(childNode, cameraM, childNode.thisMesh.localTransform);
                }
            }
        }
    }
}
