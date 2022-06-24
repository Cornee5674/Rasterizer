#version 330
 
in vec2 uv;
uniform sampler2D pixels;

out vec4 color;

void main()
{
	// A static color just returns the color on the texture corresponding to the uv coordinates.
	color = vec4(texture(pixels, uv).rgb, 1.0);
}