using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(MovementComponent))]
    [Include(typeof(TransformComponent))]
    public sealed class PushSystem : EventListenerSystem<CollisionEvent>, IUpdateSystem
    {
        public override bool ValidEvent(in CollisionEvent e)
        {
            return base.ValidEvent(e) && base.ValidEvent(new ()
            {
                Overlap = e.Overlap,
                Sender = e.Target,
                Target = e.Sender
            });
        }

        public void PreExecution() { }

        public void Execute(float deltaTime)
        {
            var count = Events.Count;
            for (var index = 0; index < count; index++)
            {
                if (!Events.TryPop(out var collisionEvent)) continue;
                if (!collisionEvent.Sender.Get<CollisionComponent>().Solid) continue;
                ref var movementSender = ref collisionEvent.Sender.Get<MovementComponent>();
                ref var movementTarget = ref collisionEvent.Target.Get<MovementComponent>();
                movementSender.Velocity += movementTarget.Velocity;
            }
        }

        public void PostExecution() { }
    }
}