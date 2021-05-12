using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    public class BoundsCollisionSystem : UpdateComponentSystem<TransformComponent, GameLoop.GameState>
    {
        public override void ActOnComponent(ref TransformComponent component, GameLoop.GameState gameState)
        {
            ref var position = ref component.Position;
            if(position.X > Scene.Bounds.X)
                position.X = Scene.Bounds.X;
            else if (position.X < 0)
                position.X = 0;
            if (position.Y > Scene.Bounds.Y)
                position.Y = Scene.Bounds.Y;
            else if(position.Y < 0)
                position.Y = 0;
        }
    }
}