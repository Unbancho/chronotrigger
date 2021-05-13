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
    public sealed class CollisionSystem : EventEmitterSystem<CollisionEvent>, ISystem<GameLoop.GameState>
    {
        public CollisionSystem()
        { 
            ComplementarySystems.Add(new MoveCollisionSystem());
            Parallel = true;
        }

        private static bool Colliding(RotatingRect c, RotatingRect tc, out RotatingRect overlap)
        {
            return c.IntersectsOrTouches(tc, out overlap);
        }
        
        public void Run(GameLoop.GameState gameState)
        {
            var components = MoveCollisionSystem.GetAll;
            if (!Parallel)
            {
                var set = new List<CollisionEvent>();
                for (var i = 0; i < components.Length; i++)
                {
                    var componentA = components[i];
                    var bs = MoveCollisionSystem.Nearby(componentA);
                    var cache = new bool[MoveCollisionSystem.MaxEntity+1];
                    for (var index = 0; index < bs.Length; index++)
                    {
                        var componentB = bs[index];
                        if (cache[componentB.Entity.ID]) continue;
                        if (!Colliding(componentA.Rect, componentB.Rect, out var overlap)) continue;
                        set.Add(new(componentA, componentB, overlap));
                        cache[componentB.Entity.ID] = true;
                    }
                }

                foreach (var @event in set)
                {
                    Emit(@event);
                }
            }
            else
            {
                var collisionsArray = new List<CollisionEvent>[components.Length];
                for (var index = 0; index < collisionsArray.Length; index++)
                {
                    collisionsArray[index] = new();
                }
                System.Threading.Tasks.Parallel.For(0, components.Length, (i) =>
                {
                    var componentA = components[i];
                    var bs = MoveCollisionSystem.Nearby(componentA);
                    var cache = new bool[MoveCollisionSystem.MaxEntity+1];
                    for (var index = 0; index < bs.Length; index++)
                    {
                        var componentB = bs[index];
                        if (cache[componentB.Entity.ID]) continue;
                        if (!Colliding(componentA.Rect, componentB.Rect, out var overlap)) continue;
                        collisionsArray[i].Add(new(componentA, componentB, overlap));
                        cache[componentB.Entity.ID] = true;
                    }
                });
                for (var i = 0; i < collisionsArray.Length; i++)
                {
                    var set = collisionsArray[i];
                    for (var index = 0; index < set.Count; index++)
                    {
                        var @event = set[index];
                        Emit(@event);
                    }
                }
            }
        }
    }
}