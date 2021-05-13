using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ResourceManagement;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(SpriteComponent))]
    [Include(typeof(AnimationComponent))]
    public sealed class AnimationSystem : UpdateEntitySystem<GameLoop.GameState>
    {
        private static void SetSprite(ref SpriteComponent drawable, ref AnimationComponent animated)
        {
            var animationStruct = ResourceManager<long, AnimationStruct>.Get(animated.AnimationKey);
            if (animationStruct.Frames == null) return;
            animated.CurrentIndex += animated.AnimationSpeed / 1000;
            animated.CurrentIndex %= animationStruct.Frames.Length;
            drawable.TextureRect = animationStruct.Frames[(int) animated.CurrentIndex];
            drawable.Scale = animationStruct.Mirrored ? new (-1, 1) : Vector2.One;
        }

        public override void ActOnEntity(Entity entity, GameLoop.GameState gameState)
        {
            SetSprite(ref entity.Get<SpriteComponent>(), ref entity.Get<AnimationComponent>());
        }
    }
}