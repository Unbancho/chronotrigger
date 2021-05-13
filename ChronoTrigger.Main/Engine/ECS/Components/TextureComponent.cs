using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using ChronoTrigger.Engine.ResourceManagement;
using JetBrains.Annotations;
using ModusOperandi.ECS.Components;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ECS.Components
{
    public interface IDebugDrawableComponent
    {
        Drawable DebugDrawable { get; }
    }

    public enum LayerEnum : sbyte
    {
        Background = -1,
        Sprite = 0,
        Foreground = 1
    }

    [Component]
    public struct SpriteComponent
    {
        private IntRect _textureRect;

        public IntRect TextureRect
        {
            get => _textureRect;
            set
            {
                _textureRect = value;
                Scale = Vector2.One;
            }
        }
        
        public Vector2 Scale;
        
        public Vector2 Origin => new (_textureRect.Width/2f, _textureRect.Height/2f);
    }

    [Component]
    [StructLayout(LayoutKind.Sequential, Size=9)]
    public struct TextureComponent
    {
        public IntPtr TexturePtr;

        public Texture Texture
        {
            set => TexturePtr = value.CPointer;
        }

        public static Color Color => Color.White;
        
        public LayerEnum Layer;

        [UsedImplicitly]
        public string SpriteSheet
        {
            set
            {
                var texture = ResourceManager.GetFile<Texture>(Directory.GetFiles($"{GameDirectories.SpritesDirectory}",
                    value, SearchOption.AllDirectories).First());
                Texture = texture;
            }
            get => "";
        }
    }
}