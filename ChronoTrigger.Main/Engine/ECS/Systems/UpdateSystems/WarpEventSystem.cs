using System;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    [UpdateSystem]
    [Include(typeof(WarpComponent))]
    public sealed class WarpEventSystem : EventListenerSystem<CollisionEvent>, IUpdateSystem
    {
        public void PreExecution() { }

        public void Execute(float deltaTime)
        {
            var count = Events.Count;
            for (var index = 0; index < count; index++)
            {
                if (!Events.TryPop(out var collisionEvent)) continue;
                var warp = collisionEvent.Sender.Get<WarpComponent>();
                Console.WriteLine(warp.Scene);
            }
        }

        public void PostExecution() { }
    }
}