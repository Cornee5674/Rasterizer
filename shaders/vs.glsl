#version 330

in vec3 vPosition;
in vec3 vNormal;
in vec2 vUV;
uniform mat4 objectToScreen;
uniform mat4 objectToWorld;

out vec4 position;
out vec4 normal;
out vec2 uv;

void main()
{
	gl_Position = objectToScreen * vec4(vPosition, 1.0);
	position = objectToWorld * vec4(vPosition, 1.0);
	normal = objectToWorld * vec4(vNormal, 0.0);
	uv = vUV;
}