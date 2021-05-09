using System;
using System.Diagnostics.CodeAnalysis;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(MovementComponent))]
    [Include(typeof(TransformComponent))]
    public sealed class RepulsionSystem : EventListenerSystem<CollisionEvent>, IUpdateSystem
    {
        public void PreExecution() { }

        public void Execute(float deltaTime)
        {
            var count = Events.Count;
            for (var index = 0; index < count; index++)
            {
                if (!Events.TryPop(out var collisionEvent)) continue;
                var entityA = collisionEvent.Sender;
                var entityB = collisionEvent.Target;

                ref var collisionA = ref entityA.Get<CollisionComponent>();
                ref var collisionB = ref entityB.Get<CollisionComponent>();
                
                ref var transformA = ref entityA.Get<TransformComponent>();
                ref var movementA = ref entityA.Get<MovementComponent>();

                if (Repulse(ref collisionA, collisionB, ref transformA, deltaTime)
                    && movementA.Speed >= 2 * deltaTime
                    && (movementA.Velocity.X == 0 || movementA.Velocity.Y == 0))
                    //TODO: Fix this not letting you turn towards solid when sprinting against it.
                    movementA.Velocity *= 0;
            }
        }

        public void PostExecution() { }

        // Sliding effect: Colliding(left, right) || i >= -1
        // TODO: Implement sliding off of edges effect.
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private static bool Repulse(ref CollisionComponent left, CollisionComponent right, ref TransformComponent t,
            float deltaTime)
        {
            var p = t.Position;
            if (left.Rotation == 0 && right.Rotation == 0)
            {
                while (left.Hitbox.Intersects(right.Hitbox, out var overlap))
                {
                    var horizontal = overlap.Height > overlap.Width;
                    var orig = left.Hitbox; //BUG FIX: Freeze when repulsion too small to make a difference.
                    if (horizontal)
                    {
                        left.Hitbox.Left += overlap.Width * (overlap.Left == left.Hitbox.Left ? 1 : -1);
                    }
                    else
                    {
                        left.Hitbox.Top += overlap.Height * (overlap.Top == left.Hitbox.Top ? 1 : -1);
                    }
                    if(orig.Left == left.Hitbox.Left && orig.Top == left.Hitbox.Top) break;
                }
            }
            //TODO: Make this work.
            else if (left.Hitbox.Intersects(right.Hitbox, out var overlap))
            {
                var horizontal = overlap.Height > overlap.Width;
                if (!horizontal)
                    left.Hitbox.Left += deltaTime * overlap.Width * 1.5f;
                else
                    left.Hitbox.Top += deltaTime * overlap.Height * 1.5f;
            }

            t.Position = left.TransformPosition - left.Offset;
            return p != t.Position;
        }
    }

    [UpdateSystem]
    [Include(typeof(RepulsionComponent))]
    public sealed class RepulsionSystem2 : EventListenerSystem<CollisionEvent>, IUpdateSystem
    {
        public override bool ValidEvent(in CollisionEvent e)
        {
            return base.ValidEvent(in e) && (Ecs.GetEntityArchetype(e.Target) & Ecs.GetSignature<MovementComponent>()) != 0;
        }

        public void PreExecution()
        {
        }

        public void Execute(float deltaTime)
        {
            var count = Events.Count;
            for (var index = 0; index < count; index++)
            {
                if (!Events.TryPop(out var collisionEvent)) continue;
                var repulse = collisionEvent.Sender.Get<RepulsionComponent>().Force;
                ref var collider = ref collisionEvent.Target.Get<CollisionComponent>();
                var overlap = collisionEvent.Overlap;
                var horizontal = overlap.Height > overlap.Width;
                
                if (horizontal)
                {
                    collider.Hitbox.Left += (repulse.X + overlap.Width) * (overlap.Left == collider.Hitbox.Left ? 1 : -1);
                }
                else
                {
                    collider.Hitbox.Top += (repulse.Y + overlap.Height) * (overlap.Top == collider.Hitbox.Top ? 1 : -1);
                }

                collisionEvent.Target.Get<TransformComponent>().Position = collider.TransformPosition - collider.Offset;
            }
        }

        public void PostExecution()
        {
        }
    }
}