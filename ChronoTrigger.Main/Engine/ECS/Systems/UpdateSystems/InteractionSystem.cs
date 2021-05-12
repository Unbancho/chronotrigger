using ChronoTrigger.Engine.Controls;
using ChronoTrigger.Engine.ECS.Components;
using ModusOperandi.ECS;
using ModusOperandi.ECS.Entities;
using ModusOperandi.ECS.Systems;
using ModusOperandi.ECS.Systems.SystemAttributes;
using ModusOperandi.ECS.Systems.SystemInterfaces;

namespace ChronoTrigger.Engine.ECS.Systems.UpdateSystems
{
    // TODO: This.
    [UpdateSystem]
    [Include(typeof(InteractiveComponent))]
    public sealed class InteractionSystem : EventListenerSystem<CollisionEvent>, ISystem<GameLoop.GameState>
    {
        private static bool _held;

        private static void Interact<T>(T @event) where T: InteractiveComponent.IInteractionEvent
        {
            ref var interactive = ref @event.Sender.Get<InteractiveComponent>();
            //var actives = interactive.ScriptSignature & interactive.FlagTriggerSignature;
            interactive.RunScript(@event);
        }

        public void Run(GameLoop.GameState gameState)
        {
            var pressed = (gameState.InputState & Buttons.A) != 0;
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
    }
}