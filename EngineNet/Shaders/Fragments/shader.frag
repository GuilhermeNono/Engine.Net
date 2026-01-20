#version 330 core

out vec4 FragColor;

in vec2 fTexCoords;
in vec3 fNormal;
in vec3 fFragPos;

uniform sampler2D uTexture;
uniform vec3 uLightPos;
uniform vec3 uLightColor;
uniform vec3 uViewPos;

void main(){
    // 1. Base texture color
    vec4 texColor = texture(uTexture, fTexCoords);

    // 2. Material settings
    float specularStrength = 0.5;
    float shininess = 32.0;

    // ------------------------------------
    // LIGHT CALCULATIONS
    // ------------------------------------

    // A. Ambient
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * uLightColor;

    // B. Diffuse
    vec3 norm = normalize(fNormal);
    vec3 lightDir = normalize(uLightPos - fFragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * uLightColor;

    // C. Specular
    vec3 viewDir = normalize(uViewPos - fFragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    vec3 specular = specularStrength * spec * uLightColor;

    // ------------------------------------
    // FINAL RESULT
    // ------------------------------------

    vec3 result = (ambient + diffuse + specular) * texColor.rgb;
    FragColor = vec4(result, texColor.a);
}