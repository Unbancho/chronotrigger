using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    public readonly struct CollisionEvent : InteractiveComponent.IInteractionEvent, IEquatable<CollisionEvent>
    {
        public Entity Sender { get; init; }
        public Entity Target { get; init; }
        public RotatingRect Overlap { get; init; }

        public bool Equals(CollisionEvent other)
        {
            return Sender == other.Sender && Target == other.Target;
        }
    }

    [UpdateSystem]
    public sealed class CollisionSystem : EventEmitterSystem<CollisionEvent>, IUpdateSystem
    {
        public CollisionSystem()
        { 
            ComplementarySystems.Add(new MoveCollisionSystem());
            Parallel = false;
        }
        
        private void Do(int i, IReadOnlyList<CollisionComponent> components)
        {
            var componentA = components[i];
            for (var j = i + 1; j < components.Count; j++)
            {
                var componentB = components[j];
                if (!Colliding(componentA, componentB, out var overlap)) continue;
                var sender = componentA.Entity;
                var target = componentB.Entity;
                Emit(new()
                {
                    Sender = sender,
                    Target = target,
                    Overlap = overlap
                });
                Emit(new()
                {
                    Sender = target,
                    Target = sender,
                    Overlap = overlap
                });
            }
        }

        public void PreExecution() { }

        public void Execute(float deltaTime)
        {
            if (!Parallel)
            {
                var set = new HashSet<CollisionEvent>();
                var components = Ecs.GetComponentManager<CollisionComponent>().Components.ComponentArray;
                for (var i = 0; i < components.Length; i++)
                {
                    var componentA = components[i];
                    var bs = MoveCollisionSystem.Nearby(componentA);
                    for (var index = 0; index < bs.Length; index++)
                    {
                        var componentB = bs[index];
                        if (!Colliding(componentA, componentB, out var overlap)) continue;
                        var sender = componentA.Entity;
                        var target = componentB.Entity;
                        set.Add(new()
                        {
                            Sender = sender,
                            Target = target,
                            Overlap = overlap
                        });
                        set.Add(new()
                        {
                            Sender = target,
                            Target = sender,
                            Overlap = overlap
                        });
                    }
                }

                foreach (var @event in set)
                {
                    Emit(@event);
                }
            }
        }

        public void PostExecution() { }

        private static bool Colliding(CollisionComponent c, CollisionComponent tc, out RotatingRect overlap)
        {
            return c.Hitbox.IntersectsOrTouches(tc.Hitbox, out overlap);
        }
    }
}