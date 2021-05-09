#define UNMANAGED

using ChronoTrigger.Engine.ECS.Components;
using ChronoTrigger.Engine.ECS.Systems.UpdateSystems;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.Rendering;
using SFML.Graphics;

namespace ChronoTrigger.Engine.ECS.Systems.DrawSystems
{
    [DrawSystem]
    public sealed class TextureLayerDraw<T> : DrawComponentSystem<T> where T :
#if UNMANAGED
        unmanaged
#else
        struct
#endif
        , ITexturedComponent, ITransformableComponent
    {
        public TextureLayerDraw()
        {
            ComplementarySystems.Add(new MoveComponentSystem<T>());
        }

        public override void DrawComponent(T component, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(component.TexturePtr, component.TransformPosition, component.TextureRect,
                component.Color, component.Scale, component.Origin);
        }
    }

    [DrawSystem]
    public sealed class DebugDrawSystem<T> : DrawComponentSystem<T> where T :
#if UNMANAGED
        unmanaged
#else
        struct
#endif
        , IDebugDrawableComponent, ITransformableComponent
    {
        public DebugDrawSystem()
        {
            ComplementarySystems.Add(new MoveComponentSystem<T>());
        }

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
}