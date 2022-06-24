#version 330
 
#define MAX_LIGHTS 128;

in vec4 position;
in vec4 normal;
in vec2 uv;
uniform vec3 specular;
uniform int n;
uniform vec3 cameraPosition;
uniform sampler2D pixels;

uniform int amountOfLights;
uniform vec3[128] lightArrayPos;
uniform vec3[128] lightArrayCol;

vec4 calcColor(vec3 lightPos, vec3 lightCol, vec3 diffuseCol);

out vec4 color;

void main()
{
	vec3 diffuseColor = texture(pixels, uv).rgb;
	color = vec4(0, 0, 0, 0);
	for (int i = 0; i < amountOfLights; i++){
		color += calcColor(lightArrayPos[i], lightArrayCol[i], diffuseColor);
	}
	
	color += vec4(diffuseColor * 0.15, 1.0);
}

vec4 calcColor(vec3 lightPos, vec3 lightCol, vec3 diffuseCol){
	vec3 L = lightPos - position.xyz;
	float attenuation = 1.0 / dot(L, L);
	float NdotL = max(0, dot(normalize(normal.xyz), normalize(L)));

	vec3 R = L - 2 * dot(L, normal.xyz) * normal.xyz;
	vec3 V = cameraPosition - position.xyz;
	float VdotR = max(0, dot(normalize(V.xyz), normalize(R)));
	float VdotRpow = pow(VdotR, n);


	return vec4(attenuation * lightCol * (diffuseCol * NdotL + specular * VdotRpow), 1.0);
}