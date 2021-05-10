using System;
using System.IO;
using System.Linq;
using System.Numerics;
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
    public struct TextureComponent
    {
        public IntPtr TexturePtr { get; private set; }

        public Texture Texture
        {
            set
            {
                TexturePtr = value.CPointer;
                TextureRect = new(default, new((int) value.Size.X, (int) value.Size.Y));
                Scale = Vector2.One;
            }
        }

        private IntRect _textureRect;

        public IntRect TextureRect
        {
            get => _textureRect;
            set => _textureRect = value;
        }

        public static Color Color => Color.White;

        public Vector2 Scale;

        public Vector2 Origin => new (_textureRect.Width/2f, _textureRect.Height/2f);

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