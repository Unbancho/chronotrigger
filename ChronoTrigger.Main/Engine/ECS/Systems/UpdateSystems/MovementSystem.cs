using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(TransformComponent))]
    [Include(typeof(MovementComponent))]
    public sealed class MovementSystem : UpdateEntitySystem
    {
        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            Move(ref entity.Get<TransformComponent>(), entity.Get<MovementComponent>());
        }

        private static void Move(ref TransformComponent tComponent, MovementComponent mComponent)
        {
            tComponent.Position += mComponent.Velocity;
        }
    }
}