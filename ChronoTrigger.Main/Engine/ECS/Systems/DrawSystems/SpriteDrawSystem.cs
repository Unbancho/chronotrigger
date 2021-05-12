using System;
using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.Rendering;
using SFML.System;

namespace ChronoTrigger.Engine.ECS.Systems.DrawSystems
{
    [DrawSystem]
    public sealed class SpriteDrawSystem : DrawComponentSystem<MoveSpriteSystem.Sprite>
    {
        public override Span<MoveSpriteSystem.Sprite> Components => MoveSpriteSystem.SortedSpritesSafe;

        public SpriteDrawSystem()
        {
            ComplementarySystems.Add(new MoveSpriteSystem());
        }

        public override void DrawComponent(MoveSpriteSystem.Sprite component, SpriteBatch spriteBatch)
        {
            var texture = component.TextureComponent;
            var transform = component.TransformComponent;
            spriteBatch.Draw(texture.TexturePtr, transform.Position, texture.TextureRect, TextureComponent.Color,
                texture.Scale, texture.Origin);
        }
    }

    internal static class VectorExtensions
    {
        public static Vector2f ToVector2f(this Vector2 v)
        {
            return new(v.X, v.Y);
        }
    }
}