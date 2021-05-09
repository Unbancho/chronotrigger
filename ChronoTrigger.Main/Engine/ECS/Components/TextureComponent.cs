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

    public interface ITexturedComponent
    {
        public IntPtr TexturePtr { get; }
        public Texture Texture { set; }
        public IntRect TextureRect { get; set; }
        public Color Color { get; set; }
        public Vector2 Scale { get; set; }
        public Vector2 Origin { get; }
    }

    //TODO: Remove Position (do sorting right before drawing).
    [Component]
    public struct TextureComponent : ITexturedComponent
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

        public Color Color
        {
            get => Color.White;
            set { }
        }

        public Vector2 Scale { get; set; }

        public Vector2 Origin => new (_textureRect.Width/2f, _textureRect.Height/2f);

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

    [Component]
    public struct BackgroundTextureComponent : ITexturedComponent, ITransformableComponent
    {
        public IntPtr TexturePtr { get; private set; }

        public Texture Texture
        {
            set
            {
                TexturePtr = value.CPointer;
                TextureRect = new(default, new((int) value.Size.X, (int) value.Size.Y));
            }
        }

        private IntRect _textureRect;

        public IntRect TextureRect
        {
            get => _textureRect;
            set => _textureRect = value;
        }

        public Color Color
        {
            get => Color.White;
            set { }
        }

        public Vector2 Scale
        {
            get => Vector2.One;
            set { }
        }

        private Vector2 _origin;

        public Vector2 Origin
        {
            get => _origin;
            [UsedImplicitly]
            set => _origin = value + new Vector2(_textureRect.Width / 2f, _textureRect.Height / 2f);
        }

        [UsedImplicitly]
        public string SpriteSheet
        {
            set
            {
                var texture = ResourceManager.GetFile<Texture>(Directory.GetFiles($"{GameDirectories.SpritesDirectory}",
                    value, SearchOption.AllDirectories).First());
                Texture = texture;
                if(Origin == default) Origin = Vector2.Zero;
            }
            get => "";
        }

        public Vector2 TransformPosition { get; set; }
    }
}