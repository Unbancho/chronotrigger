#define UNMANAGED

using System;
using System.Collections.Generic;
using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Extensions;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    public readonly struct CombinedComponent<T1, T2>
    {
        public readonly T1 Component1;
        public readonly T2 Component2;

        public CombinedComponent(T1 component1, T2 component2)
        {
            Component1 = component1;
            Component2 = component2;
        }
    }
    public class CombineComponentsSystem<T1, T2, TS> : UpdateEntitySystem
        where T1 : unmanaged
        where T2 : unmanaged
        where TS : ICollection<CombinedComponent<T1, T2>>, new()
    {
        public static readonly TS CombinedComponents = new();
        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            CombinedComponents.Add(new CombinedComponent<T1, T2>());
        }
    }
    
    
    [UpdateSystem]
    public class MoveComponentSystem<T> : UpdateEntitySystem where T :
#if UNMANAGED
        unmanaged
#else
        struct
#endif
        , ITransformableComponent
    {
        public MoveComponentSystem()
        {
            var a = Archetypes[0];
            Archetypes[0] = new (a.Signature | Ecs.GetSignature<T>(), a.AntiSignature);
        }
        
        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            entity.Get<T>().TransformPosition = entity.Get<TransformComponent>().Position;
        }
    }

    [UpdateSystem]
    [Include(typeof(TextureComponent))]
    [Include(typeof(TransformComponent))]
    public sealed class MoveSpriteSystem : UpdateEntitySystem
    {
        public readonly struct Sprite : IComparable<Sprite>
        {
            public readonly TextureComponent TextureComponent;
            public readonly TransformComponent TransformComponent;

            public Sprite(TextureComponent textureComponent, TransformComponent transformComponent)
            {
                TextureComponent = textureComponent;
                TransformComponent = transformComponent;
            }

            public int CompareTo(Sprite other)
            {
                var adjustedHeight = TransformComponent.Position.Y + TextureComponent.TextureRect.Height;
                var otherAdjustedHeight =
                    other.TransformComponent.Position.Y + other.TextureComponent.TextureRect.Height;
                var yc = adjustedHeight.CompareTo(otherAdjustedHeight);
                return yc != 0 ? yc : 1;
            }
        }
        
        private static readonly SortedSetF<Sprite> SortedSprites = new();

        public static Span<Sprite> SortedSpritesSafe
        {
            get
            {
                var arr = new Sprite[SortedSprites.Count];
                SortedSprites.CopyTo(arr);
                SortedSprites.Clear();
                return (Span<Sprite>) arr;
            }
        }

        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            ref var textureComponent = ref entity.Get<TextureComponent>();
            SortedSprites.Add(new (textureComponent, entity.Get<TransformComponent>()));
        }
    }
    
    [UpdateSystem]
    [Include(typeof(TransformComponent))]
    [Include(typeof(CollisionComponent))]
    public sealed class MoveCollisionSystem : UpdateEntitySystem
    {
        private static readonly SpatialHash<CollisionPackage> Components = new(10);

        public static CollisionPackage[] Nearby(CollisionPackage c) => Components.GetNearby(c);

        public static CollisionPackage[] GetAll => Components.Elements.ToArray();

        public override void Execute(float deltaTime)
        {
            Components.Clear();
            base.Execute(deltaTime);
        }
        
        public struct CollisionPackage : ITransformableComponent, ISizeableComponent, IEquatable<CollisionPackage>
        {
            public RotatingRect Rect;
            public Entity Entity;
            public Vector2 TransformPosition
            {
                get => Rect.Position;
                set => Rect.Position = value;
            }
            public Vector2 Size
            {
                get => Rect.Size;
                set => Rect.Size = value;
            }
            public bool Equals(CollisionPackage other)
            {
                return Entity == other.Entity;
            }
        }

        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            var collisionComponent = entity.Get<CollisionComponent>();
            var position = entity.Get<TransformComponent>().Position + collisionComponent.Offset;
            var rotatingRect = new RotatingRect()
            {
                Size = collisionComponent.Hitbox,
                Position = position
            };
            Components.AddBox(new ()
            {
                Rect = rotatingRect,
                Entity = entity
            });
        }
    }

    [UpdateSystem]
    [Include(typeof(ParentComponent))]
    [Include(typeof(TransformComponent))]
    public sealed class MoveChildSystem : UpdateEntitySystem
    {
        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            ref var childPosition = ref entity.Get<TransformComponent>().Position;
            var parent = entity.Get<ParentComponent>();
            var parentTransform = parent.Entity.Get<TransformComponent>();
            var offset = parent.Offset;
            childPosition = parentTransform.Position - offset;
        }
    }
}