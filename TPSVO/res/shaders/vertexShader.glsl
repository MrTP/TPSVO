#version 430
in vec2 pos;  
out vec2 texCoord;  
void main() {  
    texCoord = pos;  
    gl_Position = vec4(pos.x * 2.0 - 1.0, pos.y * 2.0 - 1.0, 0.0, 1.0);
};