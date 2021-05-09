using ChronoTrigger.Engine.Controls;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Components;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    // TODO: This.
    [UpdateSystem]
    [Include(typeof(InteractiveComponent))]
    public sealed class InteractionSystem : EventListenerSystem<CollisionEvent>, IUpdateSystem
    {
        private static bool _held;

        public void PreExecution() { }

        public void Execute(float deltaTime)
        {
            var pressed = Buttons.A.IsPressed();
            if (!pressed)
            {
                _held = false;
                Events.Clear();
                return;
            }

            if (_held)
            {
                Events.Clear();
                return;
            }

            _held = true;
            var count = Events.Count;
            for (var i = 0; i < count; i++)
            {
                if (!Events.TryPop(out var e)) continue;
                if((Ecs.GetEntityArchetype(e.Target) & Ecs.GetSignature<InteractionComponent>()) == 0)
                    continue;
                Interact(e);
            }
        }

        public void PostExecution() { }

        private static void Interact(InteractiveComponent.IInteractionEvent @event)
        {
            ref var interactive = ref @event.Sender.Get<InteractiveComponent>();
            //var actives = interactive.ScriptSignature & interactive.FlagTriggerSignature;
            interactive.RunScript(@event);
        }
    }
}