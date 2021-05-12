using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    public sealed class InertiaSystem : UpdateComponentSystem<MovementComponent, GameLoop.GameState>
    {
        public override void ActOnComponent(ref MovementComponent component, GameLoop.GameState gameState)
        {
            component.Velocity *= 0.5f*gameState.DeltaTime;
            if (component.Speed >= 0.05) return;
            component.Velocity *= 0;
        }
    }
}