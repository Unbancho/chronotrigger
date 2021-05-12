using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(WarpComponent))]
    public sealed class WarpEventSystem : EventListenerSystem<CollisionEvent>, ISystem<GameLoop.GameState>
    {
        public void Run(GameLoop.GameState gameState)
        {
            var count = Events.Count;
            for (var index = 0; index < count; index++)
            {
                if (!Events.TryPop(out var collisionEvent)) continue;
                var warp = collisionEvent.Sender.Get<WarpComponent>();
            }
        }
    }
}