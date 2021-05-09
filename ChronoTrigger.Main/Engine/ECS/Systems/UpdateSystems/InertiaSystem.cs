using System.Numerics;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    public sealed class InertiaSystem : UpdateComponentSystem<MovementComponent>
    {
        public override void ActOnComponent(ref MovementComponent component, float deltaTime)
        {
            //TODO: Magic number. Mass?
            component.Velocity *= 0.5f;
            if (component.Speed >= 0.05) return;
            component.Velocity *= 0;
        }
    }
}