#version 330
 
#define MAX_LIGHTS 128;
// The shader takes the position of the fragment, the normal and the uv coordinate. Position and normal in world space.
in vec4 position;
in vec4 normal;
in vec2 uv;

// Uniform variables for the lights
uniform int amountOfLights;
uniform vec3[128] lightArrayPos;
uniform vec3[128] lightArrayCol;

// The texture
uniform sampler2D pixels;

// Define the calcColor function
vec4 calcColor(vec3 lightPos, vec3 lightCol, vec3 diffuseCol);

//Output variable
out vec4 color;

void main()
{
	// We calculate the diffuseColor with the uv coordinates. Then we loop over all the lights and add the appropiate colors together. After that we also add some ambient lighting.
	vec3 diffuseColor = texture(pixels, uv).rgb;
	color = vec4(0, 0, 0, 0);
	for (int i = 0; i < amountOfLights; i++){
		color += calcColor(lightArrayPos[i], lightArrayCol[i], diffuseColor);
	}
	color += vec4(diffuseColor * 0.15, 1.0);
}

vec4 calcColor(vec3 lightPos, vec3 lightCol, vec3 diffuseCol){
	// We calculate the diffuse shading with the current lighting.
	vec3 L = lightPos - position.xyz;
	float attenuation = 1.0 / dot(L, L);
	float NdotL = max(0, dot(normalize(normal.xyz), normalize(L)));
	return vec4(lightCol * diffuseCol * attenuation * NdotL, 1.0);
}