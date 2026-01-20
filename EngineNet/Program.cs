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
    private static Model _model;

    private static Camera _camera;
    private static Vector2 _lastMousePos;

    private static int _width = 1280;
    private static int _height = 720;

    static void Main()
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(_width, _height);
        options.Title = "Game Test";

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

        _model = new Model(_gl, "Models/character-b.obj");
        _input = _window.CreateInput();
        _camera = new Camera(new Vector3(0, 0, 3f));
        _shader = new Shaders_Shader(_gl, "shader.vert", "shader.frag");
        _texture = new Textures_Texture(_gl, "texture-b.png");

        IMouse mouse = _input.Mice[0];
        mouse.Cursor.CursorMode = CursorMode.Raw;
        mouse.MouseMove += OnMouseMove;

        foreach (var keyboard in _input.Keyboards)
        {
            keyboard.KeyDown += (_, key, _) =>
            {
                if (key == Key.Escape) _window.Close();
            };
        }

        _gl.Enable(EnableCap.DepthTest);
        _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
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

        Vector3 lightPos = new(2f, 2f, 2f);

        Vector3 lightColor = Rgb(250, 211, 125);
        
        float lightIntensity = 2f;
        
        _shader.SetUniform("uLightPos", lightPos);
        _shader.SetUniform("uLightColor", lightColor);
        _shader.SetUniform("uLightIntensity", lightIntensity);
        
        _shader.SetUniform("uTexture", 0);
        _shader.SetUniform("uViewPos", _camera.Position);
        
        _texture.Bind(TextureUnit.Texture0);

        Matrix4x4 rotation = Matrix4x4.Identity;
        Matrix4x4 view = _camera.GetViewMatrix();
        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(
            (float)(Math.PI / 4.0),
            (float)_width / _height,
            0.1f,
            100.0f);

        int modelLoc = _gl.GetUniformLocation(_shader.Handle, "uModel");
        int viewLoc = _gl.GetUniformLocation(_shader.Handle, "uView");
        int projectionLoc = _gl.GetUniformLocation(_shader.Handle, "uProjection");

        unsafe
        {
            _gl.UniformMatrix4(modelLoc, 1, false, (float*)&rotation);
            _gl.UniformMatrix4(viewLoc, 1, false, (float*)&view);
            _gl.UniformMatrix4(projectionLoc, 1, false, (float*)&projection);
        }

        _model.Draw(_shader);
    }

    private static void OnClose()
    {
        _model.Dispose();
        _shader.Dispose();
        _input.Dispose();
    }

    private static Vector3 Rgb(float r, float g, float b) => new (r / 255, g / 255, b / 255);
}