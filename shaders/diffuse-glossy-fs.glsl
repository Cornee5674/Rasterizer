#version 330
 
in vec4 position;
in vec4 normal;
in vec2 uv;
uniform vec3 specular;
uniform int n;
uniform vec3 cameraPosition;
uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform sampler2D pixels;

out vec4 color;

void main()
{
	vec3 L = lightPosition - position.xyz;
	vec3 R = L - 2 * dot(L, normal.xyz) * normal.xyz;
	vec3 V = position.xyz - cameraPosition;
	vec3 diffuseColor = texture(pixels, uv).rgb;
	float attenuation = 1.0 / dot(L, L);
	float NdotL = max(0, dot(normalize(normal.xyz), normalize(L)));
	float VdotR = max(0, dot(normalize(V.xyz), normalize(R)));
	float VdotRpow = pow(VdotR, n);
	color = vec4(attenuation * lightColor * (diffuseColor * NdotL + specular * VdotRpow), 1.0);
	color += vec4(diffuseColor * 0.15, 1.0);
}