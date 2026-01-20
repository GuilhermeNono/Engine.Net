#version 330 core

out vec4 FragColor;

in vec2 fTexCoords;
in vec3 fNormal;
in vec3 fFragPos;

uniform sampler2D uTexture;
uniform vec3 uLightPos;
uniform vec3 uLightColor;

void main(){
    FragColor = texture(uTexture, fTexCoords);
    
    if (FragColor.a < 0.1) discard;

    float ambientStrength = 0.2;
    vec3 ambient = ambientStrength * uLightColor;
    
    vec3 norm = normalize(fNormal);
    
    vec3 lightDir = normalize(uLightPos - fFragPos);
    
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * uLightColor;
    
    vec3 result = (ambient + diffuse) * FragColor.rgb;
    
    FragColor = vec4(result, FragColor.a);
}