using System.Diagnostics;
using OpenTK.Mathematics;
using Rasterizer;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        SceneGraph sceneGraph;
        Matrix4 Tcamera;

        Vector3 cameraPos = (0, 0, 20);
        Vector3 forward = (0, 0, -1);
        Vector3 up = (0, 1, 0);

        float frameDuration = 0f;

        Stopwatch timer = new Stopwatch();
        float movementPerSecond = 4f;
        float anglePerSecond = 30f;

        KeyboardState keyboardState;

        // constructor
        public MyApplication(Surface screen, KeyboardState kbs)
        {
            this.keyboardState = kbs;
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            sceneGraph = new SceneGraph(screen.width, screen.height);
            Tcamera = Matrix4.LookAt(cameraPos, cameraPos + forward, up);       
        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            //screen.Print("hello world", 2, 2, 0xffff00);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            ProcessMovementInput();
            ProcessRotationInput();
            
            Tcamera = Matrix4.LookAt(cameraPos, cameraPos + forward, up);

            frameDuration = (float)timer.Elapsed.TotalSeconds;
            timer.Reset();
            timer.Start();
            sceneGraph.Render(Tcamera);
        }
        float pitch = 0;
        float yaw = -90;
        public void ProcessRotationInput()
        {
            if (keyboardState.IsKeyDown(Keys.Up)) pitch += anglePerSecond * frameDuration;
            if (keyboardState.IsKeyDown(Keys.Down)) pitch -= anglePerSecond * frameDuration;
            if (keyboardState.IsKeyDown(Keys.Right)) yaw += anglePerSecond * frameDuration;
            if (keyboardState.IsKeyDown(Keys.Left)) yaw -= anglePerSecond * frameDuration;
            if (pitch > 89.0f) pitch = 89.0f;
            if (pitch < -89.0f) pitch = -89.0f;
            Vector3 direction;
            direction.X = (float)(Math.Cos(0.0174533 * yaw) * Math.Cos(0.0174533 * pitch));
            direction.Y = (float)Math.Sin(0.0174533 * pitch);
            direction.Z = (float)(Math.Sin(0.0174533 * yaw) * Math.Cos(0.0174533 * pitch));
            forward = direction;
        }
        public void ProcessMovementInput()
        {

            if (keyboardState.IsKeyDown(Keys.W)) cameraPos += movementPerSecond * forward * frameDuration;
            if (keyboardState.IsKeyDown(Keys.S)) cameraPos -= movementPerSecond * forward * frameDuration;
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 right = Vector3.Cross(forward, up);
                right.Normalize();
                cameraPos += right * movementPerSecond * frameDuration;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 right = Vector3.Cross(forward, up);
                right.Normalize();
                cameraPos -= right * movementPerSecond * frameDuration;
            }
            if (keyboardState.IsKeyDown(Keys.Space)) cameraPos += movementPerSecond * up * frameDuration;
            if (keyboardState.IsKeyDown(Keys.LeftShift)) cameraPos -= movementPerSecond * up * frameDuration;
        }
    }
}