using System.Numerics;
using System.Runtime.InteropServices;

namespace EngineNet.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public Vector3 Position;
    public Vector2 TexCoord;
    public Vector3 Normal;
}