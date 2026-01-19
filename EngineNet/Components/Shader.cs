using System.Numerics;
using System.Reflection;
using Silk.NET.OpenGL;

namespace EngineNet.Components;

public class Shader : IDisposable
{
    public uint handle;
    private GL _gl;

    private string GetShaderDataFromAssembly(ShaderType type, string path)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var shaderPath = type switch
        {
            ShaderType.Vertex => "EngineNet.Shaders.Vertex",
            ShaderType.Fragment => "EngineNet.Shaders.Fragments",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        using var stream = assembly.GetManifestResourceStream($"{shaderPath}.{path}")
                           ?? throw new Exception($"{type.ToString()} Shaders not found.");
        
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public Shader(GL gl, string vertexPath, string fragmentPath)
    {
        _gl = gl;
        
        string vertexShaderSource = GetShaderDataFromAssembly(ShaderType.Vertex, vertexPath);
        string fragmentShaderSource = GetShaderDataFromAssembly(ShaderType.Fragment, fragmentPath);

        uint vertex = CompileShader(Silk.NET.OpenGL.ShaderType.VertexShader, vertexShaderSource);
        uint fragment = CompileShader(Silk.NET.OpenGL.ShaderType.FragmentShader, fragmentShaderSource);
        
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

    private uint CompileShader(Silk.NET.OpenGL.ShaderType type, string code)
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
    
    public void SetUniform(string name, Vector2 value)
    {
        int location = _gl.GetUniformLocation(handle, name);
        if (location == -1) return;
        
        _gl.Uniform2(location, value);
    }
    
    public void SetUniform(string name, int value)
    {
        int location = _gl.GetUniformLocation(handle, name);
        if (location == -1) return;
        
        _gl.Uniform1(location, value);
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}