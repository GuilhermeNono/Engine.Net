using System.Numerics;

namespace EngineNet.Components;

public class Camera
{
    // Vetores de Estado
    public Vector3 Position { get; set; }
    public Vector3 Front { get; set; }
    public Vector3 Up { get; set; }
    public Vector3 Right { get; set; }

    //Rotacao (Euler Angles)
    private float _yaw = -90f;
    private float _pitch = 0f;

    //Configuracao
    public float Speed { get; set; } = 2.5f;
    public float Sensitivity { get; set; } = 0.1f;

    public Camera(Vector3 position)
    {
        Position = position;
        UpdateVectors();
    }

    //Matriz View para o Shder
    public Matrix4x4 GetViewMatrix()
    {
        // LookAt (Onde estou; Para onde estou olhando; Onde e cima)
        return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
    }

    // Processamento Input
    public void Move(Vector3 direction, float deltaTime)
    {
        float velocity = Speed * deltaTime;
        
        // Se mover para frente; Soma vetor Front
        if(direction.Z > 0) Position += Front * velocity;
        if(direction.Z < 0) Position -= Front * velocity;
        
        if(direction.X > 0) Position += Right * velocity;
        if(direction.X < 0) Position -= Right * velocity;
        
        // Voo livre
        if(direction.Y > 0) Position += Up * velocity;
        if(direction.Y < 0) Position -= Up * velocity;
    }

    public void Look(float xOffset, float yOffset)
    {
        _yaw += xOffset * Sensitivity;
        _pitch += yOffset * Sensitivity;
        
        if(_pitch > 89.0f) _pitch = 89.0f;
        if(_pitch < -89.0f) _pitch = -89.0f;
        
        UpdateVectors();
    }
    
    // Converte angulos eem vetor direcao
    private void UpdateVectors()
    {
        Vector3 front;
        front.X = MathF.Cos((MathF.PI / 180) * _yaw) * MathF.Cos((MathF.PI / 180) * _pitch);
        front.Y = MathF.Sin((MathF.PI / 180) * _pitch);
        front.Z = MathF.Sin((MathF.PI / 180) * _yaw) * MathF.Cos((MathF.PI / 180) * _pitch);

        Front = Vector3.Normalize(front);

        // Recalcula a direita e cima relativos
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}