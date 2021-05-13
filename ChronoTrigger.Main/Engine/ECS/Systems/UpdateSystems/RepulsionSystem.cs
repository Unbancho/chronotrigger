using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.Movement;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    // Sliding effect: Colliding(left, right) || i >= -1
    // TODO: Implement sliding off of edges effect.
    [UpdateSystem]
    [Include(typeof(RepulsionComponent))]
    public sealed class RepulsionSystem2 : EventListenerSystem<CollisionEvent>, ISystem<GameLoop.GameState>
    {
        public override bool ValidEvent(in CollisionEvent e)
        {
            return base.ValidEvent(in e) && (Ecs.GetEntityArchetype(e.Target) & Ecs.GetSignature<MovementComponent>()) != 0;
        }

        public void Run(GameLoop.GameState gameState)
        {
            var count = Events.Count;
            for (var index = 0; index < count; index++)
            {
                if (!Events.TryPop(out var collisionEvent)) continue;
                var repulse = collisionEvent.Sender.Get<RepulsionComponent>().Force;
                ref var transform = ref collisionEvent.Target.Get<TransformComponent>();
                var origTransform = transform;
                var colliderPosition = collisionEvent.TargetPackage.Rect.Position;
                ref var movement = ref collisionEvent.Target.Get<MovementComponent>();
                if(collisionEvent.SenderPackage.Rect.Rotation == 0 && collisionEvent.TargetPackage.Rect.Rotation == 0)
                {
                    var overlap = collisionEvent.Overlap;
                    var horizontal = overlap.Height > overlap.Width;
                    if (horizontal)
                    {
                        transform.Position.X +=
                            (repulse.X + overlap.Width) * (overlap.Left == colliderPosition.X ? 1 : -1);
                    }
                    else
                    {
                        transform.Position.Y +=
                            (repulse.Y + overlap.Height) * (overlap.Top == colliderPosition.Y ? 1 : -1);
                    }
                }
                else
                {
                    //TODO: Make this work better.
                    if (movement.Velocity.X != 0 && movement.Velocity.Y != 0)
                    {
                        transform.Position += -movement.Velocity;
                        continue;
                    }
                    var speed = movement.Speed;
                    var direction = movement.Velocity.ToDirection();
                    transform.Position += direction switch
                    {
                        Direction.Right => speed * Vector2.UnitY,
                        Direction.Up => speed * -Vector2.UnitX,
                        Direction.Left => speed * -Vector2.UnitY,
                        Direction.Down => speed * Vector2.UnitX
                    };
                }

                if(origTransform.Position != transform.Position && movement.Speed >= 2 * gameState.DeltaTime 
                   && (movement.Velocity.X == 0 || movement.Velocity.Y == 0))
                //TODO: Fix this not letting you turn towards solid when sprinting against it.
                    movement.Velocity *= 0;
            }
        }
    }
}