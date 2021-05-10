#define UNMANAGED

using System.Collections.Generic;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Scenes;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;
using ModusOperandi.Rendering;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ECS.Systems.DrawSystems
{
    [DrawSystem]
    public sealed class DebugDrawSystem<T> : DrawComponentSystem<T> where T :
#if UNMANAGED
        unmanaged
#else
        struct
#endif
        , IDebugDrawableComponent
    {
        public override void DrawComponent(T component, SpriteBatch spriteBatch)
        {
            switch (component.DebugDrawable)
            {
                case Sprite sprite:
                    spriteBatch.Draw(sprite);
                    break;
                case Text text:
                    spriteBatch.Draw(text);
                    break;
            }
        }
    }
    
    [DrawSystem]
    [Include(typeof(CollisionComponent))]
    [Include(typeof(TransformComponent))]
    public sealed class DebugDrawCollisionSystem : EntitySystem, IDrawSystem
    {
        public Scene Scene { get; set; }
        public List<ISystem> ComplementarySystems { get; } = new ();
        public bool Parallel { get; set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            var entities = Ecs.Query(Archetypes[0]);
            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var pos = entity.Get<TransformComponent>().TransformPosition;
                var collision = entity.Get<CollisionComponent>();
                var sprite = (Sprite) collision.DebugDrawable;
                sprite.Position = pos.ToVector2f() + collision.Offset.ToVector2f();
                spriteBatch.Draw(sprite);
            }
        }
    }
}