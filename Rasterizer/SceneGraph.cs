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
        Texture? lightBlue;
        RenderTarget? target;
        ScreenQuad? quad;
        readonly bool useRenderTarget = false;

        Matrix4 perspectiveMatrix;

        Stopwatch timer = new Stopwatch();

        MeshNode plane;
        MeshNode well;
        MeshNode character;
        MeshNode spinningTeaPot;
        MeshNode spinningTeaPot2;
        MeshNode car;
        MeshNode wheel1;
        MeshNode wheel2;
        MeshNode wheel3;
        MeshNode wheel4;
        MeshNode smoke1;

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
            lightBlue = new Texture("../../../assets/lightBlue.jpg");
            grey = new Texture("../../../assets/grey.jpg");
            if (useRenderTarget) target = new RenderTarget(width, height);
            quad = new ScreenQuad();
            lightColors = new List<Vector3>();
            lightPositions = new List<Vector3>();
            lights = new List<MeshNode>();

            lightPositions.Add((0, 7, 15));
            lightColors.Add((50, 50, 50));
            lightPositions.Add((-5, 7, 0));
            lightColors.Add((50, 50, 50));

            plane = new MeshNode(new Mesh("../../../assets/plane.obj", wood, diffuseColorShader, (0, 0, 0), 1));
            plane.thisMesh?.TransformObject(Matrix4.CreateScale((100f, 1f, 100f)));
            world.AddChild(plane);

            well = new MeshNode(new Mesh("../../../assets/well.obj", wood, diffuseColorShader, (0, 0, 0), 1));
            well.thisMesh?.TransformObject(Matrix4.CreateTranslation((0, 0.1f, 7)));
            world.AddChild(well);

            character = new MeshNode(new Mesh("../../../assets/jack.obj", lightBlue, diffuseColorShader, (0, 0, 0), 1));
            character.thisMesh?.TransformObject(Matrix4.CreateScale(2f));
            character.thisMesh.TransformObject(Matrix4.CreateTranslation((5, 0.1f, 4)));
            world.AddChild(character);

            spinningTeaPot = new MeshNode(new Mesh("../../../assets/teapot.obj", grey, glossyDiffuseShader, (1f, 1f, 1f), 15));
            spinningTeaPot.thisMesh?.TransformObject(Matrix4.CreateTranslation((-11, 0.1f, -5)));
            world.AddChild(spinningTeaPot);
            spinningTeaPot2 = new MeshNode(new Mesh("../../../assets/teapot.obj", grey, diffuseColorShader, (1f, 1f, 1f), 15));
            spinningTeaPot2.thisMesh?.TransformObject(Matrix4.CreateTranslation((11, 0.1f, -5)));
            world.AddChild(spinningTeaPot2);

            car = new MeshNode(new Mesh("../../../assets/car.obj", yellow, glossyDiffuseShader, (1f, 1f, 1f), 8));
            car.thisMesh?.TransformObject(Matrix4.CreateTranslation(-6, 0.1f, 7));
            world.AddChild(car);
            wheel1 = new MeshNode(new Mesh("../../../assets/wheel.obj", grey, diffuseColorShader, (0, 0, 0), 1));
            wheel1.thisMesh?.TransformObject(Matrix4.CreateTranslation(1.5f, 0.6f, 1));
            car.AddChild(wheel1);
            smoke1 = new MeshNode(new Mesh("../../../assets/smoke.obj", grey, staticColorShader, (0, 0, 1), 1));
            smoke1.thisMesh?.TransformObject(Matrix4.CreateTranslation(0.5f, 0, 0));
            wheel1.AddChild(smoke1);
            wheel2 = new MeshNode(new Mesh("../../../assets/wheel.obj", grey, diffuseColorShader, (0, 0, 0), 1));
            wheel2.thisMesh?.TransformObject(Matrix4.CreateTranslation(-1.5f, 0.6f, 1));
            car.AddChild(wheel2);
            wheel3 = new MeshNode(new Mesh("../../../assets/wheel.obj", grey, diffuseColorShader, (0, 0, 0), 1));
            wheel3.thisMesh?.TransformObject(Matrix4.CreateTranslation(1.5f, 0.6f, -1));
            car.AddChild(wheel3);
            wheel4 = new MeshNode(new Mesh("../../../assets/wheel.obj", grey, diffuseColorShader, (0, 0, 0), 1));
            wheel4.thisMesh?.TransformObject(Matrix4.CreateTranslation(-1.5f, 0.6f, -1));
            car.AddChild(wheel4);

            lights.Add(new MeshNode(new Mesh("../../../assets/lamp.obj", yellow, staticColorShader, (0, 0, 0), 1)));
            lights.Add(new MeshNode(new Mesh("../../../assets/lamp.obj", yellow, staticColorShader, (0, 0, 0), 1)));

            InitLights();
            Console.WriteLine(GL.GetError());
        }

        public void MoveCarAndWheels(float frameDuration)
        {
            Vector3 move = new Vector3(x * frameDuration * sign * -1, 0, 0);
            car.thisMesh?.TransformObject(Matrix4.CreateTranslation(move));
            RotateObject(wheel1, frameDuration, (0, 0, 1), sign);
            RotateObject(wheel2, frameDuration, (0, 0, 1), sign);
            RotateObject(wheel3, frameDuration, (0, 0, 1), sign);
            RotateObject(wheel4, frameDuration, (0, 0, 1), sign);
        }

        public void moveJack(float frameDuration)
        {
            Vector3 move = new Vector3(0, 0, x * frameDuration * sign);
            character.thisMesh?.TransformObject(Matrix4.CreateTranslation(move));
        }

        public void RotateTeapot(float frameDuration)
        {
            RotateObject(spinningTeaPot, frameDuration, (0, 1, 0), sign);
            RotateObject(spinningTeaPot2, frameDuration, (0, 1, 0), sign);
        }

        public void RotateObject(MeshNode mesh, float frameDuration, Vector3 axis, int additionalSign)
        {
            Vector3 translation = mesh.thisMesh.localTransform.ExtractTranslation();
            mesh.thisMesh?.TransformObject(Matrix4.CreateTranslation(translation * -1));
            mesh.thisMesh?.TransformObject(Matrix4.CreateFromAxisAngle(axis, x * frameDuration * additionalSign));
            mesh.thisMesh?.TransformObject(Matrix4.CreateTranslation(translation));
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
            moved += Math.Abs(x * frameDuration);
            for (int i = 0; i < 2; i++)
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
            moveJack(frameDuration);
            RotateTeapot(frameDuration);
            MoveCarAndWheels(frameDuration);

            //world.childNodes[0].thisMesh?.TransformObject(Matrix4.CreateTranslation((-xAddTemp * frameDuration, 0, 0)));
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
                    RenderFromChildNode(childNode, cameraM, toWorld);
                }
            }
        }
    }
}
