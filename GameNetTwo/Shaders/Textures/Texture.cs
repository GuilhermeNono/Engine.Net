using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace GameNetTwo.Shaders.Textures;

public class Texture : IDisposable
{
    private readonly GL _gl;
    private readonly uint _handle;

    public Texture(GL gl, string path)
    {
        _gl = gl;
        _handle = _gl.GenTexture();
        gl.BindTexture(TextureTarget.Texture2D, _handle);
        
        //Carregar imagem
        using var img = Image.Load<Rgba32>(AssemblyHelper.GetStreamFromResource($"GameNetTwo.Shaders.Textures.Unit.{path}"));

        //Tip: O OpenGL le de cima para baixo. E necessario inverter
        img.Mutate(x => x.Flip(FlipMode.Vertical));

        // 1. Criar um buffer para armazenar todos os pixels da imagem
        // (Largura * Altura * 4 bytes por pixel RGBA)
        byte[] pixelData = new  byte[img.Width * img.Height * 4];

        img.CopyPixelDataTo(pixelData);
        
        unsafe
        {
            fixed(void* data = pixelData)
                gl.TexImage2D(TextureTarget.Texture2D,
                    0, InternalFormat.Rgba,
                    (uint)img.Width,
                    (uint)img.Height,
                    0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    data);
        }

        // Configurações de repetição e filtragem (Essencial para não ver pixels borrados ou pretos)
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.Repeat);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.Repeat);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);

        gl.GenerateMipmap(GLEnum.Texture2D);
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        _gl.ActiveTexture(unit);
        _gl.BindTexture(GLEnum.Texture2D, _handle);
    }

    public void Dispose() => _gl.DeleteTexture(_handle);
}