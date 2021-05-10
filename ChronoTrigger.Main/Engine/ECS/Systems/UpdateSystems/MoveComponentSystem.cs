#define UNMANAGED

using System;
using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Extensions;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
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
                int SpriteComparison(Sprite sprite)
                {
                    var adjustedHeight = sprite.TransformComponent.Position.Y + sprite.TextureComponent.TextureRect.Height;
                    var otherAdjustedHeight =
                        other.TransformComponent.Position.Y + other.TextureComponent.TextureRect.Height;
                    var yc = adjustedHeight.CompareTo(otherAdjustedHeight);
                    return yc != 0 ? yc : 1;
                }

                if (TextureComponent.Layer == other.TextureComponent.Layer &&
                    TextureComponent.Layer == LayerEnum.Sprite)
                    return SpriteComparison(this);
                var layerCompare = ((sbyte) TextureComponent.Layer).CompareTo((sbyte) other.TextureComponent.Layer);
                return layerCompare != 0 ? layerCompare : 1;
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
            var parentTransform = parent.Parent.Get<TransformComponent>();
            var offset = parent.Offset;
            childPosition = parentTransform.Position - offset;
        }
    }
}