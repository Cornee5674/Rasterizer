#version 330
 
in vec4 position;
in vec4 normal;
in vec2 uv;
uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform sampler2D pixels;

out vec4 color;

void main()
{
	vec3 L = lightPosition - position.xyz;
	float attenuation = 1.0 / dot(L, L);
	float NdotL = max(0, dot(normalize(normal.xyz), normalize(L)));
	vec3 diffuseColor = texture(pixels, uv).rgb;
	color = vec4(lightColor * diffuseColor * attenuation * NdotL, 1.0);
	color += vec4(diffuseColor * 0.15, 1.0);
}