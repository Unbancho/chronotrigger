using System;
using System.Collections.Generic;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    public readonly struct CollisionEvent : InteractiveComponent.IInteractionEvent, IEquatable<CollisionEvent>
    {
        public readonly MoveCollisionSystem.CollisionPackage SenderPackage;
        public readonly MoveCollisionSystem.CollisionPackage TargetPackage;
        public Entity Sender => SenderPackage.Entity;
        public Entity Target => TargetPackage.Entity;
        public readonly RotatingRect Overlap;

        public CollisionEvent(MoveCollisionSystem.CollisionPackage sender, MoveCollisionSystem.CollisionPackage target, 
            RotatingRect overlap)
        {
            SenderPackage = sender;
            TargetPackage = target;
            Overlap = overlap;
        }

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

        public void PreExecution() { }

        public void Execute(float deltaTime)
        {
            if (!Parallel)
            {
                var set = new Dictionary<float, CollisionEvent>();
                var components = MoveCollisionSystem.GetAll;
                for (var i = 0; i < components.Length; i++)
                {
                    var componentA = components[i];
                    var bs = MoveCollisionSystem.Nearby(componentA);
                    for (var index = 0; index < bs.Length; index++)
                    {
                        var componentB = bs[index];
                        if (!Colliding(componentA.Rect, componentB.Rect, out var overlap)) continue;
                        set[componentA.Entity + componentB.Entity*0.1f] = new(componentA, componentB, overlap);
                        set[componentB.Entity + componentA.Entity *0.1f] = new(componentB, componentA, overlap);
                    }
                }

                foreach (var @event in set.Values)
                {
                    Emit(@event);
                }
            }
        }

        public void PostExecution() { }

        private static bool Colliding(RotatingRect c, RotatingRect tc, out RotatingRect overlap)
        {
            return c.IntersectsOrTouches(tc, out overlap);
        }
    }
}