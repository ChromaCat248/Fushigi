﻿using Silk.NET.Core.Native;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fushigi.gl
{
    public class GLTexture2D : GLTexture
    {
        public GLTexture2D(GL gl) : base(gl)
        {
            Target = TextureTarget.Texture2D;
        }

        public static GLTexture2D CreateUncompressedTexture(GL gl, uint width, uint height, InternalFormat format,
             Silk.NET.OpenGL.PixelFormat pixelFormat = Silk.NET.OpenGL.PixelFormat.Rgba,
              Silk.NET.OpenGL.PixelType pixelType = Silk.NET.OpenGL.PixelType.UnsignedByte)
        {
            GLTexture2D tex = new GLTexture2D(gl);
            tex.Width = width;
            tex.Height = height;
            tex.InternalFormat = format;
            tex.PixelFormat = pixelFormat;
            tex.PixelType = pixelType;

            tex.Bind();

            unsafe
            {
                gl.TexImage2D(tex.Target, 0, tex.InternalFormat, tex.Width, tex.Height, 0,
                                  tex.PixelFormat, tex.PixelType, null);
            }

            tex.Unbind();

            return tex;
        }

        public static GLTexture2D Load(GL gl, string filePath)
        {
            GLTexture2D tex = new GLTexture2D(gl);
            tex.Load(filePath);
            return tex;
        }

        public static GLTexture2D Load(GL gl, int width, int height, byte[] rgba)
        {
            GLTexture2D tex = new GLTexture2D(gl);
            tex.Load(width, height, rgba);
            return tex;
        }

        public void Load(string filePath) => Load(File.ReadAllBytes(filePath));

        public void Load(byte[] imageFile)
        {
            ImageResult image = ImageResult.FromMemory(imageFile, ColorComponents.RedGreenBlueAlpha);

            this.Width = (uint)image.Width;
            this.Height = (uint)image.Height;

            this.InternalFormat = InternalFormat.Rgba;
            this.PixelFormat = Silk.NET.OpenGL.PixelFormat.Rgba;
            this.PixelType = Silk.NET.OpenGL.PixelType.UnsignedByte;

            LoadImage(image.Data);
        }

        public void Load(int width, int height, byte[] rgba)
        {
            this.Width = (uint)width;
            this.Height = (uint)height;

            this.InternalFormat = InternalFormat.Rgba;
            this.PixelFormat = Silk.NET.OpenGL.PixelFormat.Rgba;
            this.PixelType = Silk.NET.OpenGL.PixelType.UnsignedByte;

            LoadImage(rgba);
        }

        public unsafe void LoadImage(byte[] image)
        {
            Bind();

            fixed (byte* ptr = image)
            {
                _gl.TexImage2D(Target, 0, InternalFormat, Width, Height, 0,
                    PixelFormat, PixelType, ptr);
            }

            _gl.TextureParameter(ID, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            _gl.TextureParameter(ID, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            _gl.TextureParameter(ID, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            _gl.TextureParameter(ID, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            _gl.GenerateMipmap(Target);

            Unbind();
        }

        public unsafe void Resize(uint width, uint height)
        {
            Width = width;
            Height = height;

            Bind();

            _gl.TexImage2D(Target, 0, InternalFormat, Width, Height, 0,
                         PixelFormat, PixelType, null);

            Unbind();
        }

        public void GenerateMipmaps()
        {
            Bind();
            _gl.GenerateMipmap(Target);
        }
    }
}
