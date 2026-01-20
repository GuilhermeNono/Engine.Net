using System.Numerics;
using System.Runtime.InteropServices;
using EngineNet.Structs;
using Silk.NET.OpenGL;

namespace EngineNet.Components;

public class Mesh : IDisposable
{
    private uint _vao, _vbo, _ebo;
    private readonly GL _gl;
    private int _indicesCount;

    public Mesh(GL gl, List<Vertex> vertices, List<uint> indices)
    {
        _gl = gl;
        _indicesCount = indices.Count;
        SetupMesh(vertices, indices);
    }

    private unsafe void SetupMesh(List<Vertex> vertices, List<uint> indices)
    {
        uint stride = (uint)sizeof(Vertex);

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();
        _ebo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        fixed (void* v = CollectionsMarshal.AsSpan(vertices))
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Count * stride), v,
                BufferUsageARB.StaticDraw);
        }

        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        fixed (void* i = CollectionsMarshal.AsSpan(indices))
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Count * sizeof(uint)), i,
                BufferUsageARB.StaticDraw);
        }

        //1. Position
        _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, (void*)0);
        _gl.EnableVertexAttribArray(0);

        //2. TexCoords
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, (void*)Marshal.OffsetOf<Vertex>("TexCoord"));
        _gl.EnableVertexAttribArray(1);

        //3. Normals
        _gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, (void*)Marshal.OffsetOf<Vertex>("Normal"));
        _gl.EnableVertexAttribArray(2);

        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindVertexArray(0);
    }

    public void Draw(Shader shader)
    {
        _gl.BindVertexArray(_vao);

        unsafe
        {
            _gl.DrawElements(PrimitiveType.Triangles, (uint)_indicesCount, DrawElementsType.UnsignedInt, null);
        }

        _gl.BindVertexArray(0);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_vbo);
        _gl.DeleteBuffer(_ebo);
        _gl.DeleteVertexArray(_vao);
    }
}