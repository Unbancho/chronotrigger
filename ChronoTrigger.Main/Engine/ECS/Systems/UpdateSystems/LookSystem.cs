using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.Movement;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(LookComponent))]
    [Include(typeof(TransformComponent))]
    [Include(typeof(AnimationComponent))]
    public sealed class LookSystem : UpdateEntitySystem
    {
        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            var target = entity.Get<LookComponent>().Target;
            if (target.IsNullEntity()) return;
            var positionDifference = entity.Get<TransformComponent>().Position -
                                     target.Get<TransformComponent>().Position;
            MakeEntityLookAtTarget(positionDifference, ref entity.Get<AnimationComponent>());
        }

        private static void MakeEntityLookAtTarget(Vector2 positionDifference,
            ref AnimationComponent animationComponent)
        {
            animationComponent.Direction = (-positionDifference).ToDirection();
        }
    }
}