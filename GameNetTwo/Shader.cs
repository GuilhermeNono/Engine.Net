using System.Numerics;
using Silk.NET.OpenGL;

namespace GameNetTwo;

public class Shader : IDisposable
{
    
    public uint handle;
    private GL _gl;

    public Shader(GL gl, string vertexPath, string fragmentPath)
    {
        _gl = gl;
        
        string vertexShaderSource = File.ReadAllText($"Shaders/Vertex/{vertexPath}");
        string fragmentShaderSource = File.ReadAllText($"Shaders/Fragments/{fragmentPath}");

        uint vertex = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        uint fragment = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);
        
        handle = gl.CreateProgram();
        
        gl.AttachShader(handle, vertex);
        gl.AttachShader(handle, fragment);
        gl.LinkProgram(handle);

        gl.GetProgram(handle, ProgramPropertyARB.LinkStatus, out int linkStatus);
        if (linkStatus == 0)
            throw new Exception($"Erro ao linkar o shader: {gl.GetProgramInfoLog(handle)}");
        
        gl.DetachShader(handle, vertex);
        gl.DetachShader(handle, fragment);
        gl.DeleteShader(vertex);
        gl.DeleteShader(fragment);
    }

    private uint CompileShader(ShaderType type, string code)
    {
        uint shader = _gl.CreateShader(type);
        _gl.ShaderSource(shader, code);   
        _gl.CompileShader(shader);
        
        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
        return status == 0 ? throw new Exception($"Shader compile error {type}: {code}") : shader;
    }
    
    public void Use() => _gl.UseProgram(handle);

    public void SetUniform(string name, Vector4 value)
    {
        int location = _gl.GetUniformLocation(handle, name);
        if (location == -1) return;
        
        _gl.Uniform4(location, value);
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}