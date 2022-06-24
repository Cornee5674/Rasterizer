using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Template
{
    // mesh and loader based on work by JTalton; http://www.opentk.com/node/642

    public class Mesh
    {
        // data members
        public ObjVertex[]? vertices;            // vertex positions, model space
        public ObjTriangle[]? triangles;         // triangles (3 vertex indices)
        public ObjQuad[]? quads;                 // quads (4 vertex indices)
        int vertexBufferId;                     // vertex buffer
        int triangleBufferId;                   // triangle buffer
        int quadBufferId;                       // quad buffer (not in Modern OpenGL)

        public Matrix4 localTransform = Matrix4.Identity;
        public Texture localTexture;
        public Shader localShader;

        public Vector3 specularCoefficient;
        public int n;

        // constructor
        public Mesh(string fileName, Texture localTexture, Shader localShader, Vector3 specularCoefficient, int n )
        {
            MeshLoader loader = new();
            loader.Load(this, fileName);
            this.localTexture = localTexture;
            this.localShader = localShader;
            this.specularCoefficient = specularCoefficient;
            this.n = n;
        }

        public void TransformObject(Matrix4 translation)
        {
            localTransform *= translation;
        }


        // initialization; called during first render
        public void Prepare()
        {
            if (vertexBufferId == 0 && vertices != null && triangles != null && quads != null)
            {
                // generate interleaved vertex data (uv/normal/position (total 8 floats) per vertex)
                GL.GenBuffers(1, out vertexBufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf(typeof(ObjVertex))), vertices, BufferUsageHint.StaticDraw);

                // generate triangle index array
                GL.GenBuffers(1, out triangleBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf(typeof(ObjTriangle))), triangles, BufferUsageHint.StaticDraw);

                if (OpenTKApp.allowPrehistoricOpenGL)
                {
                    // generate quad index array
                    GL.GenBuffers(1, out quadBufferId);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf(typeof(ObjQuad))), quads, BufferUsageHint.StaticDraw);
                }
            }
        }

        // render the mesh using the supplied shader and matrix
        public void Render(Shader shader, List<Vector3> lightPositions, List<Vector3> lightColors, Matrix4 objectToScreen, Matrix4 objectToWorld, Texture texture, Vector3 cameraPosition, Vector3 specular, int n)
        {
            // on first run, prepare buffers
            Prepare();

            // enable shader
            GL.UseProgram(shader.programID);

            // enable texture
            int texLoc = GL.GetUniformLocation(shader.programID, "pixels");
            GL.Uniform1(texLoc, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.id);
            // pass transform to vertex shader
            GL.UniformMatrix4(shader.uniform_mview, false, ref objectToScreen);
            GL.UniformMatrix4(shader.uniform_wview, false, ref objectToWorld);
            GL.Uniform3(shader.uniform_cameraPos, cameraPosition);
            GL.Uniform3(shader.uniform_specular, specular);
            GL.Uniform1(shader.uniform_n, n);

            float[] lightPositionsFloat = new float[lightPositions.Count * 3];
            float[] lightColorsFloat = new float[lightColors.Count * 3];
            for (int i = 0; i < lightPositions.Count; i++)
            {
                // 0, 1, 2, 1, 2, 3, 2, 3, 4
                lightPositionsFloat[i + (i * 2)] = lightPositions[i].X; //0 + 0 : 1 + 3  : 2 + 6
                lightPositionsFloat[i + 1 + (i * 2)] = lightPositions[i].Y; // 0+1 + 0 : 2 + 3
                lightPositionsFloat[i + 2 + (i * 2)] = lightPositions[i].Z;// 0 + 2 + 0: 3 + 3
                lightColorsFloat[i + (i * 2)] = lightColors[i].X;
                lightColorsFloat[i + 1 + (i * 2)] = lightColors[i].Y;
                lightColorsFloat[i + 2 + (i * 2)] = lightColors[i].Z;
            } 
            

            GL.Uniform3(shader.uniform_lightArrayPos, lightPositions.Count, lightPositionsFloat);
            GL.Uniform3(shader.uniform_lightArrayCol, lightColors.Count, lightColorsFloat);
            GL.Uniform1(shader.amountOfLights, lightPositions.Count);

            // enable position, normal and uv attributes
            GL.EnableVertexAttribArray(shader.attribute_vuvs);
            GL.EnableVertexAttribArray(shader.attribute_vnrm);
            GL.EnableVertexAttribArray(shader.attribute_vpos);

            // bind vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);

            // link vertex attributes to shader parameters 
            GL.VertexAttribPointer(shader.attribute_vpos, 3, VertexAttribPointerType.Float, false, 32, 5 * 4);
            GL.VertexAttribPointer(shader.attribute_vnrm, 3, VertexAttribPointerType.Float, true, 32, 2 * 4);
            GL.VertexAttribPointer(shader.attribute_vuvs, 2, VertexAttribPointerType.Float, false, 32, 0);

            // bind triangle index data and render
            if (triangles != null && triangles.Length > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
                GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Length * 3);
            }

            // bind quad index data and render
            if (quads != null && quads.Length > 0)
            {
                if (OpenTKApp.allowPrehistoricOpenGL)
                {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                    GL.DrawArrays(PrimitiveType.Quads, 0, quads.Length * 4);
                }
                else throw new Exception("Quads not supported in Modern OpenGL");
            }

            // restore previous OpenGL state
            GL.UseProgram(0);
        }

        // layout of a single vertex
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjVertex
        {
            public Vector2 TexCoord;
            public Vector3 Normal;
            public Vector3 Vertex;
        }

        // layout of a single triangle
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjTriangle
        {
            public int Index0, Index1, Index2;
        }

        // layout of a single quad
        [StructLayout(LayoutKind.Sequential)]
        public struct ObjQuad
        {
            public int Index0, Index1, Index2, Index3;
        }
    }
}