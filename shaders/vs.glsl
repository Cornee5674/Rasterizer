#version 330

// Takes in the vector of the vertex, normal and uv coordinate.
in vec3 vPosition;
in vec3 vNormal;
in vec2 vUV;

// We also take in 2 matrices, one to convert to screen space, one to convert to world space.
uniform mat4 objectToScreen;
uniform mat4 objectToWorld;

// These pass onto the fragment shader.
out vec4 position;
out vec4 normal;
out vec2 uv;

void main()
{
	// First we calculate the coordinates on the screen, and then we calculate the world space coordinates for the remaining variables.
	gl_Position = objectToScreen * vec4(vPosition, 1.0);
	position = objectToWorld * vec4(vPosition, 1.0);
	normal = objectToWorld * vec4(vNormal, 0.0);
	uv = vUV;
}