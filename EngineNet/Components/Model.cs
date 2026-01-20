using System.Numerics;
using Assimp;
using EngineNet.Structs;
using Silk.NET.OpenGL;

namespace EngineNet.Components;

public class Model : IDisposable
{
    private GL _gl;
    private List<Mesh> _meshes = new List<Mesh>();
    private string _directory;

    public Model(GL gl, string path)
    {
        _gl = gl;
        LoadModel(path);
    }

    private void LoadModel(string path)
    {
        var importer = new AssimpContext();

        // Flags importantes:
        // Triangulate: Transforma quadrados/polígonos em triângulos (OpenGL só gosta de triângulos)
        // FlipUVs: Inverte a textura Y (essencial para OpenGL)
        // CalculateTangentSpace: Útil para normal mapping no futuro
        var scene = importer.ImportFile(path,
            PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateNormals);

        if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
        {
            throw new Exception("Erro ao carregar modelo Assimp: ");
        }

        _directory = Path.GetDirectoryName(path);
        ProcessNode(scene.RootNode, scene);
    }

    private void ProcessNode(Node node, Scene scene)
    {
        // Processa todas as meshes deste nó
        foreach (var meshIndex in node.MeshIndices)
        {
            var mesh = scene.Meshes[meshIndex];
            _meshes.Add(ProcessMesh(mesh, scene));
        }

        // Continua recursivamente para os filhos
        foreach (var child in node.Children)
        {
            ProcessNode(child, scene);
        }
    }

    private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
    {
        List<Vertex> vertices = new List<Vertex>();
        List<uint> indices = new List<uint>();

        // 1. Converter Vértices
        for (int i = 0; i < mesh.VertexCount; i++)
        {
            // Posição
            
            Vertex vertex = new Vertex
            {
                Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z)
            };

            // Normais
            if (mesh.HasNormals)
                vertex.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);

            // Textura (Checa se tem mapa de textura no slot 0)
            vertex.TexCoord = mesh.HasTextureCoords(0)
                ? new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y)
                : Vector2.Zero;

            vertices.Add(vertex);
        }

        // 2. Converter Índices (Faces)
        foreach (var face in mesh.Faces)
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
        foreach (var mesh in _meshes)
        {
            mesh.Draw(shader);
        }
    }

    public void Dispose()
    {
        foreach (var mesh in _meshes) mesh.Dispose();
    }
}