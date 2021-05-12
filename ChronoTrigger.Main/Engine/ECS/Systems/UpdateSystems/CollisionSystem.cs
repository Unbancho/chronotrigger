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

        private readonly struct Pair : IEquatable<Pair>
        {
            private readonly uint _a;
            private readonly uint _b;

            public Pair(uint a, uint b)
            {
                _a = a;
                _b = b;
            }

            public bool Equals(Pair other)
            {
                return _a == other._a && _b == other._b;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17;
                    hash = hash * 23 + _a.GetHashCode();
                    hash = hash * 23 + _b.GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                return obj is Pair pair && Equals(pair);
            }
        }

        private static bool Colliding(RotatingRect c, RotatingRect tc, out RotatingRect overlap)
        {
            return c.IntersectsOrTouches(tc, out overlap);
        }
        
        public void Run(GameLoop.GameState gameState)
        {
            if (!Parallel)
            {
                var set = new Dictionary<Pair, CollisionEvent>();
                var components = MoveCollisionSystem.GetAll;
                for (var i = 0; i < components.Length; i++)
                {
                    var componentA = components[i];
                    var bs = MoveCollisionSystem.Nearby(componentA);
                    for (var index = 0; index < bs.Length; index++)
                    {
                        var componentB = bs[index];
                        if (!Colliding(componentA.Rect, componentB.Rect, out var overlap)) continue;
                        set[new(componentA.Entity.ID, componentB.Entity.ID)] = new(componentA, componentB, overlap);
                        set[new(componentB.Entity.ID, componentA.Entity.ID)] = new(componentB, componentA, overlap);
                    }
                }

                foreach (var @event in set.Values)
                {
                    Emit(@event);
                }
            }
            else
            {
                var components = MoveCollisionSystem.GetAll;
                var collisionsArray = new HashSet<CollisionEvent>[components.Length];
                for (var index = 0; index < collisionsArray.Length; index++)
                {
                    collisionsArray[index] = new();
                }
                System.Threading.Tasks.Parallel.For(0, components.Length, (i) =>
                {
                    var componentA = components[i];
                    var bs = MoveCollisionSystem.Nearby(componentA);
                    for (var index = 0; index < bs.Length; index++)
                    {
                        var componentB = bs[index];
                        if (!Colliding(componentA.Rect, componentB.Rect, out var overlap)) continue;
                        collisionsArray[i].Add(new(componentA, componentB, overlap));
                        collisionsArray[i].Add(new(componentB, componentA, overlap));
                    }
                });
                var set = new HashSet<CollisionEvent>();
                foreach (var s in collisionsArray)
                {
                    set.UnionWith(s);
                }
                foreach (var @event in set)
                {
                    Emit(@event);
                }
            }
        }
    }
}