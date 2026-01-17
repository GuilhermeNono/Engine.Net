using System.Reflection;

namespace Engine.Net;

public static class AssemblyHelper
{
    public static string GetShaderDataFromAssembly(ShaderType type, string path)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var shaderPath = type switch
        {
            ShaderType.Vertex => "Engine.Net.Shaders.Vertex.",
            ShaderType.Fragment => "Engine.Net.Shaders.Fragments.",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        using var stream = assembly.GetManifestResourceStream($"{shaderPath}.{path}")
                           ?? throw new Exception($"{type.ToString()} Shaders not found.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static Stream GetStreamFromResource(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return assembly.GetManifestResourceStream(path)
                     ?? throw new Exception("Resource not found.");
    }

}