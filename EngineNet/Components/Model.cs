using System.Numerics;
using Assimp;
using EngineNet.Structs;
using Silk.NET.OpenGL;

namespace EngineNet.Components;

public class Model : IDisposable
{
    private readonly GL _gl;
    private readonly List<Mesh> _meshes = [];
    private string _directory;

    public Model(GL gl, string directory)
    {
        _gl = gl;
        LoadModel($"Models/{directory}");
    }

    private void LoadModel(string path)
    {
        AssimpContext importer = new ();
        
        Scene? scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.FlipUVs);

        if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            throw new Exception("Erro ao carregar modelo: " + path);

        _directory = Path.GetDirectoryName(path);
        ProcessNode(scene.RootNode, scene);
    }

    private void ProcessNode(Node node, Scene scene)
    {
        foreach (int meshIndex in node.MeshIndices)
        {
            var mesh = scene.Meshes[meshIndex];
            _meshes.Add(ProcessMesh(mesh, scene));
        }
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> indices = new List<uint>();

        for (int i = 0; i < mesh.VertexCount; i++)
        {
            Vertex vertex = new();
            
            // Position
            vertex.Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
            
            // Normais
            if(mesh.HasNormals) 
                vertex.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
            
            // Texture
            vertex.TexCoord = mesh.HasTextureCoords(0) ? new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y) : Vector2.Zero;
            
            vertices.Add(vertex);
        }

        foreach (Face face in mesh.Faces)
        {
            for (int i = 0; i < face.IndexCount; i++)
            {
                indices.Add((uint)face.Indices[i]);
            }
        }

        return new Mesh(_gl, vertices, indices);
    }

    public void Draw(Shader shader)
    {
        foreach (Mesh mesh in _meshes)
        {
            mesh.Draw(shader);
        }
    }

    public void Dispose()
    {
        foreach (Mesh mesh in _meshes) mesh.Dispose();
    }
}