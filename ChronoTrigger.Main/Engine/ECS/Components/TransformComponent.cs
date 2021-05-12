using System.Numerics;
using ChronoTrigger.Engine.ECS.Systems.DrawSystems;
using ModusOperandi.ECS.Components;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ECS.Components
{
    public interface ITransformableComponent
    {
        Vector2 TransformPosition { get; set; }
    }

    [Component]
    public struct TransformComponent : IDebugDrawableComponent
    {
        public Vector2 Position;

        public Drawable DebugDrawable => new Sprite(null, new(0, 0,
            5, 5))
        {
            Position = Position.ToVector2f(),
            Color = new(0, 0, 255, 255)
        };
    }
}