using System.Numerics;
using EngineNet.Components;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Shader = EngineNet.Components.Shader;
using Texture = EngineNet.Components.Texture;

namespace EngineNet;

using Shaders_Shader = Shader;
using Textures_Texture = Texture;

class Program
{
    private static IWindow _window;
    private static GL _gl;
    private static IInputContext _input;
    private static Shaders_Shader _shader;
    private static Textures_Texture _texture;

    private static Camera _camera;
    private static Vector2 _lastMousePos;

    private static int _width = 800;
    private static int _height = 600;

    private static uint _vbo;
    private static uint _vao;

    private static readonly float[] _vertices =
    [
        // POSIÇÃO (X, Y, Z)   // TEXTURE (U, V)   // NORMAL (NX, NY, NZ)

        // Face Frontal (Aponta para Z Positivo: 0, 0, 1)
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f,

        // Face Traseira (Aponta para Z Negativo: 0, 0, -1)
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, -1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,

        0.5f, 0.5f, -0.5f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f,

        // Face Esquerda (Aponta para X Negativo: -1, 0, 0)
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 1.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,

        // Face Direita (Aponta para X Positivo: 1, 0, 0)
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f,

        0.5f, 0.5f, -0.5f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,

        // Face Superior (Aponta para Y Positivo: 0, 1, 0)
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,

        0.5f, 0.5f, -0.5f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f,

        // Face Inferior (Aponta para Y Negativo: 0, -1, 0)
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, -1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 1.0f, 0.0f, -1.0f, 0.0f,

        0.5f, -0.5f, 0.5f, 1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f
    ];

    static void Main()
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(_height, _width);
        options.Title = "Game Test";

        Silk.NET.Windowing.Glfw.GlfwWindowing.RegisterPlatform();

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Closing += OnClose;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Resize += OnResize;

        _window.Run();
    }

    private static void OnResize(Vector2D<int> size)
    {
        _width = size.X;
        _height = size.Y;

        _gl.Viewport(0, 0, (uint)_width, (uint)_height);
    }

    private static void OnLoad()
    {
        _gl = _window.CreateOpenGL();

        _input = _window.CreateInput();
        _camera = new Camera(new Vector3(0, 0, 3f));
        _shader = new Shaders_Shader(_gl, "shader.vert", "shader.frag");
        _texture = new Textures_Texture(_gl, "image.png");

        IMouse mouse = _input.Mice[0];
        mouse.Cursor.CursorMode = CursorMode.Raw;
        mouse.MouseMove += OnMouseMove;

        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);

        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        foreach (var keyboard in _input.Keyboards)
        {
            keyboard.KeyDown += (_, key, _) =>
            {
                if (key == Key.Escape) _window.Close();
            };
        }

        unsafe
        {
            fixed (float* buf = _vertices)
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(_vertices.Length * sizeof(float)), buf,
                    BufferUsageARB.StaticDraw);
            }
        }

        unsafe
        {
            uint stride = 8 * sizeof(float);

            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            _gl.EnableVertexAttribArray(0);

            _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            _gl.EnableVertexAttribArray(1);
            
            _gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
            _gl.EnableVertexAttribArray(2);
        }


        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.Enable(EnableCap.DepthTest);
        _gl.BindVertexArray(0);
    }

    private static void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if (_lastMousePos == default)
        {
            _lastMousePos = position;
            return;
        }

        float xOffset = position.X - _lastMousePos.X;
        float yOffset = _lastMousePos.Y - position.Y;
        _lastMousePos = position;

        _camera.Look(xOffset, yOffset);
    }

    private static void OnUpdate(double deltaTime)
    {
        IKeyboard keyboard = _input.Keyboards[0];
        float dt = (float)deltaTime;

        Vector3 moveDir = Vector3.Zero;

        if (keyboard.IsKeyPressed(Key.W)) moveDir.Z += 1;
        if (keyboard.IsKeyPressed(Key.S)) moveDir.Z -= 1;
        if (keyboard.IsKeyPressed(Key.A)) moveDir.X -= 1;
        if (keyboard.IsKeyPressed(Key.D)) moveDir.X += 1;
        if (keyboard.IsKeyPressed(Key.Space)) moveDir.Y += 1;
        if (keyboard.IsKeyPressed(Key.ShiftLeft)) moveDir.Y -= 1;

        _camera.Move(moveDir, dt);
    }

    private static void OnRender(double deltaTime)
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _shader.Use();

        Vector3 lightPos = new Vector3(2f, 2f, 2f);
        Vector3 lightColor = new Vector3(1f, 1f, 1f);
        
        _shader.SetUniform("uLightPos", lightPos);
        _shader.SetUniform("uLightColor", lightColor);
        
        _texture.Bind();
        _shader.SetUniform("uTexture", 0);

        float angle = (float)_window.Time;

        Matrix4x4 rotation = Matrix4x4.CreateRotationX(angle) * Matrix4x4.CreateRotationY(angle);
        Matrix4x4 view = _camera.GetViewMatrix();
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            (float)(Math.PI / 4.0),
            (float)_width / _height,
            0.1f,
            100.0f);

        int modelLoc = _gl.GetUniformLocation(_shader.handle, "uModel");
        int viewLoc = _gl.GetUniformLocation(_shader.handle, "uView");
        int projectionLoc = _gl.GetUniformLocation(_shader.handle, "uProjection");

        unsafe
        {
            _gl.UniformMatrix4(modelLoc, 1, false, (float*)&rotation);
            _gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
            _gl.UniformMatrix4(projectionLoc, 1, false, (float*)&projection);
        }

        _gl.BindVertexArray(_vao);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    private static void OnClose()
    {
    }
}