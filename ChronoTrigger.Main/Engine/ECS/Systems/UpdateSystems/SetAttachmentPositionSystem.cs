using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    public sealed class
        SetAttachmentPositionSystem : UpdateEntitySystem
    {
        public override void ActOnEntity(Entity entity, float deltaTime)
        {
            MoveAttachment(ref entity.Get<TransformComponent>(),
                ref entity.Get<AttachComponent>());
        }

        private static void MoveAttachment(ref TransformComponent transformComponent,
            ref AttachComponent attachComponent)
        {
            transformComponent.Position =
                attachComponent.Target.Get<TransformComponent>().Position + attachComponent.Offset;
        }
    }
}