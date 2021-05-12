using System;
using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.Movement;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(AnimationComponent))]
    [Include(typeof(TransformComponent))]
    [Include(typeof(MovementComponent))]
    public sealed class MovementAnimationSystem : UpdateEntitySystem<GameLoop.GameState>
    {
        public float IdleSpeedLimit { get; set; } = 1 / 100f;
        public float AnimationSpeedConstant { get; set; } = 70f;

        private void UpdateAnimation(ref AnimationComponent animationComponent,
            MovementComponent movementComponent, float deltaTime)
        {
            var speed = movementComponent.Speed / deltaTime;
            animationComponent.AnimationType = ToAnimationType(speed, animationComponent.AnimationType);
            animationComponent.AnimationSpeed = AnimationSpeedConstant * deltaTime;
            if(movementComponent.Velocity != Vector2.Zero)
                animationComponent.Direction = movementComponent.Velocity.ToDirection();
        }

        private AnimationType ToAnimationType(float speed, AnimationType currentAnimation)
        {
            if (Math.Abs(speed) < IdleSpeedLimit) return AnimationType.Idle;
            var jogging = speed > 1.25f && currentAnimation == AnimationType.Running; // To fix glitching out.
            return speed < 2f && !jogging ? AnimationType.Walking : AnimationType.Running;
        }

        public override void ActOnEntity(Entity entity, GameLoop.GameState gameState)
        {
            UpdateAnimation(ref entity.Get<AnimationComponent>(),
                entity.Get<MovementComponent>(), gameState.DeltaTime);
        }
    }
}