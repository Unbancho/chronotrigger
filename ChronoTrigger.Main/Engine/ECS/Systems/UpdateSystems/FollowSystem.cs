using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(FollowerComponent))]
    [Include(typeof(MovementComponent))]
    [Include(typeof(TransformComponent))]
    public sealed class FollowSystem : UpdateEntitySystem<GameLoop.GameState>
    {
        private static void SetupTransform(ref MovementComponent movementComponent,
            FollowerComponent followerComponent,
            float deltaTime, float distance, Vector2 direction)
        {
            var excessDistance = distance - followerComponent.DistanceToKeep;
            var speed = excessDistance / followerComponent.DistanceToKeep + 0.2f; //0.1f * excessDistance;
            if (movementComponent.Speed > 1.5f) speed += 0.5f;
            movementComponent.Velocity = direction * speed * deltaTime;
        }

        public override void ActOnEntity(Entity entity, GameLoop.GameState gameState)
        {
            ref var followerComponent = ref entity.Get<FollowerComponent>();
            var followerTarget = followerComponent.Target;
            if (followerTarget.IsNullEntity()) return;
            var targetPos = followerTarget.Get<TransformComponent>().Position;
            var entityPos = entity.Get<TransformComponent>().Position;
            var positionDifference = targetPos - entityPos;
            var distance = positionDifference.Length();
            if (distance > followerComponent.DistanceToKeep)
            {
                SetupTransform(ref entity.Get<MovementComponent>(),
                    followerComponent, gameState.DeltaTime, distance, Vector2.Normalize(positionDifference));
                return;
            }

            entity.Get<MovementComponent>().Velocity *= 0;
        }
    }
}