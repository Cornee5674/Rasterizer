using System.Diagnostics;
using OpenTK.Mathematics;
using Rasterizer;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        SceneGraph sceneGraph;

        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            sceneGraph = new SceneGraph(screen.width, screen.height);
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            float angle90degrees = MathF.PI / 2f;
            Matrix4 Tcamera = Matrix4.CreateTranslation(new Vector3(0, -14.5f, 0)) * Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), angle90degrees);

            sceneGraph.Render(Tcamera);
        }
    }
}